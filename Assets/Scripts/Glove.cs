using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TransformPair
{
    public Transform reference;
    public Transform target;
}


public class Glove : MonoBehaviour
{
    public TransformPair Arm, ForeArm, Hand;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
