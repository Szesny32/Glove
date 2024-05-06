using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionToAxisAngle : MonoBehaviour
{
    [SerializeField]
    private Transform source;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Conversion(source.rotation);
    }


    private void Conversion(Quaternion q)
    {
        Vector3 axis = Vector3.zero;
        float angle = 0f;

        if (Mathf.Abs(q.w) > 1.0f)
            q.Normalize();


        angle = 2.0f * (float)System.Math.Acos(q.w); 
        float den = (float)System.Math.Sqrt(1.0 - q.w * q.w);
        if (den > 0.0001f)
        {
            axis = new Vector3(q.x, q.y, q.z) / den;
        }
        else
        {
            // This occurs when the angle is zero. 
            // Not a problem: just set an arbitrary normalized axis.
            axis = new Vector3(1, 0, 0);
        }

        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axis);
    }
}
