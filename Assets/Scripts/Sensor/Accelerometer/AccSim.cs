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
    Vector3 bias = new Vector3(0.15f, 0.15f, -0.25f);
    public Vector3 noise = new Vector3(0.25f, 0.25f, 0.25f);
    
    

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

        if(noisy){
            Vector3 normalNoise = new Vector3(
                RandomGaussian(0f, noise.x),
                RandomGaussian(0f, noise.y),
                RandomGaussian(0f, noise.z)
            );
            acceleration += normalNoise;
        }
        if(isBias){
            acceleration += bias;
        }

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



    public static float RandomGaussian(float mean = 0.0f, float sigma = 1.0f) {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);
        return Mathf.Clamp(std * sigma + mean, mean - sigma, mean + sigma);
    }

}
