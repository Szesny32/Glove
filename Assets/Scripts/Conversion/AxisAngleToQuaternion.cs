using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisAngleToQuaternion : MonoBehaviour
{

    public Transform source;
    //public bool customMode = true;

    void Start()
    {
        
    }


    void Update()
    {
        Conversion();
    }

    public void Conversion(){

        float angle = 0.0f;
        Vector3 axis = Vector3.zero;
        source.rotation.ToAngleAxis(out angle, out axis);
        Quaternion quaternion = new Quaternion();

        //if(customMode){
            axis.Normalize();

            float halfAngle = angle * Mathf.Deg2Rad* 0.5f;
            float sinHalfAngle = Mathf.Sin(halfAngle);
            float cosHalfAngle = Mathf.Cos(halfAngle);

            quaternion = new Quaternion(
                axis.x * sinHalfAngle,
                axis.y * sinHalfAngle,
                axis.z * sinHalfAngle,
                cosHalfAngle
            );

        //} else {
        //    quaternion = Quaternion.AngleAxis(angle, axis);
        //}

        transform.rotation = quaternion;

    }
}
