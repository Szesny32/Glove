using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Unity.VisualScripting;

public class DataLogger : MonoBehaviour
{
    private string filePath;
    private StreamWriter writer;
    //public GyroSim gyroscope;
    //public AccSim accelerometer;
    //public MagSim magnetometer;

    public bool logToFile = false;
    // public AttitudeEstimator quat;
    // public AttitudeEstimator t1;
    // public AttitudeEstimator t3;
    // public AttitudeEstimator t100;
    //public AttitudeEstimator eCompass;
    //public AttitudeEstimator AQUA;
    // public AttitudeEstimator complementary;
    // public AttitudeEstimator slerp;

    // public AttitudeEstimator complementary90;
    // public AttitudeEstimator complementary92;
    // public AttitudeEstimator complementary94;
    // public AttitudeEstimator complementary96;
    // public AttitudeEstimator complementary98;
    // public AttitudeEstimator complementary99;

    public AttitudeEstimator c96;
    public AttitudeEstimator EKF;
    public AttitudeEstimator UKF;
    public AttitudeEstimator Madgwick;


    void Start(){
        if(logToFile){
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            //filePath = $"C:/Users/Szesny/Desktop/AttitudeAngularRate_1.csv";
            //filePath = $"C:/Users/Szesny/Desktop/AQUA_vs_eCompass_1.csv";
            //filePath = $"C:/Users/Szesny/Desktop/Complementary_vs_slerp_1.csv";
            //filePath = $"C:/Users/Szesny/Desktop/Complementary_param2.csv";
            filePath = $"C:/Users/Szesny/Desktop/EKF_UKF_Madgwick_C96_stress_test_1h.csv";
       
       

            writer = new StreamWriter(filePath, true);
            if (new FileInfo(filePath).Length == 0) {
                //writer.WriteLine("dt;QuatDt;T1Dt;T3Dt;T100Dt;QuatADiff;T1ADiff;T3ADiff;T100ADiff;QuatMTime;T1MTime;T3MTime;T100MTime;");
                //writer.WriteLine("dt;eCompassADiff;AQUAADiff;");
                //writer.WriteLine("dt;complementaryADiff;slerpADiff;");
                //writer.WriteLine("dt;c90;c92;c94;c96;c98;c99;c90_s;c92_s;c94_s;c96_s;c98_s;c99_s;");
                writer.WriteLine("dt;c96;Madgwick;EKF;UKF;c96_s;Madgwick_s;EKF_s;UKF_s;");
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
        string row = $"{Time.deltaTime};";
        // row += $"{quat.updateDuration};{t1.updateDuration};{t3.updateDuration};{t100.updateDuration};";
        // row += $"{quat.angleDifference};{t1.angleDifference};{t3.angleDifference};{t100.angleDifference};";
        // row += $"{quat.RotationMatchTimeIndicator()};{t1.RotationMatchTimeIndicator()};{t3.RotationMatchTimeIndicator()};{t100.RotationMatchTimeIndicator()};";
        // row += $"{complementary90.angleDifference};{complementary92.angleDifference};{complementary94.angleDifference};{complementary96.angleDifference};{complementary98.angleDifference};{complementary99.angleDifference};";
        // row += $"{complementary90.smoothingAngle};{complementary92.smoothingAngle};{complementary94.smoothingAngle};{complementary96.smoothingAngle};{complementary98.smoothingAngle};{complementary99.smoothingAngle};";
        
        row += $"{c96.angleDifference};{Madgwick.angleDifference};{EKF.angleDifference};{UKF.angleDifference};";
        row += $"{c96.smoothingAngle};{Madgwick.smoothingAngle};{EKF.smoothingAngle};{UKF.smoothingAngle};";

        row = row.Replace(',', '.');
        writer.WriteLine(row);
    }


}
