using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RoamState : BaseState
{
    private SmartTank smartTank;

    public RoamState(SmartTank smartTank)
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
            return typeof(ChaseState);
        }
        else if ((smartTank.enemyBase != null) && (Vector3.Distance(smartTank.transform.position, smartTank.enemyBase.transform.position) < 30f))
        {
            return typeof(AttackBaseState);
        }
        else if((smartTank.TankCurrentFuel < 30) /*&& (smartTank.hasWaited == false)*/)
        {
            return typeof(WaitState);
        }
        else
        {
            smartTank.RandomRoam();
            return null;
        }
    }
}
