using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FleeStateFSMRBS : BaseState
{
    private SmartTankFSMRBS smartTank;

    public FleeStateFSMRBS(SmartTankFSMRBS smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        smartTank.stats["fleeState"] = true;
        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["fleeState"] = false;
        return null;
    }

    public override Type StateUpdate()
    {
        smartTank.Flee();

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
