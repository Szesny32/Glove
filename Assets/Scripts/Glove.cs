using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


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
    private Complementary[] estimators;

    void Start()
    {
        transforms = new TransformPair[]{Arm, ForeArm, Hand, HandIndex1, HandIndex2, HandIndex3, HandIndex4, HandMiddle1, HandMiddle2, HandMiddle3, HandMiddle4, HandPinky1, HandPinky2, HandPinky3, HandPinky4, HandRing1, HandRing2, HandRing3, HandRing4, HandThumb1, HandThumb2, HandThumb3, HandThumb4};
        estimators = new Complementary[transforms.Length];

        for(int i = 0; i <  transforms.Length; i++){
            GyroSim gyroscope = transforms[i].reference.AddComponent<GyroSim>();
            AccSim accelerometer = transforms[i].reference.AddComponent<AccSim>();
            MagSim magnetometer = transforms[i].reference.AddComponent<MagSim>();

            estimators[i] = transforms[i].target.AddComponent<Complementary>();
            estimators[i].Initialize(transforms[i].reference, gyroscope, accelerometer, magnetometer);
        }

    }

    // Update is called once per frame
    void Update()
    {
        foreach(Complementary estimator in estimators){
            estimator.UpdateOrientation();
            //pair.target.rotation = pair.reference.rotation;
        }
    }
}
