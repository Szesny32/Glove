using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GyroSim : MonoBehaviour{
    private Vector3 angularVelocity;
    private Vector3 angularVelocity_gt;
    private Quaternion qPrev;
    [SerializeField]
    private TMP_Text UI;
    public bool noisy = false;
    public bool isBias = false;
    public Vector3 bias = new Vector3(0.014f, 0.072f, 0.00026f);
    public Vector3 noise = new Vector3(0.2f, 0.5f, 0.025f); //variance
    
    void Start(){
        qPrev  = transform.rotation;
        angularVelocity_gt = angularVelocity = Vector3.zero;
    }

    void Update(){
        Measurement();
    }

    private void Measurement(){

        Quaternion qCurrent = transform.rotation;
        Quaternion deltaQ = qCurrent * Quaternion.Inverse(qPrev);
 
        Vector3 axis;
        float angle;
        deltaQ.ToAngleAxis(out angle, out axis);

        if (angle > 180f) angle -= 360f;  // Normalizuj kąt do przedziału [-180, 180]
        angle = Mathf.Deg2Rad * angle;     // Przekształć stopnie na radiany

        axis = transform.InverseTransformDirection(axis); // SENSOR IS IN LOCAL FRAME
        angularVelocity_gt = angularVelocity = axis * (angle / Time.deltaTime);  // Prędkość kątowa w radianach na sekundę
  
        if(noisy){
            Vector3 normalNoise = new Vector3(
                Noise.Generate(noise.x),
                Noise.Generate(noise.y),
                Noise.Generate(noise.z)
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

}
