using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorOld: MonoBehaviour
{
    IEstimationStrategy estimationStrategy;
    int i = 0;
    void Start() {
        estimationStrategy = EstimationExecutor.GetStrategy(EstimationStrategy.GyroscopeMethod);


    }

    public Vector3 AccGravity = Vector3.zero;

    void Update() {





        estimationStrategy.Estimate();

        Debug.DrawRay(transform.position, transform.up, Color.green);
        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        Debug.DrawRay(transform.position, transform.right, Color.red);




        //Debug.DrawRay(transform.position, Vector3.down, Color.green);'

        AccGravity = transform.InverseTransformDirection(Vector3.down);
        Debug.DrawRay(transform.position, AccGravity, Color.cyan);

        
        Quaternion inverseRotation = Quaternion.Inverse(transform.rotation);
        Vector3 downLocalQuaternion = inverseRotation * Vector3.down;

        Debug.DrawRay(transform.position, downLocalQuaternion, Color.gray);


        Debug.DrawRay(transform.position, transform.TransformDirection(AccGravity), Color.yellow);
    
    


        Debug.Log($"Up: {transform.up} \nQuaternionXYZW {transform.rotation} \n\nAccGravity: {AccGravity}\n\ninverseRotation: {inverseRotation}\ndownLocalQuaternion: {downLocalQuaternion}");
    }
}