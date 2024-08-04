using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSim : MonoBehaviour
{
    public float rotationSpeedDegreesPerSecond = 30f; 
    private float timer = 5f;
    private float clock;


    public Vector3 active;

    void Start(){
        active = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
    }


    
    void Update()
    {
        clock += Time.deltaTime;
        if(clock > timer){
            active = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            clock %= timer;
            rotationSpeedDegreesPerSecond = Random.Range(-1.0f, 1.0f) *  90f;
        }  

        float f = (Time.time % timer) > timer/2f ? -1 : 1;
        float angleToRotate = f * rotationSpeedDegreesPerSecond * Time.deltaTime + Mathf.Sin(Time.time)/2;


        transform.Rotate(active, angleToRotate, Space.Self);

    }
}
