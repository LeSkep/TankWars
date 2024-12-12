using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WaitState : BaseState
{
    private SmartTank smartTank;
    float t;

    public WaitState(SmartTank smartTank)
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
       t += Time.deltaTime;

       if (t > 20)
       {
            smartTank.hasWaited = true;
            return typeof(RoamState);
            
        }
       else
       {
            smartTank.Wait();
            return null;
       }
    }
}
