using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TextManager : MonoBehaviour
{


    [SerializeField]
    private TMP_Text UI;

    void Start()
    {

        var objectsWithComponent = FindObjectsOfType<AttitudeEstimator>();
        Camera mainCamera = Camera.main;

        foreach (var obj in objectsWithComponent)
        {
            GameObject textObject = new GameObject(obj.gameObject.name);
            textObject.transform.parent = this.transform;
            TextMeshPro label = textObject.AddComponent<TextMeshPro>();

            label.text = obj.gameObject.name;
            label.fontSize = 12;

            RectTransform rectTransform = label.GetComponent<RectTransform>();
            Vector3 targetPosition = obj.transform.position;
            rectTransform.position = new Vector3(targetPosition.x, targetPosition.y + 0.5f, targetPosition.z + 10f);

            float padding = 10f; // Dodatkowa przestrzeń, jeśli potrzebna
            float width = label.preferredWidth + padding;    

            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);

            textObject.transform.LookAt(mainCamera.transform);
        
            textObject.transform.Rotate(Vector3.up * 180f); 

        }
    }
    

    // Update is called once per frame
    void Update()
    {
        UI.text = $"Time: {Time.time:F2}s";
    }
}


