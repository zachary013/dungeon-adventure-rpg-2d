// using System.Reflection.PortableExecutable;
// using System.Diagnostics;
// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
// using TMPro;

public class Main : MonoBehaviour
{
    [SerializeField]
    private RoomFirstDungeonGenerator dungeonGenerator;

    void Start()
    {
        if (dungeonGenerator != null)
            {
                dungeonGenerator.playRunProceduralGeneration(); // Invoke the generation method
            }
            else
            {
                Debug.LogError("Dungeon Generator is not initialized!"); // Log an error if not initialized
            }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (dungeonGenerator != null)
            {
                dungeonGenerator.playRunProceduralGeneration(); // Invoke the generation method
            }
            else
            {
                Debug.LogError("Dungeon Generator is not initialized!"); // Log an error if not initialized
            }
        }
    }
}
