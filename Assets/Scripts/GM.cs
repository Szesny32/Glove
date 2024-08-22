using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{   

    public GameObject prefab; // Prefabrykowany obiekt do sklonowania
    private int rows = 5; // Liczba wierszy w siatce
    private int columns = 5; // Liczba kolumn w siatce

     void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab not assigned!");
            return;
        }

        for (int z = 1; z < 2; z++){
            for (int y = 0; y < rows; y++){
                for (int x = 0; x < columns; x++){
                    Vector3 position = new Vector3((x - columns/2) * 17.5f , (y - 1 - rows/2) * 10f, -10f  * z);
                    Instantiate(prefab, position, Quaternion.identity);
                }
            }
        }
        
    }
}
