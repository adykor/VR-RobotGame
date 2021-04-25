using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance; // can be called singleton
    public Artifact[] artifacts;

    private void Awake()
    {
        // Are there any other game managers yet?
        if(instance != null)
        {
            // Error
            Debug.LogError("There was more than 1 GameManager");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal static void OnArtifactStashed()
    {

        // Loop through all the artifacts
            // If all artifacts are stashed
                // You win!
    }
}
