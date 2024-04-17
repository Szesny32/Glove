using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    public Sensor sensor;
    public float yaw;
    private const float sensitivity = 10f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EstimateOrientation();
        Debug.DrawRay(transform.position, transform.up, Color.green);
        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        Debug.DrawRay(transform.position, transform.right, Color.red);
    }


    void EstimateOrientation(){
        if(sensor){
             //Vector3 accelerometerData = sensor.GetAccelerometer();
            // yaw = sensor.TEST_GetYawRotation();

            // Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);
            // Quaternion accelerometerRotation = Quaternion.FromToRotation(Vector3.up, accelerometerData.normalized);
            // Quaternion finalRotation = yawRotation * Quaternion.Inverse(accelerometerRotation);
            // transform.rotation = finalRotation;

            //Vector3 upVector = Vector3.up;

            // Obliczamy osie obrotu jako iloczyny wektorowe wektorów wejściowych.
            //Vector3 rotationAxis = Vector3.Cross(upVector, accelerometerData);
           // float rotationAngle = Mathf.Acos(Vector3.Dot(upVector, rotationAxis));
           // transform.rotation = Quaternion.AngleAxis(rotationAngle * Mathf.Rad2Deg, rotationAxis);

        }
    }
}
