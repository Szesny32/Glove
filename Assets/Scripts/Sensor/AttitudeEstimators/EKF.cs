using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EKF : AttitudeEstimator
{


    
    private Vector4 state = new Vector4(1, 0, 0, 0);
    private Vector4 state_predicted;

    private Matrix4x4 P; //Noise covariance;
    private Matrix4x4 P_predicted; //Predicted noise covariance

    
    private Matrix4x4 Q;  //Process Noise Covariance Matrix

    float[,] _H;
    float[,] v;
    float[,] K;

    public override void UpdateOrientation(){
        PredictionStep();
        CorrectionStep();


        

        float[,] correction = _Matrix.Multiply(K, v);
        // float[,] correction = new float[,]{
        //     {0,0,0,0},
        //     {0,0,0,0},
        //     {0,0,0,0},
        //     {0,0,0,0}
        // };

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



    //PREDICTION STEP

    private Matrix4x4 Omega(Vector3 w){
        Matrix4x4 omega = new Matrix4x4();
        omega.SetRow(0, new Vector4(0, -w.x, -w.y, -w.z));
        omega.SetRow(1, new Vector4(w.x, 0, w.z, -w.y));
        omega.SetRow(2, new Vector4(w.y, -w.z, 0, w.x));
        omega.SetRow(3, new Vector4(w.z, w.y, -w.x, 0));
        return omega;
    }

    private Vector4 f(Vector4 q, Vector3 w, float dt){
        return (_Matrix.Add(Matrix4x4.identity, _Matrix.Scale(Omega(w), 0.5f * dt)) *  q).normalized;
    }

    //Jacobian
    private Matrix4x4 FJacobian(Vector3 w, float dt){
            float Fx = 0.5f * dt * w.x;
            float Fy = 0.5f * dt * w.y;
            float Fz = 0.5f * dt * w.z;

            Matrix4x4 F = new Matrix4x4();
            F.SetRow(0, new Vector4(1, -Fx, -Fy, -Fz));
            F.SetRow(1, new Vector4(Fx, 1, Fz, -Fy));
            F.SetRow(2, new Vector4(Fy, -Fz, 1, Fx));
            F.SetRow(3, new Vector4(Fz, Fy, -Fx, 1));
            return F;
    }


    private float[,] WJacobian(Vector4 q, float dt){
        (float qw, float qx, float qy, float qz) = (0.5f * dt * q[0], 0.5f * dt * q[1], 0.5f * dt * q[2], 0.5f * dt * q[3]);
        return new float[,]{
            { -qx, -qy, -qz},
            { qw, -qz, qy},
            { qz, qw, -qx},
            { -qy, qx, qw}
        };
    }

    //spectral noise covariance matrix
    private float[,] spectralNoiseCovarianceMatrix(Vector3 spectralDensity){

           return new float[,]{
            { spectralDensity.x, 0, 0},
            { 0, spectralDensity.y, 0},
            { 0, 0, spectralDensity.z}
        };
    }

    //Q
    private Matrix4x4 ProcessNoiseCovarianceMatrix(Vector3 spectralDensity, Vector4 q, float dt){
        float[,] W = WJacobian(q, dt);
        float[,] Ew = spectralNoiseCovarianceMatrix(spectralDensity);

        float[,] result = _Matrix.Multiply(_Matrix.Multiply(W, Ew), _Matrix.Transpose(W));
        return _Matrix.ToMatrix4x4(result);
        
    }


    //Zamiast Omegi można wykorzystać EE-267(?)
    private void PredictionStep(){

        //predicted state
        float dt = Time.deltaTime;
        state_predicted = f(state, angularVelocity, dt);

        //predicted covariance matrix
        Matrix4x4 F = FJacobian(angularVelocity, dt);
        Q = ProcessNoiseCovarianceMatrix(gyroscopeNoise, state, dt);
        P_predicted = _Matrix.Add(F * P * F.transpose, Q);


    }

    private float[,] h(Vector4 q){
        Vector3 g = new Vector3(0, 0 , -1); //NED

        //Magnetic Dip Angle [TODO]
        float theta = 0f;

        Vector3 r = new Vector3(Mathf.Cos(theta), 0 , Mathf.Sin(theta)).normalized;
        (float qw, float qx, float qy, float qz) = (q[0], q[1], q[2], q[3]);
        float qx2 = qx * qx;
        float qy2 = qy * qy;
        float qz2 = qz * qz;

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
        return _Matrix.Scale(result, 2);
    }

    private float[,] H(Vector4 q){
        Vector3 g = new Vector3(0, 0 , -1); //NED
        //Magnetic Dip Angle [TODO]
        float theta = 0f;

        Vector3 r = new Vector3(Mathf.Cos(theta), 0 , Mathf.Sin(theta)).normalized;

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

    private void CorrectionStep(){
        Vector3 a = acceleration.normalized;
        Vector3 m = magneticField.normalized;
        float[,] z = new float[,]{
            { a.x } ,
            { a.y } ,
            { a.z } ,
            { m.x } ,
            { m.y } ,
            { m.z } 
        };

        //(Measurement acc_mag) subtract (predicted state as acc_mag)
        v = _Matrix.Subtract(z,  h(state_predicted));

        float[,] R = {
            {acceleromterNoise.x, 0, 0, 0, 0, 0},
            {0, acceleromterNoise.y, 0, 0, 0, 0},
            {0, 0, acceleromterNoise.z, 0, 0, 0},
            {0, 0, 0, magnetometerNoise.x, 0, 0},
            {0, 0, 0, 0, magnetometerNoise.y, 0},
            {0, 0, 0, 0, 0, magnetometerNoise.z}
        };

        _H = H(state_predicted);
        float[,] _HT = _Matrix.Transpose(_H);

        float[,] S = _Matrix.Add(_Matrix.Multiply(_Matrix.Multiply(_H, P_predicted), _HT), R); 


        for (int i = 0; i < S.GetLength(0); i++) {
            S[i, i] += 1e-6f;
        }

        float[,] S_inv = _Matrix.InvertMatrix(S);

        K = _Matrix.Multiply(_Matrix.Multiply(P_predicted, _HT), S_inv);
        
    }


}
