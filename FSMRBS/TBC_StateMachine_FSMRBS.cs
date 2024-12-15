using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachineFSMRBS : MonoBehaviour
{
    // Creating a type dictionary for the states
    private Dictionary<Type, BaseState> states;

    public BaseState currentState;
    // Sets current state
    public BaseState CurrentState
    {
        get
        {
            return currentState;
        }
        private set
        {
            currentState = value;
        }
    }
    // Function to set state
    public void SetStates(Dictionary<Type, BaseState> states)
    {
        this.states = states;
    }
    // If the current state is null then set current state to states.first(the first state available)
    void Update()
    {
        if(CurrentState == null)
        {
            CurrentState = states.Values.First();
            CurrentState.StateEnter();
        }
        // If the current state is NOT null then the next state is selected (the state is switched)
        else
        {
            var nextState = CurrentState.StateUpdate();

            if(nextState != null && nextState != CurrentState.GetType())
            {
                SwitchToState(nextState);
            }
        }
    }
    // Switch state function
    void SwitchToState(Type nextState)
    {
        CurrentState.StateExit(); // Exiting the current selected state
        CurrentState = states[nextState]; // Setting the state
        CurrentState.StateEnter(); // Enters the new state

    }
}
