using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ComplementaryType{
    NORMAL,
    SLEPR
}
public class Complementary  : AttitudeEstimator{

    [Range(0.0f, 1.0f)]
    public float alpha = 0.98f;
    public ComplementaryType type;


    public override void Init(){
        transform.rotation = reference.rotation;
    }
    public override void UpdateOrientation(){

        Quaternion q1 = _AngularRate();
        Quaternion q2 = _eCompass();

        if(type == ComplementaryType.SLEPR){
            transform.rotation = Quaternion.Slerp(q1, q2, 1-alpha);
        } 
        else if(type == ComplementaryType.NORMAL){
            transform.rotation = new Quaternion(
                alpha*q1.x + (1-alpha)*q2.x,
                alpha*q1.y + (1-alpha)*q2.y,
                alpha*q1.z + (1-alpha)*q2.z,
                alpha*q1.w + (1-alpha)*q2.w
            );
        } 
    }

    private Quaternion _AngularRate(){
        Vector3 angularRate = angularVelocity * Time.deltaTime;
        Quaternion dQ = Quaternion.AngleAxis(angularRate.magnitude * Mathf.Rad2Deg, angularVelocity.normalized);
        Quaternion q = transform.rotation * dQ;
        return q; 
    }

    private Quaternion _eCompass(){
        Vector3 euler = Vector3.zero;
        euler.x = -Mathf.Atan2(acceleration.z, Mathf.Sqrt(Mathf.Pow(acceleration.x, 2) + Mathf.Pow(acceleration.y, 2)));
        euler.y = 0f;
        euler.z = Mathf.Atan2 (acceleration.x, acceleration.y);

        Vector3 v = xRot(euler.x, zRot(euler.z, magneticField));
        euler.y = -Mathf.Atan2(v.x, v.z); 

        float cx = Mathf.Cos(euler.x * 0.5f);
        float cy = Mathf.Cos(euler.y * 0.5f);
        float cz = Mathf.Cos(euler.z * 0.5f);
        float sx = Mathf.Sin(euler.x * 0.5f);
        float sy = Mathf.Sin(euler.y * 0.5f);
        float sz = Mathf.Sin(euler.z * 0.5f);

        Quaternion q = new Quaternion(
            sx * cy * cz + cx * sy * sz,
            cx * sy * cz - sx * cy * sz, 
            cx * cy * sz - sx * sy * cz, 
            cx * cy * cz + sx * sy * sz  
        ); 
        return q;
    }





}
