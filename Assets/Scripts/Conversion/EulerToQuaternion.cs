using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EulerToQuaternion : MonoBehaviour
{
    public GameObject gameObject;

    void Start() {
        
    }
    
    void Update() {
        Conversion(gameObject.transform.rotation.eulerAngles);
    }


    public void Conversion(Vector3 euler) {
        Quaternion quaternion = new Quaternion();
        //Returns a quaternion constructed by first performing a rotation around the z-axis, then the x-axis and finally the y-axis.
        //Unity performs the Euler rotations sequentially around the z-axis, the x-axis and then the y-axis
        float cx = Mathf.Cos(euler.x * Mathf.Deg2Rad * 0.5f);
        float cy = Mathf.Cos(euler.y * Mathf.Deg2Rad * 0.5f);
        float cz = Mathf.Cos(euler.z * Mathf.Deg2Rad * 0.5f);
        float sx = Mathf.Sin(euler.x * Mathf.Deg2Rad * 0.5f);
        float sy = Mathf.Sin(euler.y * Mathf.Deg2Rad * 0.5f);
        float sz = Mathf.Sin(euler.z * Mathf.Deg2Rad * 0.5f);

        quaternion.x = sx * cy * cz + cx * sy * sz;
        quaternion.y = cx * sy * cz - sx * cy * sz;
        quaternion.z = cx * cy * sz - sx * sy * cz;
        quaternion.w = cx * cy * cz + sx * sy * sz;

        transform.rotation = quaternion; 
    }

}
