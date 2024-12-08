using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    private SmartTank smartTank;

    public ChaseState(SmartTank smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        return null;
    }
    
    public override Type StateExit()
    {
        return null;
    }
    
    public override Type StateUpdate()
    {
        if ((smartTank.enemyTank != null) && (Vector3.Distance(smartTank.transform.position, smartTank.enemyTank.transform.position) < 30f))
        {
            return typeof(AttackState);
        }
        else
        {
            smartTank.ChaseTarget();
            return null;
        }
    }
    }
