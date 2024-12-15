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
    // Override to the StateUpdate function, if enemybases found returns null then enter the roamstate
    public override Type StateUpdate()
    {
        
        if (smartTank.enemyBasesFound == null)
        {
            return typeof(RoamState);
        }
        else // If enemybases found returns anything other then null then enter attackbase state
        {
            smartTank.AttackBase();
            return null;
        }
    }
}
