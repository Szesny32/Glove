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
    public Vector3 bias = new Vector3(0.014f, 0.072f, 0.00026f);
    public Vector3 noise = new Vector3(0.2f, 0.5f, 0.025f);
    




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
 
        Vector3 axis;
        float angle;
        deltaQ.ToAngleAxis(out angle, out axis);

        if (angle > 180f) angle -= 360f;  // Normalizuj kąt do przedziału [-180, 180]
        angle = Mathf.Deg2Rad * angle;     // Przekształć stopnie na radiany

        angularVelocity_gt = angularVelocity = axis * (angle / Time.deltaTime);  // Prędkość kątowa w radianach na sekundę

        //angularVelocity_gt = angularVelocity = deltaQ.eulerAngles * Mathf.Deg2Rad / Time.deltaTime;

        if(noisy){
            Vector3 normalNoise = new Vector3(
                RandomGaussian(0f, noise.x),
                RandomGaussian(0f, noise.y),
                RandomGaussian(0f, noise.z)
            );
            angularVelocity += normalNoise;
        }

        if(isBias){
            angularVelocity += bias;
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
