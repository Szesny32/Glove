using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SensorMode
{
    GYROSCOPE,
    ACCELEROMETER,
    ACC_AND_MAG,
    COMPLEMENTARY_FILTER
}

public class IMU : MonoBehaviour
{
    public GyroSim gyroscope;
    public AccSim acceleromter;
    public MagSim magnetometer;
    public SensorMode mode;
    float g = 9.8067f;
    float alpha = 0.98f;

    void Start()
    {
        
    }

    
    void Update()
    {
        if(mode == SensorMode.GYROSCOPE)
            GyroscopeMode();
        else if(mode == SensorMode.ACCELEROMETER){
            AcceleromterMode();
        }
        else if(mode == SensorMode.ACC_AND_MAG){
            AccMagMode();
        }
        else if(mode == SensorMode.COMPLEMENTARY_FILTER){
            ComplementaryMode();
        }
    }

    private void GyroscopeMode(){
        Vector3 angularVelocity = gyroscope.Read();
        Vector3 deltaRotation = angularVelocity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(transform.eulerAngles + deltaRotation* Mathf.Rad2Deg);
    }

    private void AcceleromterMode(){
        Vector3 acceleration = acceleromter.Read() / g;
        Vector3 eulerAngles = Vector3.zero;
        eulerAngles.x = ( -Mathf.Atan2(acceleration.z, Mathf.Sqrt(Mathf.Pow(acceleration.x, 2) + Mathf.Pow(acceleration.y, 2))) * Mathf.Rad2Deg + 360f) % 360f;  
        eulerAngles.z = (Mathf.Atan2 (acceleration.x, acceleration.y) * Mathf.Rad2Deg+ 360f) % 360f;
        transform.rotation = Quaternion.Euler(eulerAngles);
    }

    private void AccMagMode(){
        //ACCELEROMTER
        Vector3 acceleration = acceleromter.Read() / g;
        Vector3 eulerAngles = Vector3.zero;
        eulerAngles.x = ( -Mathf.Atan2(acceleration.z, Mathf.Sqrt(Mathf.Pow(acceleration.x, 2) + Mathf.Pow(acceleration.y, 2))) * Mathf.Rad2Deg + 360f) % 360f;  
        eulerAngles.z = (Mathf.Atan2 (acceleration.x, acceleration.y) * Mathf.Rad2Deg+ 360f) % 360f;
        
        //MAGNETOMETER
        Vector3 magneticPole = magnetometer.Read();
        Vector3 v = xRot(eulerAngles.x, zRot(eulerAngles.z, magnetometer.GetOrigin()));
        float y = v.x * magneticPole.z - v.z * magneticPole.x;
        float x = v.x * magneticPole.x + v.z * magneticPole.z;
        eulerAngles.y = (-Mathf.Atan2(y, x) * Mathf.Rad2Deg + 360f) % 360f; //y was down

        transform.rotation = Quaternion.Euler(eulerAngles);
    }

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

    private void ComplementaryMode(){

        Vector3 angularVelocity = gyroscope.Read();
        Vector3 deltaRotation = angularVelocity * Time.deltaTime;
        Quaternion deltaQuaternion = Quaternion.Euler(deltaRotation * Mathf.Rad2Deg);
        Quaternion gyroOrientation = deltaQuaternion * transform.rotation;

        Vector3 acceleration = acceleromter.Read() / g;
        Vector3 eulerAngles = Vector3.zero;
        eulerAngles.x = ( -Mathf.Atan2(acceleration.z, Mathf.Sqrt(Mathf.Pow(acceleration.x, 2) + Mathf.Pow(acceleration.y, 2))) * Mathf.Rad2Deg + 360f) % 360f;  
        eulerAngles.z = (Mathf.Atan2 (acceleration.x, acceleration.y) * Mathf.Rad2Deg+ 360f) % 360f;
        
        //MAGNETOMETER
        Vector3 magneticPole = magnetometer.Read();
        Vector3 v = xRot(eulerAngles.x, zRot(eulerAngles.z, magnetometer.GetOrigin()));
        float y = v.x * magneticPole.z - v.z * magneticPole.x;
        float x = v.x * magneticPole.x + v.z * magneticPole.z;
        eulerAngles.y = (-Mathf.Atan2(y, x) * Mathf.Rad2Deg + 360f) % 360f; //y was down

        Quaternion accMagOrientation = Quaternion.Euler(eulerAngles);

        transform.rotation = Quaternion.Lerp(gyroOrientation, accMagOrientation, 1-alpha);
    }

}
