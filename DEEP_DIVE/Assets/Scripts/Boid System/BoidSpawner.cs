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
            // Get random position and forward vectors
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Boid boid = Instantiate(prefab);
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;

            boid.SetColor(color);
        }
    }

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
}
