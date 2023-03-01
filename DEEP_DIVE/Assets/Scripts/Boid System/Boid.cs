using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    #region State Variables

    BoidSettings settings;

    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    Vector3 velocity;

    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;

    // Cached Unity References For faster lookup times
    Material material;
    Transform cachedTransform;

    #endregion State Variables

    private void Awake()
    {
        material = transform.GetComponentInChildren<MeshRenderer>().material;
        cachedTransform = transform;
    }

    public void Initialize(BoidSettings settings)
    {
        // Set the target and settings
        this.settings = settings;

        // Some initial position, forward, and velocity
        position = cachedTransform.position;
        forward = cachedTransform.forward;
        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }

    public void UpdateBoid()
    {
        Vector3 acceleration = Vector3.zero;

        // TODO: Follow target functions will go here

        // Apply boid rules if there are flockmates
        if (numPerceivedFlockmates > 0)
        {
            centreOfFlockmates /= numPerceivedFlockmates;

            // Apply three force rules, one for alignment, one for cohesion, and one for separation
            acceleration += SteerTowards(avgFlockHeading) * settings.alignWeight;
            acceleration += SteerTowards(centreOfFlockmates - position) * settings.cohesionWeight;
            acceleration += SteerTowards(avgAvoidanceHeading) * settings.separationWeight;
        }

        // If there is an obstacle, get a direction to avoid the obstacle, then steer that way
        if (IsHeadingForCollision())
        {
            acceleration += SteerTowards(GetNotCollidingDirection()) * settings.avoidCollisionWeight;
        }

        // Calculate new velocity
        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;

        // Update the transform and vectors
        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        forward = dir;
    }

    #region TODO: Follow Target Functions

    Vector3 GetTargetDirection()
    {
        return Vector3.zero;
    }

    #endregion TODO: Follow Target Functions

    #region Collision Functions

    bool IsHeadingForCollision()
    {
        return Physics.SphereCast(position, settings.boundsRadius, forward, out _, settings.collisionAvoidDst, settings.obstacleMask);
    }

    Vector3 GetNotCollidingDirection()
    {
        Vector3[] rayDirections = DirectionVectorGenerator.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask))  {
                return dir;
            }
        }
        return forward;
    }

    #endregion Collision Functions

    #region Utility Functions

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, settings.maxSteerForce);
    }

    public void SetColor(Color _color)
    {
        if (material != null)
        {
            material.color = _color;
        }
    }

    #endregion Utility Functions

}
