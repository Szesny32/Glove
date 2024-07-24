using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testQuat : MonoBehaviour
{
    Vector3 rotationDelta;
    Vector3 rotationDelta2;
    Quaternion deltaRotation;
    Rigidbody rb;

    Quaternion prevRotation;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        prevRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (rb == null){
            transform.localRotation *= Quaternion.Euler(new Vector3(5, 5, 0) * Time.deltaTime);
        }


        string log = $"{rb.angularVelocity * Mathf.Rad2Deg * Time.deltaTime}\n {transform.rotation.eulerAngles}";
        Debug.Log(log);
        prevRotation = transform.rotation;
    }


    void FixedUpdate()
    {
        if (rb != null)
        {
            Vector3 angularVelocityInRadians =  new Vector3(5, 5, 0) * Mathf.Deg2Rad;
            rb.angularVelocity = angularVelocityInRadians;
        }
    }
}
