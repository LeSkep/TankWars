using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AttackStateFSMRBS : BaseState
{
    private SmartTankFSMRBS smartTank;
    float time = 0;
    // Start is called before the first frame update
    public AttackStateFSMRBS(SmartTankFSMRBS smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        smartTank.stats["attackState"] = true;

        return null;

    }

    public override Type StateExit()
    {
        smartTank.stats["attackState"] = false;

        time = 0;

        return null;
    }

    public override Type StateUpdate()
    {
        smartTank.AttackTarget();

        time += Time.deltaTime;

        if (time > 1f)
        {
            if (smartTank.stats["lowHealth"] == true)
            {
                return typeof(FleeStateFSMRBS);
            }

            if (smartTank.stats["targetReached"] == true)
            {
                return typeof(AttackStateFSMRBS);
            }
            else
            {
                return typeof(RoamStateFSMRBS);

            }
        }
        return null;
    }
}
