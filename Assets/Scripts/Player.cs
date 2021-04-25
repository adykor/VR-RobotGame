using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform hands;
    private Artifact heldArtifact;

    private void OnCollisionEnter(Collision collision)
    {
        // If the thing we touched is an artifact
        var artifact = collision.collider.GetComponent<Artifact>();
        if(artifact != null)
        {
            // Pick up the artifact
            artifact.OnPickedUp(hands);

            // Store the artifact the player is carrying
            heldArtifact = artifact;

            // TODO: Alert nearby robots that the player has picked up an artifact
        }
    }

    private void Update()
    {
        // Drop the artifact when space is pressed and we're holding an artifact
        if (Input.GetKeyDown(KeyCode.Space) && heldArtifact!= null)
        {
            // Drop the artifact
            heldArtifact.OnDropped();
            heldArtifact = null;

            // TODO: Throw the artifact?
        }
    }
}
