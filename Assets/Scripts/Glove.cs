using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[System.Serializable]
public class TransformPair {
    public Transform reference;
    public Transform target;
}


public class Glove : MonoBehaviour
{
    public TransformPair Arm, ForeArm, Hand;
    public TransformPair HandIndex1, HandIndex2, HandIndex3, HandIndex4;
    public TransformPair HandMiddle1, HandMiddle2, HandMiddle3, HandMiddle4;
    public TransformPair HandPinky1, HandPinky2, HandPinky3, HandPinky4;
    public TransformPair HandRing1, HandRing2, HandRing3, HandRing4;
    public TransformPair HandThumb1, HandThumb2, HandThumb3, HandThumb4;


    private TransformPair[] transforms;
    private AttitudeEstimator[] estimators;




    public void Start()
    {
        transforms = new TransformPair[]{Arm, ForeArm, Hand, HandIndex1, HandIndex2, HandIndex3, HandIndex4, HandMiddle1, HandMiddle2, HandMiddle3, HandMiddle4, HandPinky1, HandPinky2, HandPinky3, HandPinky4, HandRing1, HandRing2, HandRing3, HandRing4, HandThumb1, HandThumb2, HandThumb3, HandThumb4};
        estimators = new EKF[transforms.Length];

         for(int i = 0; i < transforms.Length; i++){

            GyroSim gyroscope = transforms[i].reference.AddComponent<GyroSim>();
            gyroscope.noisy = true;
            AccSim accelerometer = transforms[i].reference.AddComponent<AccSim>();
            accelerometer.noisy = true;
            MagSim magnetometer = transforms[i].reference.AddComponent<MagSim>();
            magnetometer.noisy = true;


            estimators[i] = transforms[i].target.AddComponent<EKF>();
            estimators[i].Initialize(transforms[i].reference, gyroscope, accelerometer, magnetometer, false);
        }

    }

    public void Update(){
        foreach(EKF estimator in estimators){
            estimator.UpdateOrientation();
        }
    }   




    



}
