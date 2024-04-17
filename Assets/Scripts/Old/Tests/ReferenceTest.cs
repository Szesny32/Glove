using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TestType {
    GlobalToLocal
}

public class ReferenceTest : MonoBehaviour
{

    public TestType testType;
    public Vector3 referenceVector = Vector3.down;

    void Start()
    {}

    void Update()
    {
        if(testType == TestType.GlobalToLocal){
            Debug.DrawRay(transform.position, referenceVector, Color.green);
            Debug.DrawRay(transform.position, transform.up, Color.blue);
        }
    }
}
