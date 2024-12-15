using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RoamStateFSMRBS : BaseState
{
    private SmartTankFSMRBS smartTank;
    // Start is called before the first frame update
    public RoamStateFSMRBS(SmartTankFSMRBS smartTank)
    {
        this.smartTank = smartTank; // Creating a variable with access to the smartTank class
    }

    public override Type StateEnter()
    {
        smartTank.stats["roamState"] = true; // Sets the roamState stat to true on stateEnter

        return null;
       
    }

    public override Type StateExit()
    {
        smartTank.stats["roamState"] = false; // Sets the roamState stat to true on stateExit

        return null;
    }
    // Override to the StateUpdate function
    public override Type StateUpdate()
    {
        smartTank.RandomRoam(); // Calls random roam

        // For loop to keep checking the for the next state
        foreach (var item in smartTank.rules.GetRules)
        {
            if (item.CheckRule(smartTank.stats) != null)
            {
                return item.CheckRule(smartTank.stats);

            }
        }
        return null;
    }
}
