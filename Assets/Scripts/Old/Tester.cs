using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    


    

    [SerializeField]
    private Quaternion rotation;

    [SerializeField]
    private Vector3 eulerAngles;

    [SerializeField]
    private Vector3 axis;

    [SerializeField]
    private float angle;

    [SerializeField]
    private Vector3 transformUp;

    [SerializeField]
    private Vector3 accelerometer;

    [SerializeField]
    private Vector3 accelerometer_custom;


    [SerializeField]
    private Vector3 accelerometer_euler;



    [System.Serializable]
    public struct AxisAngle{
        public Vector3 axis;
        public float angle;
    }

    [System.Serializable]
    public class MyEuler
    {
        public Vector3 eulerAngles;
        public Quaternion toQuaternion;
        public AxisAngle toAxisAngle;

        public static Quaternion ToQuaternion (Vector3 euler) {
            Quaternion result = new Quaternion();
            float cx = Mathf.Cos(euler.x * Mathf.Deg2Rad * 0.5f);
            float cy = Mathf.Cos(euler.y * Mathf.Deg2Rad * 0.5f);
            float cz = Mathf.Cos(euler.z * Mathf.Deg2Rad * 0.5f);
            float sx = Mathf.Sin(euler.x * Mathf.Deg2Rad * 0.5f);
            float sy = Mathf.Sin(euler.y * Mathf.Deg2Rad * 0.5f);
            float sz = Mathf.Sin(euler.z * Mathf.Deg2Rad * 0.5f);

            result.x = sx * cy * cz + cx * sy * sz;
            result.y = cx * sy * cz - sx * cy * sz;
            result.z = cx * cy * sz - sx * sy * cz;
            result.w = cx * cy * cz + sx * sy * sz;
            return result;
        }

    }

    [SerializeField]
    private MyEuler eulerUnity;

    [SerializeField]
    private MyEuler eulerCustom;

    [SerializeField]
    private Quaternion testQuaternion;

    [SerializeField]
    private Vector3 testEuler;



    void Start() {}
    
    void Update()
    {
        
        //Base
        rotation = transform.rotation;
        eulerAngles = rotation.eulerAngles;
        transform.rotation.ToAngleAxis(out angle, out axis);
        transformUp = transform.up;

        //Accelerometer
        Quaternion inverseRotation = Quaternion.Inverse(rotation);
        accelerometer = inverseRotation * Vector3.up;
        accelerometer_custom.x = 2.0f * (inverseRotation.x * inverseRotation.y - inverseRotation.w * inverseRotation.z);
        accelerometer_custom.y = -Mathf.Pow(inverseRotation.x, 2) + Mathf.Pow(inverseRotation.y, 2) - Mathf.Pow(inverseRotation.z, 2) + Mathf.Pow(inverseRotation.w, 2);
        accelerometer_custom.z = 2.0f * (inverseRotation.z * inverseRotation.y + inverseRotation.w * inverseRotation.x);

        accelerometer_euler.x = - Mathf.Atan2(accelerometer_custom.z, Mathf.Sqrt(Mathf.Pow(accelerometer_custom.x, 2) + Mathf.Pow(accelerometer_custom.y, 2))) * Mathf.Rad2Deg;  
        accelerometer_euler.z = Mathf.Atan2 (accelerometer_custom.x, accelerometer_custom.y) * Mathf.Rad2Deg;

        accelerometer_euler.x = (accelerometer_euler.x  + 360f) %360f;
        accelerometer_euler.z = (accelerometer_euler.z + 360f) %360f;

        //Euler convertion - UNITY
        eulerUnity.eulerAngles = transform.rotation.eulerAngles;
        eulerUnity.toQuaternion = Quaternion.Euler(eulerUnity.eulerAngles);
        eulerUnity.toQuaternion.ToAngleAxis(out eulerUnity.toAxisAngle.angle, out eulerUnity.toAxisAngle.axis);
      
        //Euler convertion - CUSTOM
        eulerCustom.eulerAngles = transform.rotation.eulerAngles;
        eulerCustom.toQuaternion = MyEuler.ToQuaternion(eulerCustom.eulerAngles);
        ToAngleAxis(eulerCustom.toQuaternion, out eulerCustom.toAxisAngle.angle, out eulerCustom.toAxisAngle.axis);

    }


   

    public static void ToAngleAxis(Quaternion q, out float angle, out Vector3 axis)
    {
        if (Mathf.Abs(q.w) > 1.0f)
            q.Normalize();

        angle = 2.0f * Mathf.Acos(q.w) * Mathf.Rad2Deg;
        float s = Mathf.Sin(angle* Mathf.Deg2Rad * 0.5f);

        if (s < 0.001f)
        {
            angle = 0f;
            axis = new Vector3(1, 0, 0);
        }
        else
        {
            axis = new Vector3(q.x / s, q.y / s, q.z / s);
        }
    }

    public Vector3 getUp(){
        return transform.up;
    }
    


}

