using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AttackBaseState : BaseState
{
    private SmartTank smartTank;
    
    // Start is called before the first frame update
    public AttackBaseState(SmartTank smartTank)
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
        
        if (smartTank.enemyBasesFound == null)
        {
            return typeof(RoamState);
        }
        else
        {
            smartTank.AttackBase();
            return null;
        }
    }
}
