using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    const int threadGroupSize = 1024;

    public BoidSettings settings;
    public ComputeShader compute;
    private List<Boid> boids = new List<Boid>();

    [SerializeField]
    private List<Transform> targets = new List<Transform>();


    // Update is called once per frame
    void Update()
    {
        if (boids.Count == 0) { return; }

        int numBoids = boids.Count;
        BoidData[] boidData = new BoidData[numBoids];

        // Populate a list of boidData structs 
        for (int i = 0; i < boids.Count; i++)
        {
            boidData[i].position = boids[i].position;
            boidData[i].direction = boids[i].forward;
        }

        // Make a 'ComputeBuffer' of the boid data using the list
        ComputeBuffer boidDataBuffer = new(numBoids, BoidData.Size);
        boidDataBuffer.SetData(boidData);

        // Pass the buffer, number of boids, and settings data to the shader
        compute.SetBuffer(0, "boids", boidDataBuffer);
        compute.SetInt("numBoids", boids.Count);
        compute.SetFloat("viewRadius", settings.perceptionRadius);
        compute.SetFloat("avoidRadius", settings.avoidanceRadius);

        // Dispatch the shader
        int threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);
        compute.Dispatch(0, threadGroups, 1, 1);

        // Get the buffer from the shader
        boidDataBuffer.GetData(boidData);

        // Update the boids
        for (int i = 0; i < boids.Count; i++)
        {
            boids[i].avgFlockHeading = boidData[i].flockHeading;
            boids[i].centreOfFlockmates = boidData[i].flockCentre;
            boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
            boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

            boids[i].UpdateBoid();
        }

        // Release the 'ComputerBuffer'
        boidDataBuffer.Release();
    }


    #region Add and Remove Boids
    
    public void AddBoid(Boid b)
    {
        boids.Add(b);
    }

    public void RemoveBoid(Boid b)
    {
        boids.Remove(b);
    }

    #endregion Add and Remove Boids


    public struct BoidData
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size { get { return sizeof(float) * 3 * 5 + sizeof(int); } }
    }
}
