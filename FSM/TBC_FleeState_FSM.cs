using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FleeState : BaseState
{
    private SmartTank smartTank;

    public FleeState(SmartTank smartTank)
    {
        this.smartTank = smartTank; // Creating a variable with access to the smartTank class
    }

    public override Type StateEnter()
    {
        return null;
    }

    public override Type StateExit()
    {
        return null;
    }
    // Override to the StateUpdate function
    public override Type StateUpdate()
    {
        // If tank is low fuel then go to wait state
        if (smartTank.lowFuel == true)
        {
            return typeof(WaitState);
        }
        // If tanks health is less than 50 then call the flee function
        if (smartTank.TankCurrentHealth < 50)
        {
            smartTank.Flee();
            return null;
        }
        // If no other conditions are met then go to RoamState
        else
        {
            return typeof(RoamState);
        }

        
    }
}
