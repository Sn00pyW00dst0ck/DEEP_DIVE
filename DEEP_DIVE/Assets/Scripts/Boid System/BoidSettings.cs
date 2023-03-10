using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject
{
    [Header("General")]
    [SerializeField] public float maxSpeed = 1.5f;
    [SerializeField] public float minSpeed = 1.125f;
    [SerializeField] public float turnSpeed = 4.5f;
    [SerializeField] public float visualRange = 0.5f;
    [SerializeField] public float minDistance = 0.15f;

    [Header("Steering Weights")]
    [SerializeField] public float cohesionFactor = 1;
    [SerializeField] public float separationFactor = 30;
    [SerializeField] public float alignmentFactor = 5;

    [Header("Obstacle Avoidance")]
    [SerializeField] public float boundsRadius = 0.15f;
    [SerializeField] public float collisionAvoidDst = 7;
    [SerializeField] public float avoidCollisionFactor = 30;
    [SerializeField] public LayerMask obstacleMask;
}
