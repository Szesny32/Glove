using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public Quaternion quaternion;
    public Quaternion inverseQuaternion;
    public Vector3 accelerometer;
    public Quaternion test;
    
    void Start()
    {
        quaternion = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = quaternion;
        inverseQuaternion = Quaternion.Inverse(quaternion);
        accelerometer = inverseQuaternion * Vector3.up;
        test = inverseQuaternion * new Quaternion(0,1,0,0);

    }
}
