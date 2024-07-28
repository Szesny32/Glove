using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSim : MonoBehaviour
{
    public float rotationSpeedDegreesPerSecond = 90f; 
    private float timer = 5f;
    void Update()
    {
        float f = (Time.time % timer) > timer/2f ? -1 : 1;
        float angleToRotate = f * rotationSpeedDegreesPerSecond * Time.deltaTime + Mathf.Sin(Time.time)/2;


        if(Time.time % 10 > 5)
            transform.Rotate(Vector3.forward, angleToRotate);
        else 
            transform.Rotate(Vector3.up, angleToRotate);
    }
}
