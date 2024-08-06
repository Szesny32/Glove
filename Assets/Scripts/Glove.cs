using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        transforms = new TransformPair[]{Arm, ForeArm, Hand, HandIndex1, HandIndex2, HandIndex3, HandIndex4, HandMiddle1, HandMiddle2, HandMiddle3, HandMiddle4, HandPinky1, HandPinky2, HandPinky3, HandPinky4, HandRing1, HandRing2, HandRing3, HandRing4, HandThumb1, HandThumb2, HandThumb3, HandThumb4};
        foreach(TransformPair pair in transforms){
            GyroSim gyroscope = pair.reference.AddComponent<GyroSim>();
            AccSim accelerometer = pair.reference.AddComponent<AccSim>();
            MagSim mangetometer = pair.reference.AddComponent<MagSim>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        foreach(TransformPair pair in transforms){
            pair.target.rotation = pair.reference.rotation;
        }
    }
}
