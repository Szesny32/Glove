using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisAngleToEuler : MonoBehaviour
{
    
    public Transform source;
    public bool customMode = true;

    void Start() {}

    void Update() {
        Conversion();
    }

    void Conversion(){
        float angle = 0.0f;
        Vector3 axis = Vector3.zero;
        source.rotation.ToAxisAngle(out axis, out angle);
        float x = axis.x;
        float y = axis.y;
        float z = axis.z;


        float radAngle = angle * Mathf.Deg2Rad;
        Vector3 euler = Vector3.zero;
        if(customMode){

        } else {

            euler = Quaternion.AxisAngle(axis, angle).eulerAngles;
        }

        transform.rotation = Quaternion.Euler(euler);
    }
}
