using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetOld : MonoBehaviour
{
    public SensorOld sensor;
    void Start()
    {
        
    }

    Vector3 accelTilt = Vector3.zero;

    // Update is called once per frame
    void Update()
    {


        accelTilt.x = - Mathf.Atan2(sensor.AccGravity.z, Mathf.Sqrt(Mathf.Pow(sensor.AccGravity.x, 2) + Mathf.Pow(sensor.AccGravity.y, 2))) * Mathf.Rad2Deg;  
            
        //Yaw
        accelTilt.y = sensor.transform.rotation.eulerAngles.y;
        
        //Roll
        //todo: Are "-" redundant? 
        accelTilt.z = - Mathf.Atan2(- sensor.AccGravity.x, sensor.AccGravity.y) * Mathf.Rad2Deg;  
        accelTilt = normalizeDegVector(accelTilt);

        transform.rotation = Quaternion.Euler(accelTilt);
}


    float normalizeDegAngle(float angle){
        return (angle + 360f) % 360f;
    }

    Vector3 normalizeDegVector(Vector3 angle){
        angle.x = normalizeDegAngle(angle.x);
        angle.y = normalizeDegAngle(angle.y);
        angle.z = normalizeDegAngle(angle.z);
        return angle;
    }

}