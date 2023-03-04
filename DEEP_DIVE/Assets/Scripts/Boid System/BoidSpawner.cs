using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{

    #region State Variables

    public enum GizmoType { Never, SelectedOnly, Always }

    [Header("Spawned Boid Settings")]
    public Boid prefab;
    public BoidSettings settings;
    public BoidManager manager;
    public Color color;

    [Header("Spawner Attributes")]
    public float spawnRadius = 10;
    public int spawnCount = 10;
    public GizmoType showSpawnRegion;

    #endregion State Variables

    void Awake()
    {
        // Spawn requested amount of boids 
        for (int i = 0; i < spawnCount; i++)
        {   
            Boid boid = Instantiate(prefab);
            boid.SetColor(color);
            boid.transform.parent = this.transform;

            // Get random position and forward vectors
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;
            boid.Initialize(settings);

            manager.AddBoid(boid);
        }
    }

    private void Start()
    {
        if (settings != manager.settings)
        {
            Debug.LogError("settings object must match the manager's settings");
        }
    }

    #region Gizmo Settings

    private void OnDrawGizmos()
    {
        if (showSpawnRegion == GizmoType.Always) { DrawGizmos(); }
    }

    void OnDrawGizmosSelected()
    {
        if (showSpawnRegion == GizmoType.SelectedOnly) { DrawGizmos(); }
    }

    void DrawGizmos()
    {
        Gizmos.color = new Color(color.r, color.g, color.b, 0.3f);
        Gizmos.DrawSphere(transform.position, spawnRadius);
    }

    #endregion Gizmo Settings

}
