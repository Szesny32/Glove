using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSim : MonoBehaviour
{
    public float rotationSpeedDegreesPerSecond = 90f; 
    private float timer = 5f;
    private int state = 0;

    private Vector3[] dir = {Vector3.up, Vector3.right, Vector3.forward};

    private float clock;
    void Update()
    {
        clock += Time.deltaTime;
        if(clock > timer){
            state = (state + 1) % 3; 
            clock %= timer;
        }  

        float f = (Time.time % timer) > timer/2f ? -1 : 1;
        float angleToRotate = f * rotationSpeedDegreesPerSecond * Time.deltaTime + Mathf.Sin(Time.time)/2;


        transform.Rotate(dir[state], angleToRotate);

    }
}
