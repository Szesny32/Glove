using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class FilterAgent : Agent
{
    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log(actions.ContinuousActions[0]);
    }
}
