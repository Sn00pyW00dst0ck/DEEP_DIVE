using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

struct Boid3D
{
    public Vector3 pos;
    public Vector3 vel;
    float pad0;
    float pad1;
}

public class BoidMain : MonoBehaviour
{

    const float blockSize = 256f;

    [Header("Boid Settings")]
    [SerializeField] int numBoids = 32;
    [SerializeField] float boidScale = 0.08f;
    [SerializeField] BoidSettings settings;

    [Header("Allowed Area")]
    [SerializeField] float edgeMargin = 0.5f;
    [SerializeField] float spaceBounds = 5.0f;
    float xBound, yBound, zBound;

    [Header("Prefabs")]
    [SerializeField] ComputeShader boidComputeShader;
    [SerializeField] ComputeShader gridShader;
    [SerializeField] Material boidMaterial;
    [SerializeField] Mesh boidMesh;

    // Render Info
    RenderParams rp;
    GraphicsBuffer coneTriangles, conePositions, coneNormals;
    int triangleCount; // constant for the cone boid settings

    // Kernel IDs
    int updateBoidsKernel, generateBoidsKernel;
    int updateGridKernel, clearGridKernel, prefixSumKernel, sumBlocksKernel, addSumsKernel, rearrangeBoidsKernel;

    // Buffers for the compute shaders
    ComputeBuffer boidBuffer;
    ComputeBuffer boidBufferOut;
    ComputeBuffer gridBuffer;
    ComputeBuffer gridOffsetBuffer;
    ComputeBuffer gridOffsetBufferIn;
    ComputeBuffer gridSumsBuffer;
    ComputeBuffer gridSumsBuffer2;

    // Index is particle ID, x value is position flattened to 1D array, y value is grid cell offset
    int gridDimY, gridDimX, gridDimZ, gridTotalCells, blocks;
    float gridCellSize;

    void Start()
    {
        #region Set Boid Boundaries

        // Set environment bounds information
        xBound = 2 * (spaceBounds - edgeMargin);
        yBound = (spaceBounds - edgeMargin);
        zBound = 2 * (spaceBounds - edgeMargin);

        #endregion Set Boid Boundaries

        #region Grab Kernel Function IDs

        updateBoidsKernel = boidComputeShader.FindKernel("UpdateBoids");
        generateBoidsKernel = boidComputeShader.FindKernel("GenerateBoids");
        updateGridKernel = gridShader.FindKernel("UpdateGrid");
        clearGridKernel = gridShader.FindKernel("ClearGrid");
        prefixSumKernel = gridShader.FindKernel("PrefixSum");
        sumBlocksKernel = gridShader.FindKernel("SumBlocks");
        addSumsKernel = gridShader.FindKernel("AddSums");
        rearrangeBoidsKernel = gridShader.FindKernel("RearrangeBoids");

        #endregion Grab Kernel Function IDs

        #region Setup boidComputeShader Inputs

        boidBuffer = new ComputeBuffer(numBoids, 32);
        boidBufferOut = new ComputeBuffer(numBoids, 32);
        boidComputeShader.SetBuffer(updateBoidsKernel, "boidsIn", boidBufferOut);
        boidComputeShader.SetBuffer(updateBoidsKernel, "boidsOut", boidBuffer);
        boidComputeShader.SetInt("numBoids", numBoids);
        boidComputeShader.SetFloat("maxSpeed", settings.maxSpeed);
        boidComputeShader.SetFloat("minSpeed", settings.minSpeed);
        boidComputeShader.SetFloat("turnSpeed", settings.turnSpeed);
        boidComputeShader.SetFloat("edgeMargin", edgeMargin);
        boidComputeShader.SetFloat("visualRange", settings.visualRange);
        boidComputeShader.SetFloat("minDistance", settings.minDistance);
        boidComputeShader.SetFloat("xBound", xBound);
        boidComputeShader.SetFloat("yBound", yBound);
        boidComputeShader.SetFloat("zBound", zBound);

        #endregion Setup boidComputeShader Inputs

        #region Generate Boids On GPU

        boidComputeShader.SetBuffer(generateBoidsKernel, "boidsOut", boidBuffer);
        boidComputeShader.SetInt("randSeed", UnityEngine.Random.Range(0, int.MaxValue));
        boidComputeShader.Dispatch(generateBoidsKernel, Mathf.CeilToInt(numBoids / blockSize), 1, 1);

        #endregion Generate Boids On GPU

        #region Rendering Shader Setup Code
        // Get required variable from model
        this.triangleCount = boidMesh.triangles.Length;
        
        rp = new RenderParams(boidMaterial);
        rp.matProps = new MaterialPropertyBlock();
        rp.matProps.SetFloat("_Scale", boidScale);
        rp.matProps.SetBuffer("boids", boidBuffer);
        rp.shadowCastingMode = ShadowCastingMode.On;
        rp.receiveShadows = true;
        rp.worldBounds = new Bounds(Vector3.zero, Vector3.one * 100);
        coneTriangles = new GraphicsBuffer(GraphicsBuffer.Target.Structured, boidMesh.triangles.Length, sizeof(int));
        coneTriangles.SetData(boidMesh.triangles);
        conePositions = new GraphicsBuffer(GraphicsBuffer.Target.Structured, boidMesh.vertices.Length, 3 * sizeof(float));
        conePositions.SetData(boidMesh.vertices);
        coneNormals = new GraphicsBuffer(GraphicsBuffer.Target.Structured, boidMesh.normals.Length, 3 * sizeof(float));
        coneNormals.SetData(boidMesh.normals);
        rp.matProps.SetBuffer("coneTriangles", coneTriangles);
        rp.matProps.SetBuffer("conePositions", conePositions);
        rp.matProps.SetBuffer("coneNormals", coneNormals);
        rp.matProps.SetInteger("triangleCount", triangleCount);

        #endregion Rendering Shader Setup Code

        #region GPU Space 'Grid' Setup

        gridCellSize = settings.visualRange;
        gridDimX = Mathf.FloorToInt(xBound * 2 / gridCellSize) + 20;
        gridDimY = Mathf.FloorToInt(yBound * 2 / gridCellSize) + 20;
        gridDimZ = Mathf.FloorToInt(zBound * 2 / gridCellSize) + 20;
        gridTotalCells = gridDimX * gridDimY * gridDimZ;

        gridBuffer = new ComputeBuffer(numBoids, 8);
        gridOffsetBuffer = new ComputeBuffer(gridTotalCells, 4);
        gridOffsetBufferIn = new ComputeBuffer(gridTotalCells, 4);
        blocks = Mathf.CeilToInt(gridTotalCells / blockSize);
        gridSumsBuffer = new ComputeBuffer(blocks, 4);
        gridSumsBuffer2 = new ComputeBuffer(blocks, 4);
        gridShader.SetInt("numBoids", numBoids);
        gridShader.SetBuffer(updateGridKernel, "boids", boidBuffer);
        gridShader.SetBuffer(updateGridKernel, "gridBuffer", gridBuffer);
        gridShader.SetBuffer(updateGridKernel, "gridOffsetBuffer", gridOffsetBufferIn);
        gridShader.SetBuffer(updateGridKernel, "gridSumsBuffer", gridSumsBuffer);

        gridShader.SetBuffer(clearGridKernel, "gridOffsetBuffer", gridOffsetBufferIn);

        gridShader.SetBuffer(prefixSumKernel, "gridOffsetBuffer", gridOffsetBuffer);
        gridShader.SetBuffer(prefixSumKernel, "gridOffsetBufferIn", gridOffsetBufferIn);
        gridShader.SetBuffer(prefixSumKernel, "gridSumsBuffer", gridSumsBuffer2);

        gridShader.SetBuffer(addSumsKernel, "gridOffsetBuffer", gridOffsetBuffer);

        gridShader.SetBuffer(rearrangeBoidsKernel, "gridBuffer", gridBuffer);
        gridShader.SetBuffer(rearrangeBoidsKernel, "gridOffsetBuffer", gridOffsetBuffer);
        gridShader.SetBuffer(rearrangeBoidsKernel, "boids", boidBuffer);
        gridShader.SetBuffer(rearrangeBoidsKernel, "boidsOut", boidBufferOut);

        gridShader.SetFloat("gridCellSize", gridCellSize);
        gridShader.SetInt("gridDimY", gridDimY);
        gridShader.SetInt("gridDimX", gridDimX);
        gridShader.SetInt("gridDimZ", gridDimZ);
        gridShader.SetInt("gridTotalCells", gridTotalCells);
        gridShader.SetInt("blocks", blocks);

        #endregion GPU Space 'Grid' Setup

        // Pass Grid Info To Boid Compute Shader
        boidComputeShader.SetBuffer(updateBoidsKernel, "gridOffsetBuffer", gridOffsetBuffer);
        boidComputeShader.SetFloat("gridCellSize", gridCellSize);
        boidComputeShader.SetInt("gridDimY", gridDimY);
        boidComputeShader.SetInt("gridDimX", gridDimX);
        boidComputeShader.SetInt("gridDimZ", gridDimZ);
    }

    // Update is called once per frame
    void Update()
    {
        // Send updated deltas to the boid compute shader
        boidComputeShader.SetFloat("deltaTime", Time.deltaTime);
        boidComputeShader.SetFloat("cohesionFactor", settings.cohesionFactor);
        boidComputeShader.SetFloat("separationFactor", settings.separationFactor);
        boidComputeShader.SetFloat("alignmentFactor", settings.alignmentFactor);

        // Clear indices
        gridShader.Dispatch(clearGridKernel, blocks, 1, 1);

        // Populate grid
        gridShader.Dispatch(updateGridKernel, Mathf.CeilToInt(numBoids / blockSize), 1, 1);

        #region CountingSortBoids

        // Generate Offsets (Prefix Sum)
        // Offsets in each block
        gridShader.Dispatch(prefixSumKernel, blocks, 1, 1);

        // Offsets for sums of block
        bool swap = false;
        for (int d = 1; d < blocks; d *= 2)
        {
            gridShader.SetBuffer(sumBlocksKernel, "gridSumsBufferIn", swap ? gridSumsBuffer : gridSumsBuffer2);
            gridShader.SetBuffer(sumBlocksKernel, "gridSumsBuffer", swap ? gridSumsBuffer2 : gridSumsBuffer);
            gridShader.SetInt("d", d);
            gridShader.Dispatch(sumBlocksKernel, Mathf.CeilToInt(blocks / blockSize), 1, 1);
            swap = !swap;
        }

        // Apply offsets of sums to each block
        gridShader.SetBuffer(addSumsKernel, "gridSumsBufferIn", swap ? gridSumsBuffer : gridSumsBuffer2);
        gridShader.Dispatch(addSumsKernel, blocks, 1, 1);

        // Rearrange (sort) boids
        gridShader.Dispatch(rearrangeBoidsKernel, Mathf.CeilToInt(numBoids / blockSize), 1, 1);

        #endregion CountingSortBoids

        // Compute boid behaviours
        boidComputeShader.Dispatch(updateBoidsKernel, Mathf.CeilToInt(numBoids / blockSize), 1, 1);

        #region Obstacle Avoidance

        // Look into doing parallel collision detections with batching raycasts for better performance.
        // Transformation from forward to direction to check is not 100% right
        Boid3D[] boids = new Boid3D[numBoids];
        boidBuffer.GetData(boids);
        for (int i = 0; i < numBoids; i++)
        {
            // If there is an obstacle, get a direction to avoid the obstacle, then steer that way
            if (IsHeadingForCollision(boids[i]))
            {
                boids[i].vel += (GetNotCollidingDirection(boids[i]).normalized * settings.maxSpeed - boids[i].vel) * settings.avoidCollisionFactor;
                Vector3.ClampMagnitude(boids[i].vel, settings.maxSpeed);
            }
        }
        boidBuffer.SetData(boids);

        #endregion Obstacle Avoidance

        // Other boid behaviors can go here

        // Render everything
        Graphics.RenderPrimitives(rp, MeshTopology.Triangles, numBoids * triangleCount);
    }

    void OnDestroy()
    {
        // Release all the buffers
        boidBuffer.Release();
        boidBufferOut.Release();
        gridBuffer.Release();
        gridOffsetBuffer.Release();
        gridOffsetBufferIn.Release();
        gridSumsBuffer.Release();
        gridSumsBuffer2.Release();
        conePositions.Release();
        coneTriangles.Release();
        coneNormals.Release();
    }

    #region Collision Functions

    bool IsHeadingForCollision(Boid3D b)
    {
        return Physics.SphereCast(b.pos, settings.boundsRadius, b.vel, out _, settings.collisionAvoidDst, settings.obstacleMask);
    }

    Vector3 GetNotCollidingDirection(Boid3D b)
    {
        Vector3[] rayDirections = DirectionVectorGenerator.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            // Alternate to original direction calculation method
            // Based on this forum post: https://answers.unity.com/questions/356638/maths-behind-transformtransformdirection.html
            Vector3 dir = Vector3.Scale(b.vel, rayDirections[i]).normalized * b.vel.magnitude;
            Ray ray = new Ray(b.pos, dir);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask))
            {
                return dir;
            }
        }
        return b.vel;
    }

    #endregion Collision Functions

}