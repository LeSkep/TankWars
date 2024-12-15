using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AttackState : BaseState
{
    private SmartTank smartTank;
    float time = 0; // Initialising the time variable and setting it to 0

    public AttackState(SmartTank smartTank)
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
    // Override to the StateUpdate function, if time is greater than 2 then enter roamstate 
    public override Type StateUpdate()
    {
        time += Time.deltaTime;
        if (time > 2)
        {
            time = 0; // Resetting time to 0 to allow time for attacking
            return typeof(RoamState);
        }
        else if (smartTank.lowFuel == true) // If lowFuel is true then enter wait state. lowFuel is boolean in the SmartTank script
        {
            return typeof(WaitState);
        }
        else if (smartTank.TankCurrentHealth < 50 || smartTank.TankCurrentAmmo < 4) // If health is lower than 50 or ammo is lower than 4, then enter flee state
        {
            return typeof(FleeState);
        }
        else // If none of the above conditions are met then the tank is okay to attack
        {
            smartTank.AttackTarget();
            return null;
        }
    }
}
