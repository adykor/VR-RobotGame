﻿using System;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private State currentState;

    public enum State
    {
        None,
        Idle,
        DetectingPlayer,
        ChasingPlayer
    }

    protected void GotoState(State newState)
    {
        // If there is a current state
        if (currentState != State.None)
        {
            // Let the agent know the current state is beign left
            OnStateLeft(currentState);
        }


        // Update the current state 
        currentState = newState;

        // Let the new state know it has been entered
        OnStateEntered(currentState);
    }

    private void OnStateEntered(State state)
    {
    }

    private void OnStateLeft(State state)
    {
    }
}