using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMU : MonoBehaviour
{
    IEstimationStrategy estimationStrategy;
    
    void Start() {
        estimationStrategy = EstimationExecutor.GetStrategy(EstimationStrategy.GyroscopeMethod);
    }

    void Update() {
        estimationStrategy.Estimate();
    }
}