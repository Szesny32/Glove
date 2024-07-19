using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SensorMode
{
    GYROSCOPE,
    ACCELEROMETER,
    ACC_AND_MAG,
    COMPLEMENTARY_FILTER,
    KALMAN_FILTER
}

public class IMU : MonoBehaviour
{
    public Transform referenceObj;
    public GyroSim gyroscope;
    public AccSim acceleromter;
    public MagSim magnetometer;
    public SensorMode mode;
    float g = 9.8067f;
    float alpha = 0.98f;

    public Material correctMaterial;
    public Material incorrectMaterial;
    private Renderer renderer;
    public float acceptedDiff = 1f;

    
    private float frequency = 100f; //Hz
    private float samplingTime = 0.01f; // 1/Hz
    private float timer = 0f;

    private bool isOk = false;
    public bool removeBias = false;

    private Vector3 previous_states = Vector3.zero;
    private Vector3 previous_process_covariances = Vector3.zero;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.material = incorrectMaterial;
    }

    
    void Update()
    {
        // timer+=Time.deltaTime;
        // if(timer >= samplingTime){
        //     UpdateOrientation();
        //     timer = 0.0f;
        // }

        UpdateOrientation();
        Verify();
    }


    void UpdateOrientation(){
        if(mode == SensorMode.GYROSCOPE){
            GyroscopeMode();
        }
        else if(mode == SensorMode.ACCELEROMETER){
            AcceleromterMode();
        }
        else if(mode == SensorMode.ACC_AND_MAG){
            AccMagMode();
        }
        else if(mode == SensorMode.COMPLEMENTARY_FILTER){
            ComplementaryMode();
        } 
        else if(mode == SensorMode.KALMAN_FILTER){
            KalmanFilterMode();
        }

    }

    void Verify(){
        (float refFixed_X,  float gyroFixed_X) = fixAngles(referenceObj.eulerAngles.x, transform.eulerAngles.x);
        (float refFixed_Y,  float gyroFixed_Y) = fixAngles(referenceObj.eulerAngles.y, transform.eulerAngles.y);
        (float refFixed_Z,  float gyroFixed_Z) = fixAngles(referenceObj.eulerAngles.z, transform.eulerAngles.z);

        float deltaX = Mathf.Abs(refFixed_X - gyroFixed_X);
        float deltaY = Mathf.Abs(refFixed_Y - gyroFixed_Y);
        float deltaZ = Mathf.Abs(refFixed_Z - gyroFixed_Z);
        if((deltaX < acceptedDiff) && (deltaY < acceptedDiff) && (deltaZ < acceptedDiff)){
            if(isOk==false){
                isOk= true;
                renderer.material = correctMaterial;
            }

        } else {
            if(isOk==true){
                isOk = false;
                renderer.material = incorrectMaterial;
            }
            
        }


    }

    private (float, float) fixAngles(float angle1, float angle2) {
        if(Mathf.Abs(angle1 - angle2) > 180){
            if(angle1 > angle2)
                angle2 += 360;
            else
                angle1 += 360;
        }
        return (angle1, angle2);
    }
    


    private void GyroscopeMode(){
        Vector3 bias = removeBias? gyroscope.GetBias() : Vector3.zero;
        Vector3 angularVelocity = gyroscope.Read();
        Vector3 deltaRotation = (angularVelocity * Time.deltaTime - bias) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(transform.eulerAngles + deltaRotation);
    }

    private void AcceleromterMode(){
        Vector3 bias = removeBias? acceleromter.GetBias() : Vector3.zero;
        Vector3 acceleration = (acceleromter.Read() - bias) / g;
        Vector3 eulerAngles = Vector3.zero;
        eulerAngles.x = ( -Mathf.Atan2(acceleration.z, Mathf.Sqrt(Mathf.Pow(acceleration.x, 2) + Mathf.Pow(acceleration.y, 2))) * Mathf.Rad2Deg + 360f) % 360f;  
        eulerAngles.z = (Mathf.Atan2 (acceleration.x, acceleration.y) * Mathf.Rad2Deg+ 360f) % 360f;
        transform.rotation = Quaternion.Euler(eulerAngles);
    }

    private void AccMagMode(){
        Vector3 accBias = removeBias? acceleromter.GetBias() : Vector3.zero;

        //ACCELEROMTER
        Vector3 acceleration = (acceleromter.Read() - accBias) / g;
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
        Vector3 gyroBias = removeBias? gyroscope.GetBias() : Vector3.zero;
        Vector3 accBias = removeBias? acceleromter.GetBias() : Vector3.zero;

        Vector3 angularVelocity = gyroscope.Read();
        Vector3 deltaRotation = (angularVelocity * Time.deltaTime - gyroBias) * Mathf.Rad2Deg;
        Vector3 gyro = transform.eulerAngles + deltaRotation;

        Vector3 acceleration = (acceleromter.Read() - accBias) / g;
        Vector3 eulerAngles = Vector3.zero;
        eulerAngles.x = ( -Mathf.Atan2(acceleration.z, Mathf.Sqrt(Mathf.Pow(acceleration.x, 2) + Mathf.Pow(acceleration.y, 2))) * Mathf.Rad2Deg + 360f) % 360f;  
        eulerAngles.z = (Mathf.Atan2 (acceleration.x, acceleration.y) * Mathf.Rad2Deg+ 360f) % 360f;
        
        //MAGNETOMETER
        Vector3 magneticPole = magnetometer.Read();
        Vector3 v = xRot(eulerAngles.x, zRot(eulerAngles.z, magnetometer.GetOrigin()));
        float y = v.x * magneticPole.z - v.z * magneticPole.x;
        float x = v.x * magneticPole.x + v.z * magneticPole.z;
        eulerAngles.y = (-Mathf.Atan2(y, x) * Mathf.Rad2Deg + 360f) % 360f; //y was down

        transform.rotation = Quaternion.Euler(new Vector3(
            InterpolateAngle((gyro.x) % 360, eulerAngles.x, alpha),
            InterpolateAngle((gyro.y) % 360, eulerAngles.y, alpha),
            InterpolateAngle((gyro.z) % 360, eulerAngles.z, alpha)
        ));



        //Quaternion deltaQuaternion = Quaternion.Euler(deltaRotation * Mathf.Rad2Deg);
        //Quaternion gyroOrientation = deltaQuaternion * transform.rotation;
        //Quaternion accMagOrientation = Quaternion.Euler(eulerAngles);
        //transform.rotation = Quaternion.Lerp(gyroOrientation, accMagOrientation, 1-alpha);
    }

    private float InterpolateAngle(float angle1, float angle2, float ratio){
        if(Mathf.Abs(angle1 - angle2) > 180){
            if(angle1 > angle2)
                angle2 += 360;
            else
                angle1 += 360;
        }
        return (ratio*angle1 + (1-ratio)*angle2) % 360;
    }

    private void KalmanFilterMode(){
        Vector3 gyroBias = removeBias? gyroscope.GetBias() : Vector3.zero;
        Vector3 accBias = removeBias? acceleromter.GetBias() : Vector3.zero;
        Vector3 gyroNoise = gyroscope.GetNoise();
        Vector3 accNoise = acceleromter.GetNoise();

        Vector3 angularVelocity = gyroscope.Read();
        Vector3 deltaRotation = (angularVelocity * Time.deltaTime - gyroBias) * Mathf.Rad2Deg; 

        Vector3 acceleration = (acceleromter.Read() - accBias) / g;
        Vector3 eulerAngles = Vector3.zero;
        eulerAngles.x = ( -Mathf.Atan2(acceleration.z, Mathf.Sqrt(Mathf.Pow(acceleration.x, 2) + Mathf.Pow(acceleration.y, 2))) * Mathf.Rad2Deg + 360f) % 360f;  
        eulerAngles.z = (Mathf.Atan2 (acceleration.x, acceleration.y) * Mathf.Rad2Deg+ 360f) % 360f;
        
        //MAGNETOMETER
        Vector3 magneticPole = magnetometer.Read();
        Vector3 v = xRot(eulerAngles.x, zRot(eulerAngles.z, magnetometer.GetOrigin()));
        float y = v.x * magneticPole.z - v.z * magneticPole.x;
        float x = v.x * magneticPole.x + v.z * magneticPole.z;
        eulerAngles.y = (-Mathf.Atan2(y, x) * Mathf.Rad2Deg + 360f) % 360f; //y was down

        // noise w żyro
        Vector3 Q_angle =  new Vector3(
            Mathf.Pow(gyroNoise.x/Time.deltaTime, 2),
            Mathf.Pow(gyroNoise.y/Time.deltaTime, 2),
            Mathf.Pow(gyroNoise.z/Time.deltaTime, 2)
        );  
        
        // noise w acc
        Vector3 R =  new Vector3(
            Mathf.Pow(accNoise.x, 2),
            Mathf.Pow(accNoise.x, 2),
            Mathf.Pow(accNoise.z, 2)
        );   


        if(Q_angle.x == 0 && Q_angle.y == 0 && Q_angle.z == 0){
            previous_states = eulerAngles;
        } else if (R.x == 0 && R.y == 0 && R.z == 0){
            previous_states = transform.eulerAngles + deltaRotation;
        } else {
            (float previous_states_x, float previous_process_covariances_x) = KalmanFilter(previous_states.x, previous_process_covariances.x, deltaRotation.x, eulerAngles.x, Q_angle.x, R.x);
            (float previous_states_y, float previous_process_covariances_y) = KalmanFilter(previous_states.y, previous_process_covariances.y, deltaRotation.y, eulerAngles.y, Q_angle.y, R.y);
            (float previous_states_z, float previous_process_covariances_z) = KalmanFilter(previous_states.z, previous_process_covariances.z, deltaRotation.z, eulerAngles.z, Q_angle.z, R.z);
            previous_process_covariances = new Vector3(previous_process_covariances_x, previous_process_covariances_y, previous_process_covariances_z);
            previous_states = new Vector3(previous_states_x, previous_states_y, previous_states_z);
        }

        

        
        
        transform.rotation = Quaternion.Euler(previous_states);
    }


    private (float, float) KalmanFilter(float previous_state, float previous_process_covariance, float gyroDelta, float accAngle, float Q_angle, float R){
        //Step 1: THE PREDICTED STATE
        float predicted_state = previous_state + gyroDelta;

        //Step 2: THE PREDICTED PROCESS COVIARIANCE MATRIX
        float predicted_process_covariance = previous_process_covariance + Q_angle;

        //Step 3: CALCULATING THE KALMAN GAIN
        float K = predicted_process_covariance / (predicted_process_covariance + R);
        //Step 5: CALCULATING THE CURRENT STATE
        float a = accAngle;
        if(Mathf.Abs(a - predicted_state) > 180.0f) {
            if(a > predicted_state){
                predicted_state += 360.0f;
            } else {
                a += 360.0f;
            }
        }
            
        float state = (predicted_state + K *(a - predicted_state)) % 360.0f;

        //Step 6: UPDATING THE PROCESS COVARIANCE MATRIX
        float process_covariance =  (1 - K) * predicted_process_covariance;

        return (state, process_covariance);
    }

    public Vector3 GetAngle(){
        return transform.eulerAngles;
    }
}
