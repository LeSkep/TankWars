using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AttackStateFSMRBS : BaseState
{
    private SmartTankFSMRBS smartTank;
    float time = 0;
    // Start is called before the first frame update
    public AttackStateFSMRBS(SmartTankFSMRBS smartTank)
    {
        this.smartTank = smartTank; // Creating a variable with access to the smartTank class
    }

    public override Type StateEnter()
    {
        smartTank.stats["attackState"] = true; // Sets the attackState stat to true on stateEnter

        return null;

    }

    public override Type StateExit()
    {
        smartTank.stats["attackState"] = false; // Sets the attackState stat to false on stateExit

        time = 0;

        return null;
    }
    // Override to the StateUpdate function
    public override Type StateUpdate()
    {
        smartTank.AttackTarget(); // Calls the attack target function

        time += Time.deltaTime;

        if (time > 1f) // If time is greater than 1
        {
            if (smartTank.stats["lowHealth"] == true)
            {
                return typeof(FleeStateFSMRBS); // lowHealth is true, go to flee state
            }

            if (smartTank.stats["targetReached"] == true)
            {
                return typeof(AttackStateFSMRBS); // targetReached is true, go to attack state
            }
            else // If no earlier conditions are met then go to roam state
            {
                return typeof(RoamStateFSMRBS); 

            }
        }
        return null;
    }
}
