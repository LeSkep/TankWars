using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FleeState : BaseState
{
    private SmartTank smartTank;

    public FleeState(SmartTank smartTank)
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
        if (smartTank.TankCurrentHealth > 60 || smartTank.TankCurrentAmmo > 6 || smartTank.TankCurrentFuel > 60)
        {
            return typeof(RoamState);
        }
        else
        {
            smartTank.Flee();
            return null;
        }
    }
}
