using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionToEuler : MonoBehaviour
{
    
    [SerializeField]
    private Transform source;

    void Start()
    {
        
    }
    void Update()
    {
        Conversion(source.rotation);
    }
    // Update is called once per frame
    void Conversion(Quaternion quaternion)
    {

        transform.rotation = Quaternion.Euler(Internal_ToEulerRad(quaternion));

    }




        private  Vector3 Internal_ToEulerRad(Quaternion rotation)
    {
        float sqw = rotation.w * rotation.w;
        float sqx = rotation.x * rotation.x;
        float sqy = rotation.y * rotation.y;
        float sqz = rotation.z * rotation.z;
        float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
        float test = rotation.x * rotation.w - rotation.y * rotation.z;
        Vector3 v;

        if (test > 0.4995f * unit)
        { // singularity at north pole
            v.y = 2f * Mathf.Atan2(rotation.y, rotation.x);
            v.x = Mathf.PI / 2;
            v.z = 0;
            return NormalizeAngles(v * Mathf.Rad2Deg);
        }
        if (test < -0.4995f * unit)
        { // singularity at south pole
            v.y = -2f * Mathf.Atan2(rotation.y, rotation.x);
            v.x = -Mathf.PI / 2;
            v.z = 0;
            return NormalizeAngles(v * Mathf.Rad2Deg);
        }
        // Quaternion q = new Quaternion(rotation.w, rotation.z, rotation.x, rotation.y);
        // Quaternion rotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);

        // v.y = (float)Mathf.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));     // Yaw
        // v.x = (float)Mathf.Asin(2f * (q.x * q.z - q.w * q.y));                             // Pitch
        // v.z = (float)Mathf.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z));      // Roll

        v.y = (float)Mathf.Atan2(2f * rotation.w * rotation.y + 2f * rotation.z *  rotation.x, 1 - 2f * ( rotation.x *  rotation.x + rotation.y * rotation.y));     // Yaw
        v.x = (float)Mathf.Asin(2f * (rotation.w *  rotation.x - rotation.y * rotation.z));                             // Pitch
        v.z = (float)Mathf.Atan2(2f * rotation.w * rotation.z + 2f * rotation.x * rotation.y, 1 - 2f * (rotation.z * rotation.z +  rotation.x *  rotation.x));      // Roll
        return NormalizeAngles(v * Mathf.Rad2Deg);
    }


   private Vector3 NormalizeAngles(Vector3 angles)
    {
        angles.x = NormalizeAngle(angles.x);
        angles.y = NormalizeAngle(angles.y);
        angles.z = NormalizeAngle(angles.z);
        return angles;
    }

    private  float NormalizeAngle(float angle)
    {
		float modAngle = angle % 360.0f;
		
		if (modAngle < 0.0f)
			return modAngle + 360.0f;
		else
			return modAngle;
    }



        private  Vector3 Internal_MakePositive(Vector3 euler)
        {
            float negativeFlip = -0.0001f * Mathf.Rad2Deg;
            float positiveFlip = 360.0f + negativeFlip;

            if (euler.x < negativeFlip)
                euler.x += 360.0f;
            else if (euler.x > positiveFlip)
                euler.x -= 360.0f;

            if (euler.y < negativeFlip)
                euler.y += 360.0f;
            else if (euler.y > positiveFlip)
                euler.y -= 360.0f;

            if (euler.z < negativeFlip)
                euler.z += 360.0f;
            else if (euler.z > positiveFlip)
                euler.z -= 360.0f;

            return euler;
        }

}
