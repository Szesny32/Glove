using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOld : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
                
        Quaternion inverseRotation = Quaternion.Inverse(transform.rotation);
        Vector3 localUp = inverseRotation * Vector3.up;
   

        Debug.DrawRay(transform.position, transform.up, Color.green); //  transform.up = transform.rotation * Vector3.up;
        //Debug.DrawRay(transform.position, localUp, Color.blue);
        Debug.DrawRay(transform.position, localUp, Color.red);
    }
}
