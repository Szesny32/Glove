using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSim : MonoBehaviour
{
    public float rotationSpeedDegreesPerSecond = 90f; 
    private float timer = 5f;
    private float clock;
    public int state = 0;

    private Vector3[] direction = {Vector3.forward, Vector3.right, Vector3.up};   
    public Vector3 active;

    void Start(){
        active = direction[state];
    }


    
    void Update()
    {
        clock += Time.deltaTime;
        if(clock > timer){
            state = (state + 1) % direction.Length; 
            active = direction[state];
            clock %= timer;
        }  

        float f = (Time.time % timer) > timer/2f ? -1 : 1;
        float angleToRotate = f * rotationSpeedDegreesPerSecond * Time.deltaTime + Mathf.Sin(Time.time)/2;


        transform.Rotate(active, angleToRotate, Space.Self);

    }
}
