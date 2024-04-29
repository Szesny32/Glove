using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    


    [SerializeField]
    private Quaternion rotation_true;

    [SerializeField]
    private Vector3 angles_euler_true;

    [SerializeField]
    private Vector3 axis_true;

    [SerializeField]
    private float angle_true;

    [SerializeField]
    private Vector3 transformUp;

    [SerializeField]
    private Vector3 accelerometer_unity;

    //[SerializeField]
    //private Vector3 accelerometer_custom;



    



    //[SerializeField]
    //private Vector3 axis_1;

    //[SerializeField]
    //private float angle_1;


    


    



    //[SerializeField]
    //private Quaternion rotation_1;



    void Start()
    {
        
    }
    
    void Update()
    {
        UpdateAccelerometer();
        Debug.DrawRay(transform.position, transform.up, Color.green);
        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        Debug.DrawRay(transform.position, transform.right, Color.red);
        Debug.DrawRay(transform.position, axis_true, Color.yellow);
    }

    private void UpdateAccelerometer(){

        
        Quaternion inverseRotation = Quaternion.Inverse(transform.rotation);
        //accelerometer_custom.x = 2.0f * (inverseRotation.x * inverseRotation.y - inverseRotation.w * inverseRotation.z);
        //accelerometer_custom.y = -Mathf.Pow(inverseRotation.x, 2) + Mathf.Pow(inverseRotation.y, 2) - Mathf.Pow(inverseRotation.z, 2) + Mathf.Pow(inverseRotation.w, 2);
        //accelerometer_custom.z = 2.0f * (inverseRotation.z * inverseRotation.y + inverseRotation.w * inverseRotation.x);

        accelerometer_unity = inverseRotation * Vector3.up;

        rotation_true = transform.rotation;
        //rotation_1 = Quaternion.FromToRotation(Vector3.up, accelerometer_custom);


        transform.rotation.ToAngleAxis(out angle_true, out axis_true);
        //rotation_1.ToAngleAxis(out angle_1, out axis_1);

    
        transformUp = transform.up;
        //transformUp2 = transform.rotation * Vector3.up;
        //transformUp3 = Quaternion.FromToRotation(Vector3.up, transform.up) * Vector3.up;

        //float q1 = axis_true.x * Mathf.Sin((angle_true/ 2.0f) * Mathf.Deg2Rad);
        //Debug.Log(q1);


        //angles_euler_true = transform.eulerAngles;
        //angles_euler_1.x = - Mathf.Atan2(accelerometer_custom.z, Mathf.Sqrt(Mathf.Pow(accelerometer_custom.x, 2) + Mathf.Pow(accelerometer_custom.y, 2))) * Mathf.Rad2Deg;  
       // angles_euler_1.z = Mathf.Atan2 (accelerometer_custom.x, accelerometer_custom.y) * Mathf.Rad2Deg;

    }
    
    public Vector3 GetAccelerometer(){
        return accelerometer_unity;
    }



}
