using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMUOld: MonoBehaviour
{
    [SerializeField] private AccelerometerOld accelerometer;
    [SerializeField] private MagnetometerOld magnetometer;
    public Vector3 eulerAngles; 

    public bool noise = false;

    void Update(){
        //EstimateXZ(noise? accelerometer.Read2() : accelerometer.Read());
        //EsimateY(magnetometer.Read());
        //transform.rotation = Quaternion.Euler(eulerAngles);


    }

    void EstimateXZ(Vector3 acc){
        eulerAngles.x = ( -Mathf.Atan2(acc.z, Mathf.Sqrt(Mathf.Pow(acc.x, 2) + Mathf.Pow(acc.y, 2))) * Mathf.Rad2Deg + 360f) % 360f;  
        eulerAngles.z = (Mathf.Atan2 (acc.x, acc.y) * Mathf.Rad2Deg+ 360f) % 360f;
    }

    void EsimateY(Vector3 mag){
        Vector3 v = xRot(eulerAngles.x, zRot(eulerAngles.z, magnetometer.GetOrigin()));
        float y = v.x * mag.z - v.z * mag.x;
        float x = v.x * mag.x + v.z * mag.z;
        eulerAngles.y = (-Mathf.Atan2(y, x) * Mathf.Rad2Deg + 360f) % 360f; //y was down
    }

    Vector3 zRot(float angle, Vector3 v){
        float radAngle = angle * Mathf.Deg2Rad;
        float sinZ = Mathf.Sin(radAngle);
        float cosZ = Mathf.Cos(radAngle);
        return new Vector3(
            (cosZ * v.x) + (-sinZ * v.y),
            (sinZ * v.x) + (cosZ * v.y),
            v.z
        );
    }

    Vector3 xRot(float angle, Vector3 v){
        float radAngle = angle * Mathf.Deg2Rad;
        float sinX = Mathf.Sin(radAngle);
        float cosX = Mathf.Cos(radAngle);
        return new Vector3(
            v.x,
            (cosX * v.y) + (-sinX * v.z),
            (sinX * v.y) + (cosX * v.z)
        );
    }

    Vector3 yRot(float angle, Vector3 v){
        float radAngle = angle * Mathf.Deg2Rad;
        float sinY = Mathf.Sin(radAngle);
        float cosY = Mathf.Cos(radAngle);
        return new Vector3(
            (cosY * v.x) + (sinY * v.z),
            v.y,
            (-sinY * v.x) + (cosY * v.z)
        );
    }

}