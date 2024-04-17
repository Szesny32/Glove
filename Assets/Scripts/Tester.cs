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
    private Quaternion eulerToQuaternion;

    [SerializeField]
    private Vector3 eulerToAxis;

    [SerializeField]
    private float eulerToAngle;



    [SerializeField]
    private Vector3 axis2;

    [SerializeField]
    private float angle2;

    [SerializeField]
    private Quaternion rotation2;

    [SerializeField]
    private Vector3 euler2;

    void Start()
    {
        
    }
    
    void Update()
    {
        
        rotation = transform.rotation;
        eulerAngles = rotation.eulerAngles;
        transform.rotation.ToAngleAxis(out angle, out axis);
        transformUp = transform.up;

        Quaternion inverseRotation = Quaternion.Inverse(rotation);
        accelerometer = inverseRotation * Vector3.up;

        eulerToQuaternion = Quaternion.Euler(eulerAngles);
        eulerToQuaternion.ToAngleAxis(out eulerToAngle, out eulerToAxis);

        //EulerToAxisAngle(eulerAngles, out angle2, out axis2);
        rotation2 = Euler(eulerAngles);
        euler2 = rotation2.eulerAngles;

        //axis2 = transform.up;
        //angle2 = axisAngleConversion(axis, angle, axis2);
       

    }

    private void EulerToAxisAngle(Vector3 euler, out float angle, out Vector3 axis)
    {

        Quaternion tmp = Euler(euler);
        rotation2 =  tmp;//Quaternion.AngleAxis(angle2, axis2);

        ToAngleAxis(tmp, out angle, out axis);

    }


    Quaternion AngleAxis(float angle, Vector3 axis)
    {
        float halfAngleRad = Mathf.Deg2Rad * angle * 0.5f;
        float sinHalfAngle = Mathf.Sin(halfAngleRad);
        float cosHalfAngle = Mathf.Cos(halfAngleRad);
        
        float x = axis.x * sinHalfAngle;
        float y = axis.y * sinHalfAngle;
        float z = axis.z * sinHalfAngle;
        float w = cosHalfAngle;

        return new Quaternion(x, y, z, w);
    }

     public  Quaternion Euler(Vector3 euler) {
        Quaternion quaternion = new Quaternion();
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

        return quaternion;
    }

    public static void ToAngleAxis(Quaternion q, out float angle, out Vector3 axis)
    {
        if (Mathf.Abs(q.w) > 1.0f)
            q.Normalize();

        angle = 2.0f * Mathf.Acos(q.w) * Mathf.Rad2Deg;
        float s = Mathf.Sqrt(1.0f - q.w * q.w);

        if (s < 0.001f)
        {
            // gdy rotacja jest bliska zeru, osia może być dowolna
            axis = new Vector3(1, 0, 0);
        }
        else
        {
            axis = new Vector3(q.x / s, q.y / s, q.z / s);
        }
    }

    


}

