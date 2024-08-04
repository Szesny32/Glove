using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TextManager : MonoBehaviour
{


    [SerializeField]
    private TMP_Text UI;

    private AttitudeEstimator[] objects;
    private TextMeshPro[] labels;

    void Start()
    {

        var objectsWithComponent = FindObjectsOfType<AttitudeEstimator>();
        Camera mainCamera = Camera.main;

        objects = new AttitudeEstimator[objectsWithComponent.Length];
        labels = new TextMeshPro[objectsWithComponent.Length];

        for ( int i = 0; i < objectsWithComponent.Length; i++)
        {
            objects[i] = objectsWithComponent[i].GetComponent<AttitudeEstimator>();
            GameObject obj = new GameObject(objectsWithComponent[i].gameObject.name);
            obj.transform.parent = this.transform;
            labels[i] = obj.AddComponent<TextMeshPro>();

            labels[i].text = objectsWithComponent[i].gameObject.name;
            labels[i].fontSize = 10;

            RectTransform rectTransform = labels[i].GetComponent<RectTransform>();
            Vector3 targetPosition = objectsWithComponent[i].transform.position;
            rectTransform.position = new Vector3(targetPosition.x, targetPosition.y + 0.5f, targetPosition.z + 10f);

            float padding = 10f; // Dodatkowa przestrzeń, jeśli potrzebna
            float width = labels[i].preferredWidth + padding;    

            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);

            obj.transform.LookAt(mainCamera.transform);
            obj.transform.Rotate(Vector3.up * 180f); 

        }
    }
    

    // Update is called once per frame
    void Update()
    {
        UI.text = $"Time: {Time.time:F2}s";
        for (int i = 0; i < objects.Length; i++){
            float t = objects[i].RotationMatchTimeIndicator();
            labels[i].text = $"{objects[i].gameObject.name} \n{t:F2}s [{100*t/Time.time:F2}%]"; 
        }
    }
}


