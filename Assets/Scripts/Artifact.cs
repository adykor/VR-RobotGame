using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : MonoBehaviour
{
    private Rigidbody rigidBody;
    internal bool stashed;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }
    internal void OnPickedUp(Transform hands)
    {
        // Use the kinematic approach to be picked up
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
        transform.SetParent(hands);

        // Reset the artifact's position
        transform.localPosition = Vector3.zero;
    }

    internal void OnDropped()
    {
        // Use the kinematic approach to be dropped
        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;
        transform.SetParent(null);
    }
}
