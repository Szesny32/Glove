using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTest : MonoBehaviour
{
    public ReferenceTest reference;

    [SerializeField]
    private Vector3 localVector;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(reference){
            if(reference.testType == TestType.GlobalToLocal){
                Quaternion inverseRotationQuaternion = Quaternion.Inverse(transform.rotation);
                localVector = inverseRotationQuaternion * reference.referenceVector;
                Debug.DrawRay(transform.position, reference.referenceVector, Color.red);
                Debug.DrawRay(transform.position, localVector, Color.green);
                Debug.DrawRay(transform.position, transform.up, Color.blue);
            }
        }
    }
}
