using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DirectionVectorGenerator 
{
    public static readonly int numDirections = 300;
    public static readonly Vector3[] directions;

    static DirectionVectorGenerator()
    {
        directions = new Vector3[DirectionVectorGenerator.numDirections];

        // Below is based on this paper, with some modifications to have points generate in a different order
        // https://www.cmu.edu/biolphys/deserno/pdf/sphere_equi.pdf

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numDirections; i++)
        {
            float t = (float)i / numDirections;
            float theta = Mathf.Acos(1 - 2 * t);
            float phi = angleIncrement * i;

            directions[i] = new Vector3(
                Mathf.Sin(theta) * Mathf.Cos(phi),
                Mathf.Sin(theta) * Mathf.Sin(phi),
                Mathf.Cos(theta));
        }
    }
}
