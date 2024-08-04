using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AngularRateType{
    EE267,
    Taylor
}
public class AngularRate : AttitudeEstimator
{
    public AngularRateType type;
    private _Quaternion Q = new _Quaternion(1, 0, 0, 0);
    public int Taylor_K_Order = 1;

    public override void UpdateOrientation(){
        

        if(type == AngularRateType.EE267){
            Vector3 angularRate = angularVelocity * Time.deltaTime;
            Quaternion dQ = Quaternion.AngleAxis(angularRate.magnitude * Mathf.Rad2Deg, angularVelocity.normalized);
            transform.rotation *= dQ; 
        } 
        
        else if(type == AngularRateType.Taylor){
            float dt = Time.deltaTime;
            Q = f(Omega(angularVelocity), dt, Taylor_K_Order) * Q;
            transform.rotation = Q.Unity();
        }  
        
    }


    private _Matrix f(_Matrix OmegaW, float dt, int k=1){

        if(k == 0)
            return _Matrix.Identity(4);

        _Matrix result = _Matrix.Identity(4);
        float K = 1;
        for(int i = 1; i <= k; i++){
            result *= ((dt/2) * OmegaW);
            K *= i;
        }    

        return result/K + f(OmegaW, dt, k-1);
    }




    private _Matrix Omega(Vector3 w){
        return new _Matrix(
            new float[,]{
                {0, -w.x, -w.y, -w.z},
                {w.x, 0, w.z, -w.y},
                {w.y, -w.z, 0, w.x},
                {w.z, w.y, -w.x, 0}
            }
        );
    }
}
