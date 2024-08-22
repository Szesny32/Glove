using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AIEstimator : Agent
{

   public float rotationSpeedDegreesPerSecond = 30f; 
    private float timer = 1f;
    private float clock;


    public Vector3 active = Vector3.up;
    float angleToRotate = 0f;

    public Vector3 AccNoise = new Vector3(0.12f, 0.21f, 0.28f);
    float g = 9.8067f;

    private const float regionalField = 50.06349f; //μT //1f WORKS
    private const float inclination = (67.73f) * Mathf.Deg2Rad;

    private readonly Vector3 r = new Vector3(
        0f,
        -Mathf.Sin(inclination),
        Mathf.Cos(inclination)
    ) * regionalField; 

    public Vector3 MagNoise = new Vector3(2f, 1f, 1.5f); //variance

    


    Vector3 acceleration;
    Vector3 magneticPole;
    float dt = 0f;


    public override void Initialize(){
        
    }

    private void _UpdateSensors(){
        dt = 0.01f;
        clock += dt;
        if(clock > timer){
            active = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            clock %= timer;
            rotationSpeedDegreesPerSecond = Random.Range(-1.0f, 1.0f) *  90f;
            angleToRotate = rotationSpeedDegreesPerSecond * dt * Mathf.Sin(Time.time);
        }  
        transform.Rotate(active, angleToRotate, Space.Self);

        Quaternion q = transform.rotation;
        acceleration = new Vector3(
            2f*(q.x*q.y + q.w*q.z), 
            -q.x*q.x + q.y*q.y - q.z*q.z + q.w*q.w,
            2f*(q.y*q.z - q.x*q.w)
        ) * g;
        //acceleration_gt = acceleration = zRot(-q.eulerAngles.x ,xRot(-q.eulerAngles.x, yRot(-q.eulerAngles.y, Vector3.up))) * g;
       string log = $"q: {q} \n\n\acceleration: {acceleration}";
        if (log.Contains("NaN"))
            Debug.Log($"log: {log}");

  
        Vector3 normalNoise = new Vector3(
            Noise.Generate(AccNoise.x),
            Noise.Generate(AccNoise.y),
            Noise.Generate(AccNoise.z)
        );
        acceleration += normalNoise;

        float qx2 = q.x * q.x;
        float qy2 = q.y * q.y;
        float qz2 = q.z * q.z;

        float qwqx = q.w * q.x;
        float qwqy = q.w * q.y;
        float qwqz = q.w * q.z;
        float qxqy = q.x * q.y;
        float qxqz = q.x * q.z;
        float qyqz = q.y * q.z;

        magneticPole = new Vector3(
            r.x*(0.5f - qy2 - qz2) + r.y*(qwqz + qxqy) + r.z*(qxqz - qwqy),
            r.x*(qxqy - qwqz) + r.y*(0.5f - qx2 - qz2) + r.z*(qwqx + qyqz),
            r.x*(qwqy + qxqz) + r.y*(qyqz - qwqx) + r.z*(0.5f - qx2 - qy2)
        ) * 2f;

        normalNoise = new Vector3(
            Noise.Generate(MagNoise.x),
            Noise.Generate(MagNoise.y),
            Noise.Generate(MagNoise.z)
        );
        magneticPole += normalNoise;
    }  


    
    


   public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        //for(int i = 0; i < 3; i++){
            continuousActions[0] = 0f;
            continuousActions[1] = 0f;
            continuousActions[2] = 0f;
            //continuousActions[3] = 1f;
        // }
    }

     public override void OnActionReceived(ActionBuffers actions)
    {

        float x = actions.ContinuousActions[0] * 180f;
        float y = actions.ContinuousActions[1] * 180f;
        float z = actions.ContinuousActions[2] * 180f;
        //float w = actions.ContinuousActions[3];
        //transform.rotation = new Quaternion(x, y, z, w);
        Quaternion true_state = transform.rotation;
        Quaternion estimated_state  = Quaternion.Euler(x, y, z);
        
        float angleDiff = Quaternion.Angle(true_state, estimated_state);
        float reward = Mathf.Pow((10f - Mathf.Clamp(angleDiff, 0f, 10f)), 3) / 1000f;
        SetReward(reward);
    }

    public override void CollectObservations(VectorSensor sensor){
        _UpdateSensors();
        sensor.AddObservation(acceleration.normalized);
        sensor.AddObservation(magneticPole.normalized);
        sensor.AddObservation(AccNoise.normalized);
        sensor.AddObservation(MagNoise.normalized);
    }

    
}