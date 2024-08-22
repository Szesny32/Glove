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
    public ComplementaryType type = ComplementaryType.SLEPR;

    Quaternion state;  

    string log = "";
    string prevLog = "";

    public override void Init(){
        state = transform.rotation = reference.rotation;
    }

    
    public override Quaternion GetState(){
        return state;
    }


    public override void UpdateOrientation(){
        log = $"{transform.name}\n";
        Quaternion q1 = _AngularRate();
        log += $"q1: {q1}\n\n";

        Quaternion q2 = _eCompass();
        log += $"q2: {q2}\n\n";


        if(type == ComplementaryType.SLEPR){
            state = Quaternion.Slerp(q1, q2, 1-alpha);
            log += $"state: {state}\n\n";
        } 
        else if(type == ComplementaryType.NORMAL){
             state = new Quaternion(
                alpha*q1.x + (1-alpha)*q2.x,
                alpha*q1.y + (1-alpha)*q2.y,
                alpha*q1.z + (1-alpha)*q2.z,
                alpha*q1.w + (1-alpha)*q2.w
            );
        }


        transform.rotation = state; 
        if (log.Contains("NaN"))
            Debug.Log($"prevLog: {prevLog} \n\n\nlog: {log} ");

        prevLog = log;
    }

    private Quaternion _AngularRate(){
        log += $"_AngularRate\n";
        Vector3 angularRate = angularVelocity * Time.deltaTime;
        log += $"angularRate: {angularRate}\n";
        Quaternion dQ = Quaternion.AngleAxis(angularRate.magnitude * Mathf.Rad2Deg, angularVelocity.normalized);
        log += $"dQ: {dQ}\n";
        Quaternion q = state * dQ;
        return q; 
    }

    private Quaternion _eCompass(){
        Vector3 euler = Vector3.zero;
        log += $"_eCompass\n";

        log += $"acceleration: {acceleration}\n";

        euler.x = -Mathf.Atan2(acceleration.z, Mathf.Sqrt(Mathf.Pow(acceleration.x, 2) + Mathf.Pow(acceleration.y, 2)));
        euler.y = 0f;
        euler.z = Mathf.Atan2 (acceleration.x, acceleration.y);

        Vector3 v = xRot(euler.x, zRot(euler.z, magneticField));

        log += $"v: {v}\n";
        euler.y = -Mathf.Atan2(v.x, v.z); 

        log += $"euler: {euler}\n";

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
