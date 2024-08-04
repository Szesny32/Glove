using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XXX : AttitudeEstimator
{

     private List<Quaternion> quaternionList = new List<Quaternion>();
private float alpha = 0.98f;
    public override void UpdateOrientation(){

        
        Quaternion q2 = _eCompass();
        

        //transform.rotation = q;
        q2 = Quaternion.Slerp(q2, GetWeightMean(), 0.1f);

        Quaternion q1 = _AngularRate();

        Quaternion q3 = Quaternion.Slerp(q1, q2, 1-alpha);

        transform.rotation = q3;



        AddQuaternion(q2);


    }


    public void AddQuaternion(Quaternion newQuaternion)
    {
        quaternionList.Add(newQuaternion);

        if (quaternionList.Count > 100)
            quaternionList.RemoveAt(0);
    }

    private Quaternion _AngularRate(){
        Vector3 angularRate = angularVelocity * Time.deltaTime;
        Quaternion dQ = Quaternion.AngleAxis(angularRate.magnitude * Mathf.Rad2Deg, angularVelocity.normalized);
        Quaternion q = transform.rotation * dQ;
        return q; 
    }


    private Quaternion GetWeightMean(){
        Quaternion mean = new Quaternion(0, 0, 0, 1);

        float weight = 1;
        for(int i = quaternionList.Count; i>0; i--){
        
            mean = Quaternion.Slerp(mean,quaternionList[i-1], weight);
            weight*=0.1f;        
        }
        return mean;
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
