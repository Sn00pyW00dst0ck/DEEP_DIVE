using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    const int threadGroupSize = 1024;


    public BoidSettings settings;
    public ComputeShader compute;
    Boid[] boids;

    // Start is called before the first frame update
    void Start()
    {
        // Find all the boids and add them to the manager's list
        boids = FindObjectsOfType<Boid>();
        foreach (Boid b in boids) 
        {
            b.Initialize(settings);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (boids.Length == 0) { return; }

        int numBoids = boids.Length;
        BoidData[] boidData = new BoidData[numBoids];

        // Populate a list of boidData structs 
        for (int i = 0; i < boids.Length; i++)
        {
            boidData[i].position = boids[i].position;
            boidData[i].direction = boids[i].forward;
        }

        // Make a 'ComputeBuffer' of the boid data using the list
        ComputeBuffer boidDataBuffer = new(numBoids, BoidData.Size);
        boidDataBuffer.SetData(boidData);

        // Pass the buffer, number of boids, and settings data to the shader
        compute.SetBuffer(0, "boids", boidDataBuffer);
        compute.SetInt("numBoids", boids.Length);
        compute.SetFloat("viewRadius", settings.perceptionRadius);
        compute.SetFloat("avoidRadius", settings.avoidanceRadius);

        // Dispatch the shader
        int threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);
        compute.Dispatch(0, threadGroups, 1, 1);

        // Get the buffer from the shader
        boidDataBuffer.GetData(boidData);

        // Update the boids
        for (int i = 0; i < boids.Length; i++)
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
