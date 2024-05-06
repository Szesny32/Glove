using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnetometer : MonoBehaviour
{
    private Vector3 data;
    private const float inclination = (66f + 2f/3f) * Mathf.Deg2Rad;
    private const float magneticFieldStrength = 49822.3f;
    private readonly Vector3 origin = new Vector3(
            0f,
            magneticFieldStrength * Mathf.Sin(inclination),
            magneticFieldStrength * Mathf.Cos(inclination)
    ); 

    void Update(){
        //data = transform.rotation * origin;
        data = yRot(transform.rotation.eulerAngles.y, xRot(transform.rotation.eulerAngles.x, zRot(transform.rotation.eulerAngles.z, origin)));
    }

    public Vector3 Read(){
        return data;
    }

    public Vector3 GetOrigin(){
        return origin;
    }

    //Obróciłem rotacje z powrotem -> todo zobaczyć
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