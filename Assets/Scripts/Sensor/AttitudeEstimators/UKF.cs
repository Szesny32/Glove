using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UKF : AttitudeEstimator
{
    //https://github.com/pronenewbits/Arduino_AHRS_System/blob/master/ahrs_ukf/ukf.cpp

    private _Quaternion X; //4x1
    private _Matrix P;

    float alpha = 0.01f, beta = 2f, kappa = 0f, gamma, lambda; //scalar constants
    
    int N  = 4;

    _Matrix Wm;     //First order weight
    _Matrix Wc;     //second order weight

    _Matrix XSigma; //(N)x(2N+1) => 4x9
    _Matrix DX;


    _Matrix Y;
    _Matrix YSigma;
    _Matrix DY;
    _Matrix PY;


    _Matrix U;
    _Matrix Rv;
    _Matrix Rn;

    string log;

    private readonly Vector3 g = new Vector3(0, 1 , 0); //ENU

    private const float inclination = (67.73f) * Mathf.Deg2Rad;
    private const float regionalField = 50.06349f; //μT [TODO] //1f WORKS

    private readonly Vector3 r = new Vector3(            
            0f,
            -Mathf.Sin(inclination),
            Mathf.Cos(inclination)
    );

    public override void Init(){
        X = new _Quaternion(transform.rotation);


            acceleromterNoise /= 9.8067f;
            magnetometerNoise  /= 50.06349f; 

        P = ProcessNoiseCovarianceMatrix(gyroscopeNoise, X, Time.deltaTime);

        Rn = new _Matrix( new float[,]{
            {acceleromterNoise.x, 0, 0, 0, 0, 0},
            {0, acceleromterNoise.y, 0, 0, 0, 0},
            {0, 0, acceleromterNoise.z, 0, 0, 0},
            {0, 0, 0, magnetometerNoise.x, 0, 0},
            {0, 0, 0, 0, magnetometerNoise.y, 0},
            {0, 0, 0, 0, 0, magnetometerNoise.z}
        });





        //UKF-1
        lambda = alpha*alpha * (N+kappa) - N;        
        gamma = Mathf.Sqrt(N + lambda);           

        //UKF-2 Wm: 1xN
        //Wm = [lambda/(N+lambda)         1/(2(N+lambda)) ... 1/(2(N+lambda))] 

        float[,] wm =  new float[1, 2*N+1];
        wm[0,0] = lambda/(N + lambda);
        for (int i = 1; i < wm.GetLength(1); i++) {
            wm[0, i] = 0.5f/(N + lambda);
        }
        Wm = new _Matrix(wm);

        //UKF-3
        //Wc = [Wm(0)+(1-alpha(^2)+beta)  1/(2(N+lambda)) ... 1/(2(N+lambda))]
        Wc = new _Matrix(Wm);
        Wc[0,0]+= 1.0f - (alpha*alpha) + beta;

    }


    public override void UpdateOrientation(){
        log = "";

        // Rv = new _Matrix(new float[,]{
        //     {1e-6f, 0, 0, 0},
        //     {0, 1e-6f, 0, 0},
        //     {0, 0, 1e-6f, 0},
        //     {0, 0, 0, 1e-6f}
        // });

        float dt = Time.deltaTime;
        Rv = ProcessNoiseCovarianceMatrix(gyroscopeNoise, X, dt);

        _Matrix U = new _Matrix(new float[,]{
            {angularVelocity.x},
            {angularVelocity.y},
            {angularVelocity.z}
        });

        _Matrix Ym = new _Matrix(new float[,]{
            {acceleration.x},
            {acceleration.y},
            {acceleration.z},
            {magneticField.x},
            {magneticField.y},
            {magneticField.z}
        });


        SigmaPoints();
        UnscentedTransformX(U, Rv);

        UnscentedTransformY(U, Rn);
        for (int i = 0; i < DX.rows; i++) {
            for (int j = 0; j < DX.cols; j++) {
                DX[i, j] *= Wc[0, j];
            }
        }
        _Matrix Pxy = DX * DY.T;
        _Matrix Gain = Pxy * PY.Inv;
        _Matrix Err = Ym - Y;;
        X += (Gain*Err).toQuaternion();
        P = P - (Gain * PY * Gain.T);
        transform.rotation = X.Unity();
        //Debug.Log(log);
    }


    private void SigmaPoints(){
        /* UKF-4
            Xs(k-1) = [x(k-1) ... x(k-1)]            ; Xs(k-1) = NxN
            gSqrtP = gamma * sqrt(P(k-1))
            XSigma(k-1) = [x(k-1) Xs(k-1)+gSqrtP Xs(k-1)-gSqrtP]                 
        */


        //  Xs(k-1) = [x(k-1) ... x(k-1)]            ; Xs(k-1) = NxN
        _Matrix Xs = new _Matrix(new float[,]{
            {X.w, X.w, X.w, X.w},
            {X.x, X.x, X.x, X.x},
            {X.y, X.y, X.y, X.y},
            {X.z, X.z, X.z, X.z}
        });


        //gSqrtP = gamma * sqrt(P(k-1))
        _Matrix gSqrtP = gamma * P.CholeskyDecomposition();
     
     

        //XSigma(k-1) = [x(k-1) Xs(k-1)+gSqrtP Xs(k-1)-gSqrtP]       
        _Matrix Xsp =  Xs + gSqrtP;
        _Matrix Xsn =  Xs - gSqrtP;

        XSigma = new _Matrix(new float[,]{
            {X.w, Xsp[0,0], Xsp[0,1], Xsp[0,2], Xsp[0,3], Xsn[0,0], Xsn[0,1], Xsn[0,2], Xsn[0,3]},
            {X.x, Xsp[1,0], Xsp[1,1], Xsp[1,2], Xsp[1,3], Xsn[1,0], Xsn[1,1], Xsn[1,2], Xsn[1,3]},
            {X.y, Xsp[2,0], Xsp[2,1], Xsp[2,2], Xsp[2,3], Xsn[2,0], Xsn[2,1], Xsn[2,2], Xsn[2,3]},
            {X.z, Xsp[3,0], Xsp[3,1], Xsp[3,2], Xsp[3,3], Xsn[3,0], Xsn[3,1], Xsn[3,2], Xsn[3,3]}
        });  



    
    }

    private void UnscentedTransformX(_Matrix InpVector, _Matrix _CovNoise){

        /* XSigma(k) = f(XSigma(k-1), u(k-1))                                  ...{UKF_5a}  */
        /* x(k|k-1) = sum(Wm(i) * XSigma(k)(i))    ; i = 1 ... (2N+1)          ...{UKF_6a}  */
        _Matrix AuxSigma1 = _Matrix.Homogen(XSigma.rows, 1);
        _Matrix AuxSigma2 = _Matrix.Homogen(XSigma.rows, 1);
        _Matrix newXSigma = _Matrix.Homogen(XSigma.rows, XSigma.cols);
        _Matrix newState =_Matrix.Homogen(4, 1, 0f);

        for (int j = 0; j < XSigma.cols; j++) {
            AuxSigma1 = _Matrix.Homogen(XSigma.rows, 1);
            AuxSigma2 = _Matrix.Homogen(XSigma.rows, 1);
            for (int i = 0; i < XSigma.rows; i++) {
                AuxSigma1[i, 0] = XSigma[i,j];
            }


           // AuxSigma2 = UpdateNonlinearX(AuxSigma2, AuxSigma1, InpVector, Time.deltaTime);
            AuxSigma2 = f(AuxSigma1, angularVelocity, Time.deltaTime); 
            for (int k = 0; k < AuxSigma2.rows; k++){
                newXSigma[k, j] = AuxSigma2[k, 0];
            }
            AuxSigma2 *= Wm[0, j];
            newState += AuxSigma2;
        }


        /* DX = XSigma(k)(i) - Xs(k)   ; Xs(k) = [x(k|k-1) ... x(k|k-1)]
        *                             ; Xs(k) = Nx(2N+1)                      ...{UKF_7a}  */

        AuxSigma1 = _Matrix.Homogen(newXSigma.rows, newXSigma.cols);
        for (int j = 0; j < newXSigma.cols; j++) {
            for (int k = 0; k < AuxSigma2.rows; k++){
                AuxSigma1[k, j] = newState[k, 0];
            }

        }

        DX  = newXSigma - AuxSigma1;
        AuxSigma1 = new _Matrix(DX);
        for (int i = 0; i < DX.rows; i++) {
            for (int j = 0; j < DX.cols; j++) {
                AuxSigma1[i, j] *= Wc[0, j];
            }
        }

        P = (AuxSigma1 * DX.T) + _CovNoise;
        X = new _Quaternion(newState.matrix).normalized;
        XSigma = newXSigma;
    }




    private _Matrix f(_Matrix q, Vector3 w, float dt){
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

    // _Matrix UpdateNonlinearX(_Matrix X_Next, _Matrix X, _Matrix U, float dt)
    // {
    //     /* Insert the nonlinear update transformation here
    //     *          x(k+1) = f[x(k), u(k)]
    //     *
    //     * The quaternion update function:
    //     *  q0_dot = 1/2. * (  0   - p*q1 - q*q2 - r*q3)
    //     *  q1_dot = 1/2. * ( p*q0 +   0  + r*q2 - q*q3)
    //     *  q2_dot = 1/2. * ( q*q0 - r*q1 +  0   + p*q3)
    //     *  q3_dot = 1/2. * ( r*q0 + q*q1 - p*q2 +  0  )
    //     * 
    //     * Euler method for integration:
    //     *  q0 = q0 + q0_dot * dT;
    //     *  q1 = q1 + q1_dot * dT;
    //     *  q2 = q2 + q2_dot * dT;
    //     *  q3 = q3 + q3_dot * dT;
    //     */
    //     float q0, q1, q2, q3;
    //     float p, q, r;
        
    //     q0 = X[0, 0];
    //     q1 = X[1, 0];
    //     q2 = X[2, 0];
    //     q3 = X[3, 0];
        
    //     p = U[0, 0];
    //     q = U[1, 0];
    //     r = U[2, 0];
        
    //     X_Next[0, 0] = (0.5f * (+0f -p*q1 -q*q2 -r*q3))*dt + q0;
    //     X_Next[1, 0] = (0.5f * (+p*q0 +0f +r*q2 -q*q3))*dt + q1;
    //     X_Next[2, 0] = (0.5f * (+q*q0 -r*q1 +0f +p*q3))*dt + q2;
    //     X_Next[3, 0] = (0.5f * (+r*q0 +q*q1 -p*q2 +0f))*dt + q3;
    //     //float norm = Mathf.Sqrt(X_Next[0, 0]*X_Next[0, 0] + X_Next[1, 0]*X_Next[1, 0] + X_Next[2, 0]*X_Next[2, 0] + X_Next[3, 0]*X_Next[3, 0]);
    //     //log += $"norm: \n{norm}\n\n"; 
    //     return X_Next;
        
    // }


    private void UnscentedTransformY(_Matrix InpVector, _Matrix _CovNoise){

        /* XSigma(k) = f(XSigma(k-1), u(k-1))                                  ...{UKF_5a}  */
        /* x(k|k-1) = sum(Wm(i) * XSigma(k)(i))    ; i = 1 ... (2N+1)          ...{UKF_6a}  */
        _Matrix AuxSigma1 = _Matrix.Homogen(XSigma.rows, 1);
        _Matrix AuxSigma2 = _Matrix.Homogen(6, 1);


        _Matrix newSigmaY = _Matrix.Homogen(6, 9);


        _Matrix newStateY =_Matrix.Homogen(6, 1, 0f);
        for (int j = 0; j < XSigma.cols; j++) {
            AuxSigma1 = _Matrix.Homogen(XSigma.rows, 1);
            AuxSigma2 = _Matrix.Homogen(6, 1);
            for (int i = 0; i < XSigma.rows; i++) {
                AuxSigma1[i, 0] = XSigma[i,j];
            }

            //AuxSigma2 = UpdateNonlinearY(AuxSigma2, AuxSigma1, InpVector, Time.deltaTime);
            AuxSigma2 = h(new _Quaternion(AuxSigma1.matrix));

            for (int k = 0; k < AuxSigma2.rows; k++){
                newSigmaY[k, j] = AuxSigma2[k, 0];
            }
            AuxSigma2 *= Wm[0, j];
            newStateY += AuxSigma2;
        }


        /* DX = XSigma(k)(i) - Xs(k)   ; Xs(k) = [x(k|k-1) ... x(k|k-1)]
        *                             ; Xs(k) = Nx(2N+1)                      ...{UKF_7a}  */

        AuxSigma1 = _Matrix.Homogen(newSigmaY.rows, newSigmaY.cols);
        for (int j = 0; j < newSigmaY.cols; j++) {
            for (int k = 0; k < AuxSigma2.rows; k++){
                AuxSigma1[k, j] = newStateY[k, 0];
            }

        }
        DY  = newSigmaY - AuxSigma1;

        AuxSigma1 = new _Matrix(DY);
        for (int i = 0; i < DY.rows; i++) {
            for (int j = 0; j < DY.cols; j++) {
                AuxSigma1[i, j] *= Wc[0, j];
            }
        }
        PY = (AuxSigma1 * DY.T) + _CovNoise;

        Y = newStateY;
        YSigma = newSigmaY;
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
            {g.x*(qwqy + qxqz) + g.y*(qyqz - qwqx) + g.z*(0.5f - qx2 - qy2)},
            {r.x*(0.5f - qy2 - qz2) + r.y*(qwqz + qxqy) + r.z*(qxqz - qwqy)},
            {r.x*(qxqy - qwqz) + r.y*(0.5f - qx2 - qz2) + r.z*(qwqx + qyqz)},
            {r.x*(qwqy + qxqz) + r.y*(qyqz - qwqx) + r.z*(0.5f - qx2 - qy2)}
        };
        return 2 * new _Matrix(result);
    }




    // _Matrix UpdateNonlinearY(_Matrix Y, _Matrix X, _Matrix U, float dt){
    //     /* Insert the nonlinear measurement transformation here
    //     *          y(k)   = h[x(k), u(k)]
    //     *
    //     * The measurement output is the gravitational and magnetic projection to the body
    //     *     DCM     = [(+(q0**2)+(q1**2)-(q2**2)-(q3**2)),                        2*(q1*q2+q0*q3),                        2*(q1*q3-q0*q2)]
    //     *               [                   2*(q1*q2-q0*q3),     (+(q0**2)-(q1**2)+(q2**2)-(q3**2)),                        2*(q2*q3+q0*q1)]
    //     *               [                   2*(q1*q3+q0*q2),                        2*(q2*q3-q0*q1),     (+(q0**2)-(q1**2)-(q2**2)+(q3**2))]
    //     * 
    //     *  G_proj_sens = DCM * [0 0 1]             --> Gravitational projection to the accelerometer sensor
    //     *  M_proj_sens = DCM * [Mx My Mz]          --> (Earth) magnetic projection to the magnetometer sensor
    //     */
    //     float q0, q1, q2, q3;
    //     float q0_2, q1_2, q2_2, q3_2;

    //     float IMU_ACC_Z0 = 1f;
    //     float[,] IMU_MAG_B0= new float[,]{{Mathf.Cos(0)}, {Mathf.Sin(0)}, {0f}};

    //     q0 = X[0, 0];
    //     q1 = X[1, 0];
    //     q2 = X[2, 0];
    //     q3 = X[3, 0];

    //     q0_2 = q0 * q0;
    //     q1_2 = q1 * q1;
    //     q2_2 = q2 * q2;
    //     q3_2 = q3 * q3;
        
    //     Y[0, 0] = (2*q1*q3 -2*q0*q2) * IMU_ACC_Z0;

    //     Y[1, 0] = (2*q2*q3 +2*q0*q1) * IMU_ACC_Z0;

    //     Y[2, 0] = (+(q0_2) -(q1_2) -(q2_2) +(q3_2)) * IMU_ACC_Z0;
        
    //     Y[3, 0] = (+(q0_2)+(q1_2)-(q2_2)-(q3_2)) * IMU_MAG_B0[0, 0]
    //             +(2*(q1*q2+q0*q3)) * IMU_MAG_B0[1, 0]
    //             +(2*(q1*q3-q0*q2)) * IMU_MAG_B0[2, 0];

    //     Y[4, 0] = (2*(q1*q2-q0*q3)) * IMU_MAG_B0[0, 0]
    //             +(+(q0_2)-(q1_2)+(q2_2)-(q3_2)) * IMU_MAG_B0[1, 0]
    //             +(2*(q2*q3+q0*q1)) * IMU_MAG_B0[2, 0];

    //     Y[5, 0] = (2*(q1*q3+q0*q2)) * IMU_MAG_B0[0, 0]
    //             +(2*(q2*q3-q0*q1)) * IMU_MAG_B0[1, 0]
    //             +(+(q0_2)-(q1_2)-(q2_2)+(q3_2)) * IMU_MAG_B0[2, 0];

    //     return Y;
    // }

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

}