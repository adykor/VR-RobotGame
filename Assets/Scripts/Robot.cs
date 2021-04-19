﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Agent
{
    private void Start()
    {
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
                // Play the idle animation
                break;
            case State.DetectingPlayer:
                // Show the detection indicator above the robot's head
                break;
            case State.ChasingPlayer:
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
        throw new NotImplementedException();
    }
    private void DetectingPlayerUpdate()
    {
        throw new NotImplementedException();
    }
    private void ChasingPlayerUpdate()
    {
        throw new NotImplementedException();
    }

   

    
}