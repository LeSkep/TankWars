using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RoamStateFSMRBS : BaseState
{
    private SmartTankFSMRBS smartTank;

    public RoamStateFSMRBS(SmartTankFSMRBS smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        smartTank.stats["roamState"] = true;
        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["roamState"] = false;
        return null;
    }

    public override Type StateUpdate()
    {
        smartTank.RandomRoam();

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
