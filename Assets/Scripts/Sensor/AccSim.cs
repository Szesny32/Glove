using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AccSim : MonoBehaviour
{
    //todo: add motion acceleration 
    float g = 9.8067f;
    Vector3 acceleration;
    Vector3 acceleration_gt;

    [SerializeField]
    private TMP_Text UI;

    public bool noisy = false;
    public bool isBias = false;
    public Vector3 bias = new Vector3(0.15f, 0.15f, -0.25f);
    public Vector3 noise = new Vector3(0.12f, 0.21f, 0.28f); //variance
    
    

    void Start()
    {
        acceleration_gt = acceleration = Vector3.zero;
    }

    void Update()
    {

        Quaternion q = transform.rotation;
        acceleration_gt = acceleration = new Vector3(
            2f*(q.x*q.y + q.w*q.z), 
            -q.x*q.x + q.y*q.y - q.z*q.z + q.w*q.w,
            2f*(q.y*q.z - q.x*q.w)
        ) * g;
        //acceleration_gt = acceleration = zRot(-q.eulerAngles.x ,xRot(-q.eulerAngles.x, yRot(-q.eulerAngles.y, Vector3.up))) * g;
       string log = $"q: {q} \n\n\nacceleration_gt: {acceleration_gt}";
        if (log.Contains("NaN"))
            Debug.Log($"log: {log}");



        if(noisy){
            Vector3 normalNoise = new Vector3(
                Noise.Generate(noise.x),
                Noise.Generate(noise.y),
                Noise.Generate(noise.z)
            );
            acceleration += normalNoise;
        }
        if(isBias){
            acceleration += bias;
        }
        if(UI != null)
            UI.text = $"Accelerometer: {acceleration} [m/s²]";
    }

    public Vector3 Read(){
        return acceleration;
    }

     public Vector3 GetGroundTrue(){
        return acceleration_gt;
    }

    public Vector3 GetBias(){
        return isBias? bias : Vector3.zero;
    }

    public Vector3 GetNoise(){
        return noisy? noise : Vector3.zero;
    }


}
