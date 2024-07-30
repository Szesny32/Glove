using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataLogger : MonoBehaviour
{
    private string filePath;
    private StreamWriter writer;
    public GyroSim gyroscope;
    public AccSim accelerometer;
    public MagSim magnetometer;
    public bool logToFile = false;
    void Start(){
        if(logToFile){
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            filePath = $"C:/Users/Szesny/Desktop/Magisterka - playground/Record_{currentDate}.csv";
            writer = new StreamWriter(filePath, true);
            if (new FileInfo(filePath).Length == 0) {
                writer.WriteLine("dt;qw;qx;qy;qz;gyroX;gyroY;gyroZ;accX;accY;accZ;magX;magY;magZ;");
            }
        }
    }

    void OnApplicationQuit(){
        if(writer!=null){
            writer.Close();
        }
    }

    void Update(){
        if(logToFile)
            LogToFile();
    }

    private void LogToFile(){

        Quaternion rotation = transform.rotation;
        Vector3 angularVelocity = gyroscope.Read();
        Vector3 acceleration = accelerometer.Read();
        Vector3 magneticPole = magnetometer.Read();

        string row = $"{Time.deltaTime};";
        row += $"{rotation.w};{rotation.x};{rotation.y};{rotation.z};";
        row += $"{angularVelocity.x};{angularVelocity.y};{angularVelocity.z};";
        row += $"{acceleration.x};{acceleration.y};{acceleration.z};";
        row += $"{magneticPole.x};{magneticPole.y};{magneticPole.z};";

        row = row.Replace(',', '.');
        writer.WriteLine(row);
    }


}
