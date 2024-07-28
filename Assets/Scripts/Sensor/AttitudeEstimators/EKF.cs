using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EKF : AttitudeEstimator
{


    
    private _Quaternion state = new _Quaternion(1, 0, 0, 0);
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



    //The measurement noise covariance matrix
    private _Matrix R = new _Matrix( new float[,]{
            {Mathf.Pow(acceleromterNoise.x, 2), 0, 0, 0, 0, 0},
            {0, Mathf.Pow(acceleromterNoise.y, 2), 0, 0, 0, 0},
            {0, 0, Mathf.Pow(acceleromterNoise.z, 2), 0, 0, 0},
            {0, 0, 0, Mathf.Pow(magnetometerNoise.x, 2), 0, 0},
            {0, 0, 0, 0, Mathf.Pow(magnetometerNoise.y, 2), 0},
            {0, 0, 0, 0, 0, Mathf.Pow(magnetometerNoise.z, 2)}
    });


    float[,] _H;
    float[,] v;
    float[,] K;


    public override void UpdateOrientation(){
        PredictionStep();
        CorrectionStep();

        float[,] correction = _Matrix.Multiply(K, v);

        state = new Vector4( 
            state_predicted[0] + correction[0,0],
            state_predicted[1] + correction[1,0],
            state_predicted[2] + correction[2,0],
            state_predicted[3] + correction[3,0]
        );
        transform.rotation = new Quaternion(state[1], state[2], state[3], state[0]).normalized;

        
        float[,] newP = _Matrix.Multiply(_Matrix.Subtract(_Matrix.Identity(), _Matrix.Multiply(K, _H)), P_predicted);
        P_predicted = _Matrix.ToMatrix4x4(newP);

    }


    //-----------PREDICTION STEP------------------------------------------------------------------------------

    //Zamiast Omegi można wykorzystać EE-267(?)
    private void PredictionStep(){
        float dt = Time.deltaTime;
        state_predicted = f(state, angularVelocity, dt);

        _Matrix F = FJacobian(angularVelocity, dt);
        Q = ProcessNoiseCovarianceMatrix(gyroscopeNoise, state, dt);
        P_predicted = F * P * F.T + Q;
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
        Vector F = w * (dt/2);
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
            { Mathf.Pow(noise.x, 2), 0, 0},
            { 0, Mathf.Pow(noise.y, 2), 0},
            { 0, 0, Mathf.Pow(noise.z, 2)}
        });
    }

    //--------------------------------------------------------------------------------------------------------



    //-----------CORRECTION STEP------------------------------------------------------------------------------


    private void CorrectionStep(){

        Vector3 a = acceleration.normalized;
        Vector3 m = magneticField.normalized;
        _Matrix z = NEW _Matrix(new float[,]{
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

        float[,] _HT = _Matrix.Transpose(_H);

        float[,] S = _Matrix.Add(_Matrix.Multiply(_Matrix.Multiply(_H, P_predicted), _HT), R); 

        for (int i = 0; i < S.GetLength(0); i++) {
            S[i, i] += 1e-6f;
        }
        float[,] S_inv = _Matrix.InvertMatrix(S);
        K = _Matrix.Multiply(_Matrix.Multiply(P_predicted, _HT), S_inv);

        
    }

    private _Matrix h(_Quaternion q){
        float qx2 = q.x * q.x;
        float qy2 = q.y * q.y;
        float qz2 = q.z * q.z;

        float qwqx = qw * qx;
        float qwqy = qw * qy;
        float qwqz = qw * qz;
        float qxqy = qx * qy;
        float qxqz = qx * qz;
        float qyqz = qy * qz;

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

    private float[,] H(Vector4 q){

        (float qw, float qx, float qy, float qz) = (q[0], q[1], q[2], q[3]);

        //[TODO]: NEED TO OPTIMALIZE - IF IT IS ZERO NOW - WHY MULTIPLY IT FURTHER
        float[,] result = new float[,]{
            {g.x*qw  +g.y*qz   -g.z*qy,       g.x*qx  +g.y*qy  +g.z*qz,        -g.x*qy  +g.y*qx  -g.z*qw,        -g.x*qz  +g.y*qw  +g.z*qx},
            {-g.x*qz   +g.y*qw   +g.z*qx,       g.x*qy  -g.y*qx  +g.z*qw,        g.x*qx  +g.y*qy  +g.z*qz,        -g.x*qw  -g.y*qz  +g.z*qy},
            {g.x*qy   -g.y*qx   +g.z*qw,       g.x*qz  -g.y*qw  -g.z*qx,        g.x*qw  +g.y*qz  -g.z*qy,        g.x*qx  +g.y*qy  +g.z*qz},
            {r.x*qw  +r.y*qz   -r.z*qy,       r.x*qx  +r.y*qy  +r.z*qz,        -r.x*qy  +r.y*qx  -r.z*qw,        -r.x*qz  +r.y*qw  +r.z*qx},
            {-r.x*qz   +r.y*qw   +r.z*qx,       r.x*qy  -r.y*qx  +r.z*qw,        r.x*qx  +r.y*qy  +r.z*qz,        -r.x*qw  -r.y*qz  +r.z*qy},
            {r.x*qy   -r.y*qx   +r.z*qw,       r.x*qz  -r.y*qw  -r.z*qx,        r.x*qw  +r.y*qz  -r.z*qy,        r.x*qx  +r.y*qy  +r.z*qz}
        };

        return _Matrix.Scale(result, 2);
    }


}
