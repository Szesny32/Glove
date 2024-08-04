using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EKF : AttitudeEstimator
{


    
    private _Quaternion state;
    private _Quaternion state_predicted;

    private _Matrix P; //Noise covariance;
    private _Matrix P_predicted; //Predicted noise covariance

    private _Matrix Q;  //Process Noise Covariance Matrix

    private readonly Vector3 g = new Vector3(0, 1 , 0); //ENU

    private const float inclination = (67.73f) * Mathf.Deg2Rad;
    private const float regionalField = 50.06349f; //μT [TODO] //1f WORKS

    private readonly Vector3 r = new Vector3(            
            0f,
            -Mathf.Sin(inclination),
            Mathf.Cos(inclination)
    );


    _Matrix _H;
    _Matrix v;
    _Matrix K;

    //The measurement noise covariance matrix
    _Matrix R;
    public override void Init(){
        state = new _Quaternion(1, 0, 0, 0);
        P = _Matrix.Identity(4);

        R = new _Matrix( new float[,]{
            {acceleromterNoise.x, 0, 0, 0, 0, 0},
            {0, acceleromterNoise.y, 0, 0, 0, 0},
            {0, 0, acceleromterNoise.z, 0, 0, 0},
            {0, 0, 0, magnetometerNoise.x, 0, 0},
            {0, 0, 0, 0, magnetometerNoise.y, 0},
            {0, 0, 0, 0, 0, magnetometerNoise.z}
        });
    }


    public override void UpdateOrientation(){
        PredictionStep();
        CorrectionStep();

        //Debug.Log(K.Print());
        _Quaternion correction = (K * v).toQuaternion();
        state = state_predicted + correction;
        state.Normalize();

        transform.rotation = new Quaternion(state[1,0], state[2,0], state[3,0], state[0,0]).normalized;

        P = (_Matrix.Identity(4) - K *_H) * P_predicted;

    }


    //-----------PREDICTION STEP------------------------------------------------------------------------------

    //Zamiast Omegi można wykorzystać EE-267(?)
    private void PredictionStep(){
        float dt = Time.deltaTime;
        state_predicted = f(state, angularVelocity, dt);

        _Matrix F = FJacobian(angularVelocity, dt);
        Q = ProcessNoiseCovarianceMatrix(gyroscopeNoise, state, dt);
        P_predicted = (F * P * F.T) + Q;
    }

    private _Quaternion f(_Quaternion q, Vector3 w, float dt){
        return(_Matrix.Identity(4) + (dt/2)* Omega(w)) * q;
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

    private _Matrix FJacobian(Vector3 w, float dt){
        Vector3 F = w * (dt/2);
        return new _Matrix(
            new float[,]{
                {1, -F.x, -F.y, -F.z},
                {F.x, 1, F.z, -F.y},
                {F.y, -F.z, 1, F.x},
                {F.z, F.y, -F.x, 1}
            }
        );
    }

    //Q
    private _Matrix ProcessNoiseCovarianceMatrix(Vector3 noise, _Quaternion q, float dt){
        _Matrix W = WJacobian(q, dt);
        _Matrix Ew = spectralNoiseCovarianceMatrix(noise);
        return W * Ew * W.T;
    }

    private _Matrix WJacobian(_Quaternion q, float dt){
        return (dt/2) * new _Matrix(new float[,]{
            { -q.x, -q.y, -q.z},
            { q.w, -q.z, q.y},
            { q.z, q.w, -q.x},
            { -q.y, q.x, q.w}
        });
    }

    //spectral noise covariance matrix
    private _Matrix spectralNoiseCovarianceMatrix(Vector3 noise){
        return new _Matrix(new float[,]{
            { noise.x, 0, 0},
            { 0, noise.y, 0},
            { 0, 0, noise.z, }
        });
    }

    //--------------------------------------------------------------------------------------------------------

 

    //-----------CORRECTION STEP------------------------------------------------------------------------------


    private void CorrectionStep(){

        Vector3 a = acceleration.normalized;
        Vector3 m = magneticField.normalized;
        _Matrix z = new _Matrix(new float[,]{
            { a.x } ,
            { a.y } ,
            { a.z } ,
            { m.x } ,
            { m.y } ,
            { m.z } 
        });


        //(Measurement acc_mag) subtract (predicted state as acc_mag)
        v = z - h(state_predicted);


        _H = H(state_predicted);

 
        _Matrix S = (_H * P_predicted * _H.T) + R;

        //TODO
        for (int i = 0; i < S.GetLength(0); i++) {
            S[i, i] +=  1e-6f;
        }

        K = P_predicted * _H.T * S.Inv;
    
    }

    private _Matrix h(_Quaternion q){
        float qx2 = q.x * q.x;
        float qy2 = q.y * q.y;
        float qz2 = q.z * q.z;

        float qwqx = q.w * q.x;
        float qwqy = q.w * q.y;
        float qwqz = q.w * q.z;
        float qxqy = q.x * q.y;
        float qxqz = q.x * q.z;
        float qyqz = q.y * q.z;

        float[,] result = new float[,]{
            {g.x*(0.5f - qy2 - qz2) + g.y*(qwqz + qxqy) + g.z*(qxqz - qwqy)},
            {g.x*(qxqy - qwqz) + g.y*(0.5f - qx2 - qz2) + g.z*(qwqx + qyqz)},
            {g.x*(qwqy + qxqz) + g.y*(qyqz - qwqx) + q.z*(0.5f - qx2 - qy2)},
            {r.x*(0.5f - qy2 - qz2) + r.y*(qwqz + qxqy) + r.z*(qxqz - qwqy)},
            {r.x*(qxqy - qwqz) + r.y*(0.5f - qx2 - qz2) + r.z*(qwqx + qyqz)},
            {r.x*(qwqy + qxqz) + r.y*(qyqz - qwqx) + r.z*(0.5f - qx2 - qy2)}
        };
        return 2 * new _Matrix(result);
    }

    private _Matrix H(_Quaternion q){
        //[TODO]: NEED TO OPTIMALIZE - IF IT IS ZERO NOW - WHY MULTIPLY IT FURTHER
        float[,] result = new float[,]{
            {g.x*q.w  +g.y*q.z   -g.z*q.y,       g.x*q.x  +g.y*q.y  +g.z*q.z,        -g.x*q.y  +g.y*q.x  -g.z*q.w,        -g.x*q.z  +g.y*q.w  +g.z*q.x},
            {-g.x*q.z   +g.y*q.w   +g.z*q.x,       g.x*q.y  -g.y*q.x  +g.z*q.w,        g.x*q.x  +g.y*q.y  +g.z*q.z,        -g.x*q.w  -g.y*q.z  +g.z*q.y},
            {g.x*q.y   -g.y*q.x   +g.z*q.w,       g.x*q.z  -g.y*q.w  -g.z*q.x,        g.x*q.w  +g.y*q.z  -g.z*q.y,        g.x*q.x  +g.y*q.y  +g.z*q.z},
            {r.x*q.w  +r.y*q.z   -r.z*q.y,       r.x*q.x  +r.y*q.y  +r.z*q.z,        -r.x*q.y  +r.y*q.x  -r.z*q.w,        -r.x*q.z  +r.y*q.w  +r.z*q.x},
            {-r.x*q.z   +r.y*q.w   +r.z*q.x,       r.x*q.y  -r.y*q.x  +r.z*q.w,        r.x*q.x  +r.y*q.y  +r.z*q.z,        -r.x*q.w  -r.y*q.z  +r.z*q.y},
            {r.x*q.y   -r.y*q.x   +r.z*q.w,       r.x*q.z  -r.y*q.w  -r.z*q.x,        r.x*q.w  +r.y*q.z  -r.z*q.y,        r.x*q.x  +r.y*q.y  +r.z*q.z}
        };
        return 2 * new _Matrix(result);
    }


}
