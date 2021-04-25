using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Robot : Agent

{
    public Transform headBone;
    public Transform player;
    public GameObject detectionIndicator;
    public float detectionTime;
    public TMP_Text detectionIndicatorText;

    private Animator animator;
    private float timeLeftUntilDetected;

    private void Start()
    {
        animator = GetComponent<Animator>();

        // Start in the idle state
        GotoState(State.Idle);
    }

    protected override void OnStateEntered(State state)
    {
        base.OnStateEntered(state);

        // Handle entering the new state
        switch (state)
        {
            case State.Idle:
                break;
            case State.DetectingPlayer:
                // Show the detection indicator above the robot's head
                detectionIndicator.SetActive(true);

                // Start the detection countdown
                timeLeftUntilDetected = detectionTime;
                break;
            case State.ChasingPlayer:
                // Play the walking animation
                animator.SetBool("Walking", true);
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
            default:
                throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null);
        }
    }

    private void IdleUpdate()
    {
        // Can the robot see the player
        if(CanSeePlayer())
        {
            // Go to the detecting player state
            GotoState(State.DetectingPlayer);
        }
    }

    private bool CanSeePlayer()
    {
        // Calculate the direction from the robot's head to the player
        var playerDirection = (player.position - headBone.position).normalized;

        if (Physics.Raycast(headBone.position, playerDirection, out var hit))
        {
            // If the player was the thing we hit
            return hit.collider.gameObject.CompareTag("Player");
        }

        // TODO: Why is this not false?!
        return true;
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
            // Go to the chasing state
            GotoState(State.ChasingPlayer);
        }

        // If the robot lost sight of the player
        if(!CanSeePlayer())
        {
            // Go back to the idle state
            GotoState(State.Idle);
        }
    }
    private void ChasingPlayerUpdate()
    {

    }
}
