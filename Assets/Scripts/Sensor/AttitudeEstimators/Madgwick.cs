using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Madgwick : AttitudeEstimator
{

    //https://ahrs.readthedocs.io/en/latest/filters/madgwick.html
    //Orientation as solution of Gradient Descent

    //METHOD: analytically derived and optimised gradient-descent algorithm 
    //SUBSTRATE: accelerometer and magnetometer data 
    //PRODUCT: direction of the gyroscope measurement error 
    //UNIT: quaternion derivative

    //A single adjustable parameter defined by observable systems characteristics
    //An analytically derived and optimised gradient-descent algorithm enabling performance at low sampling rates. <----- Low sampling problem(?)

    //earth frame eD = [0, dx, dy, dz]
    //corresponding measured direction in the sensor frame sS = [0, sx, sy, sz]

    //Gradient Descent Algorithm?

    _Quaternion Q;
    //estimated mean zero gyroscope measurement error of each axis

    float beta;  //TODO
    //_Matrix beta;


    public override void Init(){

        //Q = new _Quaternion(transform.rotation);
        Q = new _Quaternion(1, 0, 0, 0);

        float ex = Mathf.Sqrt(gyroscopeNoise.x);
        float ey = Mathf.Sqrt(gyroscopeNoise.y);
        float ez = Mathf.Sqrt(gyroscopeNoise.z);

        float omega_beta = Mathf.Sqrt(gyroscopeNoise.x + gyroscopeNoise.y + gyroscopeNoise.z);
        beta = Mathf.Sqrt(3f / 4f) * omega_beta;

        //LEPSZY BEZ SZUMU
        //  beta = Mathf.Sqrt(0.75f) * new _Matrix(new float[,]{
        //     {Mathf.Sqrt(gyroscopeNoise.x + gyroscopeNoise.y + gyroscopeNoise.z), 0, 0, 0},
        //     {0, ex, 0, 0},
        //     {0, 0, ey, 0},
        //     {0, 0, 0, ez}
        // });

       


        //beta = Mathf.Sqrt(0.75f) * Mathf.Sqrt(gyroscopeNoise.x + gyroscopeNoise.y + gyroscopeNoise.z);
           
    }

    public override void UpdateOrientation(){
        float dt = Time.deltaTime;


        _Quaternion Qw = 0.5f * Omega(angularVelocity)*Q;

        Vector3 g = Vector3.up;
        Vector3 a = acceleration;

        Vector3 m = magneticField;
        _Matrix _m = new _Matrix(new float[,]{{m.x}, {m.y}, {m.z}});
        
        _Matrix h = Q.toDirectionCosineMatrix().T *_m;
        float hx = h[0,0], hy = h[1,0], hz = h[2,0];
        Vector3 b = new Vector3(0, hy, Mathf.Sqrt(hx*hx + hz*hz));

        _Matrix fg = f(Q, g, a);
        _Matrix fb = f(Q, b, m);
        _Matrix fgb = _Matrix.StackByRows(fg, fb);

        _Matrix Jg = Jacobian(Q, g);
        _Matrix Jb = Jacobian(Q, b);
        _Matrix Jgb = _Matrix.StackByRows(Jg, Jb);

        Q += (Qw - beta*Gradient(Jgb, fgb)) * dt;
        
        transform.rotation = Q.Unity();
    }
    
    private _Quaternion Gradient(_Matrix Jgb, _Matrix fgb){
        _Quaternion gradient = (Jgb.T * fgb).toQuaternion().normalized;
        return gradient;
    }

    private _Matrix f(_Quaternion q, Vector3 d, Vector3 s){
        float [,] result = new float[,]{ 
            {d.x*(0.5f - q.y*q.y - q.z*q.z) + d.y*(q.w*q.z + q.x*q.y) + d.z*(q.x*q.z - q.w*q.y)},
            {d.x*(q.x*q.y - q.w*q.z) + d.y*(0.5f - q.x*q.x - q.z*q.z) + d.z*(q.w*q.x + q.y*q.z)},
            {d.x*(q.w*q.y + q.x*q.z) + d.y*(q.y*q.z - q.w*q.x) + d.z*(0.5f - q.x*q.x - q.y*q.y)}
        };
        return 2f * new _Matrix(result) - new _Matrix(new float[,]{{s.x},{s.y},{s.z}});
    }   

    private _Matrix Jacobian(_Quaternion q, Vector3 d){
         float [,] result = new float[,]{ 
            {d.x*q.w  +d.y*q.z   -d.z*q.y,       d.x*q.x  +d.y*q.y  +d.z*q.z,        -d.x*q.y  +d.y*q.x  -d.z*q.w,        -d.x*q.z  +d.y*q.w  +d.z*q.x},
            {-d.x*q.z   +d.y*q.w   +d.z*q.x,       d.x*q.y  -d.y*q.x  +d.z*q.w,        d.x*q.x  +d.y*q.y  +d.z*q.z,        -d.x*q.w  -d.y*q.z  +d.z*q.y},
            {d.x*q.y   -d.y*q.x   +d.z*q.w,       d.x*q.z  -d.y*q.w  -d.z*q.x,        d.x*q.w  +d.y*q.z  -d.z*q.y,        d.x*q.x  +d.y*q.y  +d.z*q.z}
       };
        return 2f * new _Matrix(result);
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
