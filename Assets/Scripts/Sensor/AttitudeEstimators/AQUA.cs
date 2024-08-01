using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AQUA : AttitudeEstimator
{
    public override void UpdateOrientation(){
        Vector3 a = acceleration;
        _Matrix m = new _Matrix(new float[,]{{magneticField.x}, {magneticField.y}, {magneticField.z}});


        //GLOBAL TO LOCAL
         _Quaternion qA = (a.y >= 0)? 
             new _Quaternion(    Mathf.Sqrt((a.y + 1)/2),        - a.z / Mathf.Sqrt(2*(a.y + 1)),     0,                               a.x / Mathf.Sqrt(2*(a.y + 1))) 
             : new _Quaternion(  - a.z / Mathf.Sqrt(2*(1 - a.y)),  Mathf.Sqrt((1 - a.y)/2),           a.x / Mathf.Sqrt(2*(1 - a.y)),   0);
        


        _Matrix rAT = qA.toDirectionCosineMatrix().T; //localToGlobal       //CHECK
        _Matrix l = rAT * m;


        float lx = l[0,0];
        float ly = l[1,0];
        float lz = l[2,0];
        float L = Mathf.Pow(lx, 2) + Mathf.Pow(lz, 2);
        float sL = Mathf.Sqrt(L);
        float qw = (lz>=0)? Mathf.Sqrt((L + sL*lz) / (2*L))  : lx / Mathf.Sqrt(2*(L - sL*lz));
        float qy = (lz>=0)? lx / Mathf.Sqrt(2*(L + sL*lz))  : Mathf.Sqrt((L - sL*lz) / (2*L));


        //global to locAL
        _Quaternion qM = new _Quaternion(qw, 0, qy, 0); //globalToLocal


        //qM.T *qA.T * Local = Global

        //Quaternion R = qMT * qAT;



        //Quaternion R =  M.Inverse().Unity() * qA.Inverse().Unity();
        
        //Somethign is not ok
        Quaternion R = qM.Inverse().Unity() * qA.Unity();
        
        transform.rotation = R;
       
    }


}
