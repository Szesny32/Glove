using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eCompass : AttitudeEstimator{
  public override void UpdateOrientation(){
    Vector3 euler = Vector3.zero;
    euler.x = -Mathf.Atan2(acceleration.z, Mathf.Sqrt(Mathf.Pow(acceleration.x, 2) + Mathf.Pow(acceleration.y, 2)));
    euler.y = 0f;
    euler.z = Mathf.Atan2 (acceleration.x, acceleration.y);

    Vector3 v = xRot(euler.x, zRot(euler.z, magneticField));
    euler.y = -Mathf.Atan2(v.x, v.z); 

    float cx = Mathf.Cos(euler.x * 0.5f);
    float cy = Mathf.Cos(euler.y * 0.5f);
    float cz = Mathf.Cos(euler.z * 0.5f);
    float sx = Mathf.Sin(euler.x * 0.5f);
    float sy = Mathf.Sin(euler.y * 0.5f);
    float sz = Mathf.Sin(euler.z * 0.5f);

    transform.rotation = new Quaternion(
        sx * cy * cz + cx * sy * sz, //X
        cx * sy * cz - sx * cy * sz, //Y
        cx * cy * sz - sx * sy * cz, //Z
        cx * cy * cz + sx * sy * sz  //W
    ); 
  }
}
