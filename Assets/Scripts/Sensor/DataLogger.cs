using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataLogger : MonoBehaviour
{
    private string filePath;
    private StreamWriter writer;
    public GyroSim gyroscope;
    public AccSim acceleromter;
    public MagSim magnetometer;
    public IMU imu;
    public bool logToFile = false;
    void Start(){
        filePath = "C:/Users/Szesny/Desktop/Magisterka - playground/Sensor.csv";
        writer = new StreamWriter(filePath, true);
        if (new FileInfo(filePath).Length == 0) {
            writer.WriteLine("dt;angleX;angleY;angleZ;gyroX;gyroY;gyroZ;gyroX_gt;gyroY_gt;gyroZ_gt;accX;accY;accZ;accX_gt;accY_gt;accZ_gt;magX;magY;magZ;estimatedX;estimatedY;estimatedZ");
        }
    }

    void OnApplicationQuit(){
        writer.Close();
    }

    void Update(){
        if(logToFile)
            LogToFile();
    }

    private void LogToFile(){
        Vector3 groundTrueAngle = transform.eulerAngles;
        Vector3 angularVelocity = gyroscope.Read();
        Vector3 angularVelocity_gt = gyroscope.GetGroundTrue();

        Vector3 acceleration = acceleromter.Read();
        Vector3 acceleration_gt = acceleromter.GetGroundTrue();

        Vector3 magneticPole = magnetometer.Read();

        Vector3 estimated = imu.GetAngle();

        string row = $"{Time.deltaTime};";
        row += $"{groundTrueAngle.x};{groundTrueAngle.y};{groundTrueAngle.z};";
        row += $"{angularVelocity.x};{angularVelocity.y};{angularVelocity.z};";
        row += $"{angularVelocity_gt.x};{angularVelocity_gt.y};{angularVelocity_gt.z};";
        row += $"{acceleration.x};{acceleration.y};{acceleration.z};";
        row += $"{acceleration_gt.x};{acceleration_gt.y};{acceleration_gt.z};";
        row += $"{magneticPole.x};{magneticPole.y};{magneticPole.z};";
        row += $"{estimated.x};{estimated.y};{estimated.z}";
        writer.WriteLine(row);
    }


}
