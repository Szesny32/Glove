using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Diagnostics;

public class AttitudeEstimator : MonoBehaviour
{
    public Transform reference;

    public GyroSim gyroscope;
    protected Vector3 angularVelocity;
    protected Vector3 gyroscopeBias;
    public Vector3 gyroscopeNoise;

    public AccSim acceleromter;
    protected Vector3 acceleration;
    protected Vector3 acceleromterBias;
    public Vector3 acceleromterNoise;

    public MagSim magnetometer;
    protected Vector3 magneticField;
    public Vector3 magnetometerNoise;
    
    protected Renderer renderer;
    private Material correctMaterial;
    private Material incorrectMaterial;

    public float angleDifference;
    public float smoothingAngle;
    protected Quaternion prevRot;

    public float angleDifferenceTotal = 0f;
    private float angleThreshold = 10f;
    protected bool rotationMatch = false;
    protected float rotationMatchTime = 0f;

    public bool removeBiasMode = false;

    public bool autoMode = true;

    protected float dt = 0f;

    public float updateDuration = 0f;


    
    void Start()
    {

        correctMaterial = Resources.Load<Material>("Correct");
        incorrectMaterial = Resources.Load<Material>("Incorrect");

        renderer = GetComponent<Renderer>();
        if(renderer!=null)
            renderer.material = incorrectMaterial;
        gyroscopeBias = removeBiasMode? gyroscope.GetBias() : Vector3.zero;
        acceleromterBias = removeBiasMode? acceleromter.GetBias() : Vector3.zero;
        transform.rotation = reference.rotation;

        //[TODO]
        gyroscopeNoise = gyroscope.GetNoise();
        acceleromterNoise  = acceleromter.GetNoise();

        //TODO
        magnetometerNoise  = magnetometer.GetNoise();

        Init();

        

    }
    public void Initialize(Transform reference, GyroSim gyroscope, AccSim acceleromter, MagSim magnetometer, bool autoMode)
    {
        this.reference = reference;
        this.gyroscope = gyroscope;
        this.acceleromter = acceleromter;
        this.magnetometer = magnetometer;
        this.autoMode = autoMode;
        Init();
    }

    public virtual void Init(){
    }

    public virtual Quaternion GetState(){
        return new Quaternion(0, 0, 0, 1);
    }

    void Update()
    {

        //dt = Time.fixedDeltaTime;
        dt = Time.deltaTime;
        Vector3 bias = removeBiasMode? gyroscopeBias : Vector3.zero;
        angularVelocity = gyroscope.Read() - bias;

        bias = removeBiasMode? acceleromterBias : Vector3.zero;
        acceleration = (acceleromter.Read() - bias).normalized;

        magneticField = magnetometer.Read().normalized;
        prevRot = transform.rotation;
        if(autoMode){
            
            float start = Time.realtimeSinceStartup;
            UpdateOrientation();
            updateDuration = Time.realtimeSinceStartup - start;
        }
        if(renderer!=null)
            Verify();

    }

    public virtual void UpdateOrientation(){
        transform.rotation = transform.rotation;
    }


    void Verify(){
        angleDifference = Quaternion.Angle(reference.rotation, transform.rotation);
        smoothingAngle = Quaternion.Angle(prevRot, transform.rotation);
        angleDifferenceTotal +=angleDifference;
        if (angleDifference <= angleThreshold && !rotationMatch) {
            rotationMatch = true;
            renderer.material = correctMaterial;
        }
        else if (angleDifference > angleThreshold && rotationMatch){
            rotationMatch = false;
            renderer.material = incorrectMaterial;
        } else if(angleDifference <= angleThreshold && rotationMatch){
            rotationMatchTime += Time.deltaTime;
        }

    }
    

    public float RotationMatchTimeIndicator(){
        //return 100f * rotationMatchTime / Time.time;
        return rotationMatchTime;
    }


    public Vector3 zRot(float angle, Vector3 v){
        float sinZ = Mathf.Sin(angle);
        float cosZ = Mathf.Cos(angle);
        return new Vector3(
            (cosZ * v.x) + (-sinZ * v.y),
            (sinZ * v.x) + (cosZ * v.y),
            v.z
        );
    }

    public Vector3 xRot(float angle, Vector3 v){
        float sinX = Mathf.Sin(angle);
        float cosX = Mathf.Cos(angle);
        return new Vector3(
            v.x,
            (cosX * v.y) + (-sinX * v.z),
            (sinX * v.y) + (cosX * v.z)
        );
    }

}
