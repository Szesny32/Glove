using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngularRate : AttitudeEstimator
{
    public override void UpdateOrientation(){
        Vector3 angularRate = angularVelocity * Time.deltaTime;
        Quaternion dQ = Quaternion.AngleAxis(angularRate.magnitude * Mathf.Rad2Deg, angularVelocity.normalized);
        transform.rotation *= dQ; 
    }
}
