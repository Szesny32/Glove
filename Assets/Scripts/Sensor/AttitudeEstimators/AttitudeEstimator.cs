using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttitudeEstimator : MonoBehaviour
{
    public Transform reference;

    public GyroSim gyroscope;
    protected Vector3 angularVelocity;
    protected Vector3 gyroscopeBias;
    protected Vector3 gyroscopeNoise;

    public AccSim acceleromter;
    protected Vector3 acceleration;
    protected Vector3 acceleromterBias;
    protected Vector3 acceleromterNoise;

    public MagSim magnetometer;
    protected Vector3 magneticField;
    protected Vector3 magnetometerNoise;
    
    protected Renderer renderer;
    private Material correctMaterial;
    private Material incorrectMaterial;

    private float angleThreshold = .1f;
    protected bool rotationMatch = false;

    public bool removeBiasMode = false;
    
    void Start()
    {

        correctMaterial = Resources.Load<Material>("Correct");
        incorrectMaterial = Resources.Load<Material>("Incorrect");

        renderer = GetComponent<Renderer>();
        renderer.material = incorrectMaterial;
        gyroscopeBias = removeBiasMode? gyroscope.GetBias() : Vector3.zero;
        acceleromterBias = removeBiasMode? acceleromter.GetBias() : Vector3.zero;
        transform.rotation = reference.rotation;

        //[TODO]
        gyroscopeNoise = gyroscope.GetNoise();
        acceleromterNoise  = acceleromter.GetNoise();

        //TODO
        magnetometerNoise  = Vector3.zero;

    }

    void Update()
    {
        Vector3 bias = removeBiasMode? gyroscopeBias : Vector3.zero;
        angularVelocity = gyroscope.Read() - bias;

        bias = removeBiasMode? acceleromterBias : Vector3.zero;
        acceleration = (acceleromter.Read() - bias).normalized;

        magneticField = magnetometer.Read().normalized;

        UpdateOrientation();
        Verify();

    }

    public virtual void UpdateOrientation(){
        transform.rotation = transform.rotation;
    }


    void Verify(){
        float angleDifference = Quaternion.Angle(reference.rotation, transform.rotation);
        if (angleDifference <= angleThreshold && !rotationMatch) {
            rotationMatch = true;
            renderer.material = correctMaterial;
        }
        else if (angleDifference > angleThreshold && rotationMatch){
            rotationMatch = false;
            renderer.material = incorrectMaterial;
        }

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
