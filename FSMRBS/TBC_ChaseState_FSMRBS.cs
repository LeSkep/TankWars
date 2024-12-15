using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseStateFSMRBS : BaseState
{
    private SmartTankFSMRBS smartTank;
    // Start is called before the first frame update
    public ChaseStateFSMRBS(SmartTankFSMRBS smartTank)
    {
        this.smartTank = smartTank; // Creating a variable with access to the smartTank class
    }

    public override Type StateEnter()
    {
        smartTank.stats["chaseState"] = true; // Sets the chaseState stat to true on stateEnter


        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["chaseState"] = false; // Sets the chaseState stat to false on stateExit
        return null;
    }
    // Override to the StateUpdate function
    public override Type StateUpdate()
    {
        smartTank.ChaseTarget(); // Calls chase target

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
