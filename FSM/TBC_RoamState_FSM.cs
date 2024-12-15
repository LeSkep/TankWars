using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RoamState : BaseState
{
    private SmartTank smartTank;

    public RoamState(SmartTank smartTank)
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
        // If enemyTank isn't null and is in range then go to chase state
        if ((smartTank.enemyTank != null) && (Vector3.Distance(smartTank.transform.position, smartTank.enemyTank.transform.position) < 30f))
        {
            return typeof(ChaseState);
        }
        // If enemyTank isn't null and is in attacking range then go to attack state
        else if ((smartTank.enemyBase != null) && (Vector3.Distance(smartTank.transform.position, smartTank.enemyBase.transform.position) < 30f))
        {
            return typeof(AttackBaseState);
        }
        // If the tank is low fuel then go to wait state
        else if (smartTank.lowFuel == true)
        {
            return typeof(WaitState);
        }
        else // If no other conditions are met then call the RandomRoam function
        {
            smartTank.RandomRoam();
            return null;
        }
    }
}
