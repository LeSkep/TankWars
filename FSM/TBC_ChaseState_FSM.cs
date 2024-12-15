using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    private SmartTank smartTank;
    // Start is called before the first frame update
    public ChaseState(SmartTank smartTank)
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
        if (Vector3.Distance(smartTank.transform.position, smartTank.enemyTank.transform.position) < 30f) // If enemyTank is in range then go to attack state
        {
            return typeof(AttackState);
        }
        else
        {
            smartTank.ChaseTarget(); // If the above condition isn't met then call the chaseTarget function
            return null;
        }
    }
    }
