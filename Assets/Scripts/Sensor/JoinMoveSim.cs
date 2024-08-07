using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinMoveSim : MonoBehaviour
{
    private Vector3 rotationLimitsP = new Vector3(0f, 90f, 0f); // Ograniczenia rotacji w stopniach
    private Vector3 rotationLimitsL = new Vector3(0f, -180f, 0f); // Ograniczenia rotacji w stopniach
    private float timer = 1f;
    private float clock = 0f;

    private Vector3 active = Vector3.up;
    float angleToRotate = 0f;

    private float rotationSpeedDegreesPerSecond = 30f; 
    

    private Vector3 currentEulerAngles; // Przechowuje aktualny kąt rotacji
  void Start()
    {
        // Inicjalizacja aktualnych kątów rotacji na podstawie początkowej rotacji obiektu
        currentEulerAngles = transform.localEulerAngles;
    }
    void Update()
    {

        clock += Time.deltaTime;

        clock += Time.deltaTime;
        if(clock > timer){
            active = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            clock %= timer;
            rotationSpeedDegreesPerSecond = 10f;
            angleToRotate = rotationSpeedDegreesPerSecond * Time.deltaTime;
        }

        // Oblicz nowy kąt rotacji
        Vector3 rotationAmount = active * angleToRotate;

        // Dodaj nowy kąt do aktualnych kątów
        Vector3 newEulerAngles = currentEulerAngles + rotationAmount;

        Debug.Log(currentEulerAngles);


        // Ogranicz rotację do zadanych limitów
        newEulerAngles.x = Mathf.Clamp(newEulerAngles.x, -rotationLimitsL.x, rotationLimitsP.x);
        newEulerAngles.y = Mathf.Clamp(newEulerAngles.y, -rotationLimitsL.y, rotationLimitsP.y);
        newEulerAngles.z = Mathf.Clamp(newEulerAngles.z, -rotationLimitsL.z, rotationLimitsP.z);

        // Zaktualizuj rotację obiektu
        transform.localEulerAngles = newEulerAngles;

        // Zaktualizuj aktualne kąty rotacji
        currentEulerAngles = transform.localEulerAngles;
    }



}
