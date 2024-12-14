using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseStateFSMRBS : BaseState
{
    private SmartTankFSMRBS smartTank;

    public ChaseStateFSMRBS(SmartTankFSMRBS smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        smartTank.stats["chaseState"] = true;

        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["chaseState"] = false;

        return null;
    }

    public override Type StateUpdate()
    {
        smartTank.ChaseTarget();

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
