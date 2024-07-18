using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Quaternion inverseRotation = Quaternion.Inverse(transform.rotation);
        // Vector3 accelerometer = inverseRotation * Vector3.up;
        // Quaternion q = transform.rotation;
        // accelerometer.x = 2f * (q.x*q.y + q.z*q.w);
        // accelerometer.y = -q.x*q.x + q.y*q.y -q.z*q.z + q.w*q.w;
        // accelerometer.z = 2f * (q.y*q.z - q.x*q.w);


        // Vector3 euler = new Vector3();
        // euler.x = - Mathf.Atan2(accelerometer.z, Mathf.Sqrt(Mathf.Pow(accelerometer.x, 2) + Mathf.Pow(accelerometer.y, 2))) * Mathf.Rad2Deg;  
        // euler.z = Mathf.Atan2 (accelerometer.x, accelerometer.y) * Mathf.Rad2Deg;

       // Debug.Log($"{q.y*q.y + q.w*q.w  + q.x*q.x + q.z*q.z  }");


        Vector3 up = transform.up.normalized;
        Debug.Log($"{transform.rotation} : {new Quaternion(up.x, up.y, up.z, 0)}");


    }



}
