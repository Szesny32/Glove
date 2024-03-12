using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EstimationStrategy {
    None,
    GyroscopeMethod,
    AccelerometerMethod
}

public interface IEstimationStrategy {
    void Estimate();
}

public class NoneMethod : IEstimationStrategy  {
    public void Estimate() {
        Debug.Log("NoneMethod");
    }
}

public class GyroscopeMethod : IEstimationStrategy {
    public void Estimate() {
        Debug.Log("GyroscopeMethod");
    }
}

public class AccelerometerMethod : IEstimationStrategy {
    public void Estimate() {
        Debug.Log("AccelerometerMethod");
    }
}

public static class EstimationExecutor {
    private static Dictionary<EstimationStrategy, IEstimationStrategy> strategies = new Dictionary<EstimationStrategy, IEstimationStrategy>();
    private static IEstimationStrategy noneMethod = new NoneMethod();

    static EstimationExecutor() {
        strategies.Add(EstimationStrategy.None, noneMethod);
        strategies.Add(EstimationStrategy.GyroscopeMethod, new GyroscopeMethod());
        strategies.Add(EstimationStrategy.AccelerometerMethod, new AccelerometerMethod());
    }

    public static IEstimationStrategy GetStrategy(EstimationStrategy mode) {
        if (strategies.TryGetValue(mode, out IEstimationStrategy strategy)) {
            return strategy;
        } else {
            Debug.LogError("No strategy for the mode: " + mode);
            return noneMethod;
        }
    }
}