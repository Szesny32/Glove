using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Noise {
 
    private  static readonly System.Random _random = new System.Random();

    public static float Generate(float variance){
        return NormalDistribution(variance);
    }

    private static float NormalDistribution (float variance) {
        // Box-Muller method for generating normal distribution
        float u1 = 1.0f - (float)_random.NextDouble(); // U(0,1) 
        float u2 = 1.0f - (float)_random.NextDouble(); // U(0,1)
        float z1 = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2); // Z(0,1)
        float standardDeviation = Mathf.Sqrt(variance);
        return z1 * standardDeviation;
    }
}
