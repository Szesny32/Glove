using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextRot : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro textMeshPro;

    public string index = "";

    void Start()
    {
        textMeshPro.text = "test";
    }

    // Update is called once per frame
    void Update()
    {
        textMeshPro.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
       
        float angle = 0.0f;
        Vector3 axis = Vector3.zero;
        transform.rotation.ToAngleAxis(out angle, out axis);

        textMeshPro.text = $"{index}\n{transform.rotation.eulerAngles}\n{axis} {angle}\n{transform.rotation}";
    }
}
