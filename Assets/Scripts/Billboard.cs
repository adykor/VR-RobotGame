using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        // Face the camera
        transform.forward = Camera.main.transform.forward;
    }
}
