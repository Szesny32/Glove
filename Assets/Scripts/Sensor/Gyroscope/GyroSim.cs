using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GyroSim : MonoBehaviour
{

    private Vector3 angularVelocity;
    private Vector3 angularVelocity_gt;
    private Quaternion qPrev;
    [SerializeField]
    private TMP_Text UI;

    public bool noisy = false;
    public bool isBias = false;
    private Vector3 bias = new Vector3(0.0005f, 0.0005f, 0.00005f);
    public Vector3 noise = new Vector3(0.00025f, 0.00025f, 0.000025f);
    




    void Start(){

        qPrev  = transform.rotation;
        angularVelocity_gt = angularVelocity = Vector3.zero;
    }

    void Update(){
        Measurement();
    }

    private void Measurement(){

        Quaternion qCurrent = transform.rotation;
        Quaternion qPrevInverse = Quaternion.Inverse(qPrev);
        Quaternion deltaQ = qCurrent * qPrevInverse;
        Vector3 eulerDelta =  deltaQ.eulerAngles;


        angularVelocity_gt = angularVelocity = eulerDelta * Mathf.Deg2Rad / Time.deltaTime;

        if(noisy){
            Vector3 normalNoise = new Vector3(
                RandomGaussian(0f, noise.x),
                RandomGaussian(0f, noise.y),
                RandomGaussian(0f, noise.z)
            );
            angularVelocity += (normalNoise / Time.deltaTime);
        }

        if(isBias){
            angularVelocity += (bias / Time.deltaTime);
        }

        qPrev  = transform.rotation;
        UI.text = $"Gyroscope: {angularVelocity} [rad/s]";
    }


    public Vector3 GetBias(){
        return isBias? bias : Vector3.zero;
    }

    public Vector3 GetNoise(){
        return noisy? noise : Vector3.zero;
    }

    public Vector3 Read(){
        return angularVelocity;
    }

    public Vector3 GetGroundTrue(){
        return angularVelocity_gt;
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
