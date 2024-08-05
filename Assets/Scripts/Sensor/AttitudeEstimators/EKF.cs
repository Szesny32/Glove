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


    string log;   


    public override void Init(){
        state = new _Quaternion(transform.rotation);
        P = ProcessNoiseCovarianceMatrix(gyroscopeNoise, state, Time.deltaTime);

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

        state = (state_predicted + correction).normalized;


        transform.rotation = state.Unity();

        P = (_Matrix.Identity(4) - K *_H) * P_predicted;
        

    }

    private void PredictionStep(){
        log+=$"PREDICTION STEP\n\n";

        float dt = Time.deltaTime;
        state_predicted = f(state, angularVelocity, dt).normalized;

        _Matrix F = FJacobian(angularVelocity, dt);

        Q = ProcessNoiseCovarianceMatrix(gyroscopeNoise, state, dt);

        P_predicted = (F * P * F.T) + Q;
    }


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

        _Matrix hg = h(state_predicted, g);
        _Matrix hm = h(state_predicted, r);
        _Matrix _h = _Matrix.StackByRows(hg, hm); //state as acc-mag

        v = z - _h; //Innovation or Measurement residual.

        _Matrix Hg = H(state_predicted, g);
        _Matrix Hr = H(state_predicted, r);
        _H = _Matrix.StackByRows(Hg, Hr);

        _Matrix S = (_H * P_predicted * _H.T) + R;

        //TODO
        for (int i = 0; i < S.GetLength(0); i++) {
            S[i, i] +=  1e-6f;
        }

        K = P_predicted * _H.T * S.Inv;
    }




    //-----------PREDICTION STEP------------------------------------------------------------------------------


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



       private _Matrix h(_Quaternion q, Vector3 d){
        float [,] result = new float[,]{ 
            {d.x*(0.5f - q.y*q.y - q.z*q.z) + d.y*(q.w*q.z + q.x*q.y) + d.z*(q.x*q.z - q.w*q.y)},
            {d.x*(q.x*q.y - q.w*q.z) + d.y*(0.5f - q.x*q.x - q.z*q.z) + d.z*(q.w*q.x + q.y*q.z)},
            {d.x*(q.w*q.y + q.x*q.z) + d.y*(q.y*q.z - q.w*q.x) + d.z*(0.5f - q.x*q.x - q.y*q.y)}
        };
        return 2f * new _Matrix(result);
    }   

    private _Matrix H(_Quaternion q, Vector3 d){
         float [,] result = new float[,]{ 
            {d.x*q.w  +d.y*q.z   -d.z*q.y,       d.x*q.x  +d.y*q.y  +d.z*q.z,        -d.x*q.y  +d.y*q.x  -d.z*q.w,        -d.x*q.z  +d.y*q.w  +d.z*q.x},
            {-d.x*q.z   +d.y*q.w   +d.z*q.x,       d.x*q.y  -d.y*q.x  +d.z*q.w,        d.x*q.x  +d.y*q.y  +d.z*q.z,        -d.x*q.w  -d.y*q.z  +d.z*q.y},
            {d.x*q.y   -d.y*q.x   +d.z*q.w,       d.x*q.z  -d.y*q.w  -d.z*q.x,        d.x*q.w  +d.y*q.z  -d.z*q.y,        d.x*q.x  +d.y*q.y  +d.z*q.z},
       };
        return 2f * new _Matrix(result);
    }


}
