using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MagSim : MonoBehaviour
{
    //todo: przenalizować - nie zgadzają się wpsółczynniki
        
    private const float inclination = (67.73f) * Mathf.Deg2Rad;
    private const float regionalField = 50.06349f; //μT //1f WORKS
    private readonly Vector3 r = new Vector3(
        0f,
        -Mathf.Sin(inclination),
        Mathf.Cos(inclination)
    ) * regionalField; 
    


    private Vector3 magneticPole;


    [SerializeField]
    private TMP_Text UI;

    void Start()
    {
        
    }


    void Update(){

        Quaternion q = transform.rotation;

        float qx2 = q.x * q.x;
        float qy2 = q.y * q.y;
        float qz2 = q.z * q.z;

        float qwqx = q.w * q.x;
        float qwqy = q.w * q.y;
        float qwqz = q.w * q.z;
        float qxqy = q.x * q.y;
        float qxqz = q.x * q.z;
        float qyqz = q.y * q.z;


        magneticPole = new Vector3(
            r.x*(0.5f - qy2 - qz2) + r.y*(qwqz + qxqy) + r.z*(qxqz - qwqy),
            r.x*(qxqy - qwqz) + r.y*(0.5f - qx2 - qz2) + r.z*(qwqx + qyqz),
            r.x*(qwqy + qxqz) + r.y*(qyqz - qwqx) + r.z*(0.5f - qx2 - qy2)
        ) * 2f;


        //magneticPole = yRot(transform.rotation.eulerAngles.y, xRot(transform.rotation.eulerAngles.x, zRot(transform.rotation.eulerAngles.z, origin)));
        UI.text = $"Magnetometer: {magneticPole }[μT]";
    }

    public Vector3 Read(){
        return magneticPole;
    }

    public Vector3 GetOrigin(){
        return r;
    }

    //Obróciłem rotacje z powrotem -> todo zobaczyć
    Vector3 zRot(float angle, Vector3 v){
        float radAngle = angle * Mathf.Deg2Rad;
        float sinZ = Mathf.Sin(radAngle);
        float cosZ = Mathf.Cos(radAngle);
        return new Vector3(
            (cosZ * v.x) + (-sinZ * v.y),
            (sinZ * v.x) + (cosZ * v.y),
            v.z
        );
    }

    Vector3 xRot(float angle, Vector3 v){
        float radAngle = angle * Mathf.Deg2Rad;
        float sinX = Mathf.Sin(radAngle);
        float cosX = Mathf.Cos(radAngle);
        return new Vector3(
            v.x,
            (cosX * v.y) + (-sinX * v.z),
            (sinX * v.y) + (cosX * v.z)
        );
    }

    Vector3 yRot(float angle, Vector3 v){
        float radAngle = angle * Mathf.Deg2Rad;
        float sinY = Mathf.Sin(radAngle);
        float cosY = Mathf.Cos(radAngle);
        return new Vector3(
            (cosY * v.x) + (sinY * v.z),
            v.y,
            (-sinY * v.x) + (cosY * v.z)
        );
    }
    
}
