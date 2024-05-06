using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerometer : MonoBehaviour
{
    private Vector3 data;

    void Update(){
        Quaternion q = transform.rotation;
        data = new Vector3(
            2f*(q.x*q.y + q.w*q.z), 
            -q.x*q.x + q.y*q.y - q.z*q.z + q.w*q.w,
            2f*(q.y*q.z - q.x*q.w)
        );   
    }

    public Vector3 Read(){
        return data;
    }
}