using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AttackState : BaseState
{
    private SmartTank smartTank;
    float time = 0;
    // Start is called before the first frame update
    public AttackState(SmartTank smartTank)
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
        time += Time.deltaTime;
        if (time > 2)
        {
            time = 0;
            return typeof(RoamState);
        }
        else
        {
            smartTank.AttackTarget();
            return null;
        }
    }
}
