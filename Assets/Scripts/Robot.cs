using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Robot : Agent

{
    public Transform headBone;
    public Transform player;
    public GameObject detectionIndicator;
    public float detectionTime;
    public TMP_Text detectionIndicatorText;
    public float fieldOfView;
    public float stoppedDistance;
    public float returnHomeTime;
    public GameObject detectedIndicator;
    public float stunningDistance;
    public Transform hands;

    private Animator animator;
    private NavMeshAgent navAgent;
    private float timeLeftUntilDetected;
    private Vector3 lastKnownPlayerLocation;
    private float timeLeftUntilReturnHome;
    private Vector3 homePosition;
    private bool playerDetected;
    private Artifact targetArtifact;
    private Artifact heldArtifact;
    

    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        // Start in the idle state
        GotoState(State.Idle);

        // Set the robot's home location
        homePosition = transform.position;
    }

    protected override void OnStateEntered(State state)
    {
        base.OnStateEntered(state);

        // Handle entering the new state
        switch (state)
        {
            case State.Idle:
                // Stop the walking animation
                animator.SetBool("Walking", false);
                break;
            case State.DetectingPlayer:
                // TODO: Have the robot look at the player with their head

                // Show the detection indicator above the robot's head
                detectionIndicator.SetActive(true);

                // Start the detection countdown
                timeLeftUntilDetected = detectionTime;
                break;
            case State.ChasingPlayer:
                // Play the walking animation
                animator.SetBool("Walking", true);
                break;
            case State.MoveToLastKnownPlayerPosition:
                // Play the walking animation
                animator.SetBool("Walking", true);

                // Move to the player's last known location
                navAgent.SetDestination(lastKnownPlayerLocation);
                break;
            case State.LookingForPlayer:
                // Stop the walking animation
                animator.SetBool("Walking", false);

                // Start the return home countdown
                timeLeftUntilReturnHome = returnHomeTime;
                break;
            case State.ReturningHome:
                // Play the walking animation
                animator.SetBool("Walking", true);

                // Move back home
                navAgent.SetDestination(homePosition);
                break;
            case State.GrabbingArtifact:
                // Play the walking animation
                animator.SetBool("Walking", true);
                // Move toward the artifact
                navAgent.SetDestination(targetArtifact.transform.position);
                break;
            case State.ReturningArtifact:
                // Play the walking animation
                animator.SetBool("Walking", true);

                // Move toward the artifact's home
                navAgent.SetDestination(heldArtifact.homePosition);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    protected override void OnStateLeft(State state)
    {
        base.OnStateLeft(state);
        // Handle leaving the previous state
        switch (state)
        {
            case State.Idle:
                break;
            case State.DetectingPlayer:
                // Hide the detection indicator above the robot's head
                detectionIndicator.SetActive(false);
                break;
            case State.ChasingPlayer:
                break;
            case State.MoveToLastKnownPlayerPosition:
                break;
            case State.LookingForPlayer:
                break;
            case State.ReturningHome:
                break;
            case State.GrabbingArtifact:
                break;
            case State.ReturningArtifact:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    public void Update()
    {
        
    // Handle updating the current state
    switch (currentState)
        {
            case State.Idle:
                IdleUpdate();
                break;
            case State.DetectingPlayer:
                DetectingPlayerUpdate();
                break;
            case State.ChasingPlayer:
                ChasingPlayerUpdate();
                break;
            case State.MoveToLastKnownPlayerPosition:
                MoveToLastKnownPlayerPositionUpdate();
                break;
            case State.LookingForPlayer:
                LookingForPlayerUpdate();
                break;
            case State.ReturningHome:
                ReturningHomeUpdate();
                break;
            case State.GrabbingArtifact:
                GrabbingArtifactUpdate();
                break;
            case State.ReturningArtifact:
                ReturningArtifactUpdate();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null);
        }
    }

    private bool CanSeePlayer()
    {
        // Calculate the direction from the robot's head to the player
        var playerDirection = (player.position - headBone.position).normalized;

        if (Physics.Raycast(headBone.position, playerDirection, out var hit))
        {
            // Is the player within the robot's field of view
            var angleToPlayer = Vector3.Angle(transform.forward, playerDirection);
            if (angleToPlayer >= -(fieldOfView / 2f) && angleToPlayer <= (fieldOfView / 2f))
            {
                // If the player was the thing we hit
                return hit.collider.gameObject.CompareTag("Player");
            }
        }

        // TODO: Why is this not false?!
        return false;
    }

    private bool ShouldChasePlayer()
    {
        // Should chase the player if they are holding an artifact
        return GameManager.Player.heldArtifact != null;
    }


    private void IdleUpdate()
    {
        // If the robot sees a player that wasn't detected alredy
        if (CanSeePlayer() && !playerDetected)
        {
            // Go to the detecting player state
            GotoState(State.DetectingPlayer);
            return;
        }

        // If the robot can't see the player but they were detected
        if(!CanSeePlayer() && playerDetected)
        {
            // Flag the player as undetected
            OnPlayerUndetected();
        }
    }

    private void DetectingPlayerUpdate() 
    {
        // Coundown the detection time
        timeLeftUntilDetected -= Time.deltaTime;

        // Update detection indicator
        var percent = 1f - (timeLeftUntilDetected / detectionTime);
        detectionIndicatorText.text = $"{percent * 100f:F0}%";

        // If the player has been fully detected
        if(timeLeftUntilDetected <= 0)
        {
            // Handle the player being detected
            OnPlayerDetected();

            // Should we chase the player?
            if (ShouldChasePlayer())
            {
                // Store the artifact we're trying to recover
                targetArtifact = GameManager.Player.heldArtifact;

                // Go to the chasing state
                GotoState(State.ChasingPlayer);
                return;
            }
            else
            {
                // Go back to the idle state
                GotoState(State.Idle);
                return;
            }
        }

        // If the robot lost sight of the player
        if(!CanSeePlayer())
        {
            // Go back to the idle state
            GotoState(State.Idle);
            return;
        }
    }

    private void ChasingPlayerUpdate()
    {
        // Set the robot's destination to the player's position 
        navAgent.SetDestination(player.position);

        // If the robot is close enough to the player
        if(Vector3.Distance(transform.position, GameManager.Player.transform.position) <= stunningDistance)
        {
            // Stun the player
            GameManager.Player.OnStunned();

            // TODO: What if the player drops the artifact?

            // Go to the grabbing artifact state
            GotoState(State.GrabbingArtifact);
            return;
        }

        // If the robot loses sight of the player
        if(!CanSeePlayer())
        {
            // Store the last known location of the player
            lastKnownPlayerLocation = player.position;

            // Go to the player's last known location state
            GotoState(State.MoveToLastKnownPlayerPosition);
            return;
        }
    }

    private void MoveToLastKnownPlayerPositionUpdate()
    {
        // If we reached the player's last known position
        if(navAgent.remainingDistance <= stoppedDistance)
        {
            // Go to the looking around for player state
            GotoState(State.LookingForPlayer);
            return;
        }

        // If the player was spotted again
        if (CanSeePlayer())
        {
            // Resume the chase
            GotoState(State.ChasingPlayer);
            return;
        }
    }

    private void LookingForPlayerUpdate()
    {
        // Coundown the return home time
        timeLeftUntilReturnHome -= Time.deltaTime;

        // If the robot is totally bored now
        if (timeLeftUntilReturnHome <= 0)
        {
            // Go to the returning home state
            GotoState(State.ReturningHome);
            return;
        }

        // If the robot sees the player again
        if (CanSeePlayer())
        {
            // Resume the chase
            GotoState(State.ChasingPlayer);
            return;
        }
    }

    private void ReturningHomeUpdate()
    {
        // If we reached the robot's home position
        if (navAgent.remainingDistance <= stoppedDistance)
        {
            // Go to idle state
            GotoState(State.Idle);
            return;
        }

        // If the player was spotted again and we should chase them
        if (CanSeePlayer() && ShouldChasePlayer())
        {
            // Resume the chase
            GotoState(State.ChasingPlayer);
            return;
        }
    }


    private void GrabbingArtifactUpdate()
    {
        // Keep moving towards the target artifact
        navAgent.SetDestination(targetArtifact.transform.position);

        // TODO: Handle the artifact being stashed 
        // TODO: Has the player picked up the artifact again
    }

    private void ReturningArtifactUpdate()
    {
        // If we've reached the artifact's home 
        if(navAgent.remainingDistance <= stoppedDistance)
        {
            // Drop the artifact
            heldArtifact.OnDropped();
            heldArtifact = null;

            // Go to the returning home state
            GotoState(State.ReturningHome);
            return;
        }
    }
    private void OnPlayerDetected()
    {
        playerDetected = true;

        // Show the detection indicator
        detectedIndicator.SetActive(true);
    }

    private void OnPlayerUndetected()
    {
        playerDetected = false;

        // Hide the detection indicator
        detectedIndicator.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == State.GrabbingArtifact)
            return;

        // If the thing we touched is an artifact
        var artifact = collision.collider.GetComponent<Artifact>();
        if (artifact != null && !artifact.stashed)
        {
            // Pick up the artifact
            artifact.OnPickedUp(hands);

            // Store the artifact the player is carrying
            heldArtifact = artifact;
            targetArtifact = null;

            // Go to the returning artifact state
            GotoState(State.ReturningArtifact);
            return;
        }
    }
}
