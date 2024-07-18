using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerometerOld : MonoBehaviour
{
    private Vector3 data;
    public float fullScaleRange = 2f;
    public float RMS = 0.008f; 
    private Vector3 data2;




    void Update(){
        Quaternion q = transform.rotation;
        data = new Vector3(
            2f*(q.x*q.y + q.w*q.z), 
            -q.x*q.x + q.y*q.y - q.z*q.z + q.w*q.w,
            2f*(q.y*q.z - q.x*q.w)
        );   


        float xOffset = (Random.Range(0, 2) == 0 ? RMS : -RMS)  + Random.Range(-RMS, RMS);
        float yOffset = (Random.Range(0, 2) == 0 ? RMS : -RMS)  + Random.Range(-RMS, RMS);
        float zOffset = (Random.Range(0, 2) == 0 ? RMS : -RMS)  + Random.Range(-RMS, RMS);
        data2 = data + new Vector3( xOffset, yOffset, zOffset);
    }


    public Vector3 Read(){
        return data;
    }

    public Vector3 Read2(){
        return data2;
    }


}