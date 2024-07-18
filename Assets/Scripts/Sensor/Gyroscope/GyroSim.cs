using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GyroSim : MonoBehaviour
{

    private Vector3 angularVelocity;
    private Vector3 angularVelocity_gt;
    private Quaternion previousRotation;
    [SerializeField]
    private TMP_Text UI;

    public bool noisy = false;
    Vector3 bias = new Vector3(0.0005f, 0.0005f, 0.00005f);
    Vector3 noise = new Vector3(0.00025f, 0.00025f, 0.000025f);
    

    void Start(){
        previousRotation = transform.rotation;
        angularVelocity_gt = angularVelocity = Vector3.zero;
    }

    void Update(){
        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(previousRotation);
        Vector3 deltaEuler = deltaRotation.eulerAngles;
        if (deltaEuler.x > 180) deltaEuler.x -= 360;
        if (deltaEuler.y > 180) deltaEuler.y -= 360;
        if (deltaEuler.z > 180) deltaEuler.z -= 360;
        angularVelocity_gt = angularVelocity = deltaEuler * Mathf.Deg2Rad / Time.deltaTime;

        if(noisy){
            Vector3 normalNoise = new Vector3(
                RandomGaussian(0f, noise.x),
                RandomGaussian(0f, noise.y),
                RandomGaussian(0f, noise.z)
            );
            angularVelocity += ((bias + normalNoise) / Time.deltaTime);
        }

        previousRotation = transform.rotation;
        UI.text = $"Gyroscope: {angularVelocity} [rad/s]";
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
