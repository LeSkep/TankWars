using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WaitState : BaseState
{
    private SmartTank smartTank;
    float t = 0;

    public WaitState(SmartTank smartTank)
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
    // Override to the StateUpdate function
    public override Type StateUpdate()
    {
       t += Time.deltaTime;
        
       if (t > 20) // If time is greater than 20 go to RoamState
       {
            return typeof(RoamState); 
       }
       else // If earlier condition isn't met then call Wait function
       {
            smartTank.Wait();
            return null;
       }
    }
}
