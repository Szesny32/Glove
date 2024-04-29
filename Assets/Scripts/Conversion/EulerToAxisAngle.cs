using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EulerToAxisAngle : MonoBehaviour
{
    
    [SerializeField]
    private Transform source;
    public bool customMode = true;

    void Start()
    {
        
    }

    
    void Update()
    {
        Conversion(source.rotation.eulerAngles);
    }

    private void Conversion(Vector3 euler){

        float angle = 0f;
        Vector3 axis = Vector3.zero;

        if(customMode){
            //Unity performs the Euler rotations sequentially around the z-axis, the x-axis and then the y-axis.
            float heading = euler.y * Mathf.Deg2Rad * 0.5f;
            float attitude = euler.z * Mathf.Deg2Rad * 0.5f;
            float bank = euler.x * Mathf.Deg2Rad * 0.5f;

            float c1 = Mathf.Cos(heading);
            float c2 = Mathf.Cos(attitude);
            float c3 = Mathf.Cos(bank);
            float s1 = Mathf.Sin(heading);
            float s2 = Mathf.Sin(attitude);
            float s3 = Mathf.Sin(bank);

            angle = 2 * Mathf.Acos(c1*c2*c3 - s1*s2*s3) * Mathf.Rad2Deg;
            axis = new Vector3(
                s1*s2*c3 + c1*c2*s3,
                s1*c2*c3 + c1*s2*s3,
                c1*s2*c3 - s1*c2*s3
            );

            float norm = axis.x*axis.x + axis.y*axis.y + axis.z*axis.z;
            if (norm < 0.001f) { // when all euler angles are zero angle =0 so
                // we can set axis to anything to avoid divide by zero
                axis.x = 1;
                axis.y = axis.z = 0;
            } else {
                norm = Mathf.Sqrt(norm);
                axis.x /= norm;
                axis.y /= norm;
                axis.z /= norm;
            }
        }

        else {
            Quaternion.Euler( source.rotation.eulerAngles).ToAngleAxis(out angle, out axis);
        }

        transform.rotation = Quaternion.AngleAxis(angle, axis);


    }
}
