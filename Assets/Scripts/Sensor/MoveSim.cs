using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSim : MonoBehaviour
{
    public float rotationSpeedDegreesPerSecond = 30f; 

    void Update()
    {
        float angleToRotate = rotationSpeedDegreesPerSecond * Time.deltaTime;
        transform.Rotate(Vector3.forward, angleToRotate);
    }
}
