using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisAngleToEuler : MonoBehaviour
{
    
    public Transform source;
   // public bool customMode = true;

    void Start() {}

    void Update() {
        Conversion();
    }

    void Conversion(){
        float angle = 0.0f;
        Vector3 axis = Vector3.zero;
        source.rotation.ToAngleAxis(out angle, out axis);
        float ax = axis.x;
        float ay = axis.y;
        float az = axis.z;


        float halfRadAngle = angle * Mathf.Deg2Rad * 0.5f;
        Vector3 euler = Vector3.zero;
        euler.x = Mathf.Asin(2f * Mathf.Sin(halfRadAngle) * (ax * Mathf.Cos(halfRadAngle) - ay*az*Mathf.Sin(halfRadAngle)));
        euler.y = Mathf.Atan2(2f * Mathf.Sin(halfRadAngle) * (ay * Mathf.Cos(halfRadAngle)  + ax*az*Mathf.Sin(halfRadAngle)), 1- 2f *(Mathf.Sin(halfRadAngle) * Mathf.Sin(halfRadAngle) * (ay*ay+ax*ax)));
        euler.z = Mathf.Atan2(2f * Mathf.Sin(halfRadAngle) * (az * Mathf.Cos(halfRadAngle)  + ax*ay*Mathf.Sin(halfRadAngle)), 1- 2f *(Mathf.Sin(halfRadAngle) * Mathf.Sin(halfRadAngle) * (az*az+ax*ax)));
        euler*=Mathf.Rad2Deg;
        //if(customMode){

        //} else {

         //   euler = Quaternion.AxisAngle(axis, angle).eulerAngles;
        //}

        transform.rotation = Quaternion.Euler(euler);
    }
}
