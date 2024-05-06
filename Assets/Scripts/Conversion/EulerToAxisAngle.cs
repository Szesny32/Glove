using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EulerToAxisAngle : MonoBehaviour
{
    
    [SerializeField]
    private Transform source;
    //public bool customMode = true;

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

        //if(customMode){
            //Unity performs the Euler rotations sequentially around the z-axis, the x-axis and then the y-axis.
            float ey = euler.y * Mathf.Deg2Rad * 0.5f;
            float ez = euler.z * Mathf.Deg2Rad * 0.5f;
            float ex = euler.x * Mathf.Deg2Rad * 0.5f;

            float cex = Mathf.Cos(ex);
            float cey = Mathf.Cos(ey);
            float cez = Mathf.Cos(ez);
            float sex = Mathf.Sin(ex);
            float sey = Mathf.Sin(ey);
            float sez = Mathf.Sin(ez);

            angle = 2 * Mathf.Acos(cex*cey*cez + sex*sey*sez) * Mathf.Rad2Deg;
            axis = new Vector3(
                sex*cey*cez + cex*sey*sez,
                cex*sey*cez - sex*cey*sez,
                cex*cey*sez - sex*sey*cez
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
        //}

        //else {
        //    Quaternion.Euler( source.rotation.eulerAngles).ToAngleAxis(out angle, out axis);
        //}

        transform.rotation = Quaternion.AngleAxis(angle, axis);


    }
}
