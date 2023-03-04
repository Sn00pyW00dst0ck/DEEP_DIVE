using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject
{
    [Header("General Settings")]
    public float minSpeed = 2;
    public float maxSpeed = 5;
    public float maxSteerForce = 3;
    public float perceptionRadius = 2.5f;
    public float avoidanceRadius = 1;

    [Header("Steering Weights")]
    public float alignWeight = 1;
    public float cohesionWeight = 1;
    public float separationWeight = 1;

    [Header("Collisions")]
    public LayerMask obstacleMask;
    public float boundsRadius = .27f;
    public float avoidCollisionWeight = 10;
    public float collisionAvoidDst = 5;

    [Header("Miscellaneous")]
    public float targetWeight = 1;
}
