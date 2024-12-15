﻿using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AStar;
using JetBrains.Annotations;
using static UnityEditor.PlayerSettings;

/// <summary>
/// Class <c>DumbTank</c> is an example class used to demonstrate how to use the functions available from the <c>AITank</c> base class. 
/// Copy this class when creating your smart tank class.
/// </summary>
public class SmartTankFSMRBS : AITank
{
    public Dictionary<GameObject, float> enemyTanksFound = new Dictionary<GameObject, float>();     /*!< <c>enemyTanksFound</c> stores all tanks that are visible within the tanks sensor. */
    public Dictionary<GameObject, float> consumablesFound = new Dictionary<GameObject, float>();    /*!< <c>consumablesFound</c> stores all consumables that are visible within the tanks sensor. */
    public Dictionary<GameObject, float> enemyBasesFound = new Dictionary<GameObject, float>();     /*!< <c>enemyBasesFound</c> stores all enemybases that are visible within the tanks sensor. */

    public GameObject enemyTank;        /*!< <c>enemyTank</c> stores a reference to a target enemy tank. 
                                        * This should be taken from <c>enemyTanksFound</c>, only whilst within the tank sensor. 
                                        * Reference should be removed and refreshed every update. */

    public GameObject consumable;       /*!< <c>consumable</c> stores a reference to a target consumable. 
                                        * This should be taken from <c>consumablesFound</c>, only whilst within the tank sensor. 
                                        * Reference should be removed and refreshed every update. */

    public GameObject enemyBase;        /*!< <c>enemyBase</c> stores a reference to a target enemy base. 
                                         * This should be taken from <c>enemyBasesFound</c>, only whilst within the tank sensor. 
                                        * Reference should be removed and refreshed every update. */

    float t;    /*!< <c>t</c> stores timer value */
    public HeuristicMode heuristicMode; /*!< <c>heuristicMode</c> Which heuristic used for find path. */

    private GameObject turretGun; // Creating a game object variable named turretGun
    public float constantSpeed = 5f; // Stores a constant speed value for turret rotation speed


    public bool lowHealth; // Boolean for checking low health

    // New dictionary called facts that stores strings and booleans
    public Dictionary<string, bool> stats = new Dictionary<string, bool>();
    // New variable called rules with access to the RulesFSMRBS class
    public RulesFSMRBS rules = new RulesFSMRBS();

   
    /// <summary>
    ///WARNING, do not use void <c>Start()</c> function, use this <c>AITankStart()</c> function instead if you want to use Start method from Monobehaviour.
    ///Use this function to initialise your tank variables etc.
    /// </summary>
    public override void AITankStart()
    {
        //turretGun = transform.Find("Model").transform.Find("Turret").gameObject;
        InitialiseStats(); // Calls InitialiseStats function to Initialise all the stats
        InitialiseRules(); // Calls InitialiseRules function to Initialise all the rules
        InitialiseStateMachine(); // Calls InitialiseStateMachine function to Initialise the state machine
    }
    // Initialises stats and sets all to false on called
    private void InitialiseStats()
    {
        stats.Add("lowHealth", lowHealth);
        stats.Add("targetSpotted", false);
        stats.Add("targetReached", false);
        stats.Add("fleeState", false);
        stats.Add("chaseState", false);
        stats.Add("roamState", false);
        stats.Add("attackState", false);
    }
    // Initialises rules 
    // Each rule has to antecedents and one consequent, the consequent is only reached if once or both of the antecedents is true based on the Predicate
    private void InitialiseRules()
    {
        rules.AddRule(new RuleFSMRBS("attackState", "lowHealth", typeof(FleeStateFSMRBS), RuleFSMRBS.Predicate.And));
        rules.AddRule(new RuleFSMRBS("chaseState", "lowHealth", typeof(FleeStateFSMRBS), RuleFSMRBS.Predicate.And));
        rules.AddRule(new RuleFSMRBS("targetSpotted", "roamState", typeof(ChaseStateFSMRBS), RuleFSMRBS.Predicate.And));
        rules.AddRule(new RuleFSMRBS("roamState", "targetSpotted", typeof(RoamStateFSMRBS), RuleFSMRBS.Predicate.nAnd));
        rules.AddRule(new RuleFSMRBS("chaseState", "targetReached", typeof(AttackStateFSMRBS), RuleFSMRBS.Predicate.And));
    }
    // Initialises the state machine
    private void InitialiseStateMachine()
    {
        // Creating a dictionary to store the states
        Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>();

        states.Add(typeof(RoamStateFSMRBS), new RoamStateFSMRBS(this));
        states.Add(typeof(ChaseStateFSMRBS), new ChaseStateFSMRBS(this));
        states.Add(typeof(AttackStateFSMRBS), new AttackStateFSMRBS(this));
        states.Add(typeof(FleeStateFSMRBS), new FleeStateFSMRBS(this));
        //states.Add(typeof(AttackBaseState), new AttackBaseState(this));
        //states.Add(typeof(WaitState), new WaitState(this));

        GetComponent<StateMachineFSMRBS>().SetStates(states); // Getting a function from the state machine to script to set the states in the dictionary 
    }
    // Chase target function
    public void ChaseTarget()
    {
        if (enemyTank != null) // If enemyTank stores a value
        {
            //Follow target
            FollowPathToWorldPoint(enemyTank, 1f, heuristicMode);
        }
    }
    // Attack target
    public void AttackTarget()
    {
        // fire at target
        TurretFireAtPoint(enemyTank);
    }
    // Tank roam function
    public void RandomRoam()
    {
        
        //searching
        enemyTank = null; // Resets the enemyTank variable
        consumable = null; // Resets the consumable variable
        enemyBase = null; // Resets the enemyBase variable
        // Follow path
        FollowPathToRandomWorldPoint(1f, heuristicMode);
        t += Time.deltaTime;
        if (t > 10) // If time is greater than 10
        {
            // Follow new path
            GenerateNewRandomWorldPoint();
            t = 0;
        }
        // Look for consumables
        if (consumablesFound.Count > 0)
        {
            //get the first consumable from the list.
            consumable = consumablesFound.First().Key;
            FollowPathToWorldPoint(consumable, 1f, heuristicMode);
            t += Time.deltaTime;
            if (t > 10)
            {
                GenerateNewRandomWorldPoint();
                t = 0;
            }
        }
    }
    // Flee function
    public void Flee()
    {
        // Looks for consumables
        if (consumablesFound.Count > 0)
        {
            //get the first consumable from the list.
            consumable = consumablesFound.First().Key;
            FollowPathToWorldPoint(consumable, 1f, heuristicMode);
            t += Time.deltaTime;
            if (t > 15)
            {
                GenerateNewRandomWorldPoint();
                t = 0;
            }
        }
        else // If it can't see any consumables then it generates a new path and follows it
        {
            enemyTank = null; // Resets the enemyTank variable
            consumable = null; // Resets the consumable variable
            enemyBase = null; // Resets the enemyBase variable
            FollowPathToRandomWorldPoint(1f, heuristicMode);
        }
    }

    //public void AttackBase()
    //{
    //
    //    if (enemyBase != null)
    //    {
    //        //go close to it and fire
    //        if (Vector3.Distance(transform.position, enemyBase.transform.position) < 25f)
    //        {
    //            TurretFireAtPoint(enemyBase);
    //        }
    //        else
    //        {
    //            FollowPathToWorldPoint(enemyBase, 1f, heuristicMode);
    //
    //        }
    //    }
    //    else
    //    {
    //        //searching
    //        enemyTank = null;
    //        consumable = null;
    //        enemyBase = null;
    //        FollowPathToRandomWorldPoint(1f, heuristicMode);
    //        t += Time.deltaTime;
    //        if (t > 10)
    //        {
    //            GenerateNewRandomWorldPoint();
    //            t = 0;
    //        }
    //    }
    //    CheckTargetSpotted();
    //    CheckTargetReached();
    //}
    //
    //public void Wait()
    //{
    //   
    //    t += Time.deltaTime;
    //    TankStop();
    //    turretGun.transform.Rotate(0f, 0f, constantSpeed * Time.deltaTime);
    //    if (t > 20)
    //    {
    //        GenerateNewRandomWorldPoint();
    //        t = 0;
    //    }
    //    else
    //    {
    //        enemyTank = null;
    //        consumable = null;
    //        enemyBase = null;
    //        FollowPathToRandomWorldPoint(1f, heuristicMode);
    //    }
    //    CheckTargetSpotted();
    //    CheckTargetReached();
    //}

    // Function to update stats
    void UpdateStats()
    {
        if (enemyTank != null) // If enemyTank is not null
        {
            // If enemyTank is within a 30 unit range then targetspotted = true, else false
            stats["targetSpotted"] = Vector3.Distance(transform.position, enemyTank.transform.position) < 30f ? true : false;
            // If enemyTank is within a 25 unit range then targetreached = true, else false
            stats["targetReached"] = Vector3.Distance(transform.position, enemyTank.transform.position) < 25f ? true : false;
        }

        // Checks to see if lowHealth, lowAmmo and lowFuel are true or false 
        stats["lowHealth"] = TankCurrentHealth < 50 ? true : false;
    }


    /// <summary>
    ///WARNING, do not use void <c>Update()</c> function, use this <c>AITankUpdate()</c> function instead if you want to use Start method from Monobehaviour.
    ///Function checks to see what is currently visible in the tank sensor and updates the Founds list. <code>First().Key</code> is used to get the first
    ///element found in that dictionary list and is set as the target, based on tank health, ammo and fuel. 
    /// </summary>
    public override void AITankUpdate()
    {
        //Update all currently visible.
        
        // Constantly calls the update stats funciton to see if they're true or false
        UpdateStats();

        enemyTanksFound = VisibleEnemyTanks; // Stores the VisibleEnemyTanks dictionary in the enemyTanksFound variable   
        consumablesFound = VisibleConsumables; // Stores the VisibleConsumables dictionary in the consumablesFound variable
        enemyBasesFound = VisibleEnemyBases; // Stores the VisibleEnemyBases dictionary in the enemyBasesFound variable

        // Constantly checking to see if an enemy tanks is found and then stores it
        if (enemyTanksFound.Count > 0 && enemyTanksFound.First().Key != null)
        {
            // if tank is found
            enemyTank = enemyTanksFound.First().Key;
        }

        // Constantly checking to see if an enemy base is found and then stores it
        if (enemyBasesFound.Count > 0)
        {
            //if base is found
            enemyBase = enemyBasesFound.First().Key;
        }
    }

    /// <summary>
    ///WARNING, do not use void <c>OnCollisionEnter()</c> function, use this <c>AIOnCollisionEnter()</c> function instead if you want to use OnColiisionEnter function from Monobehaviour.
    ///Use this function to see if tank has collided with anything.
    /// </summary>
    public override void AIOnCollisionEnter(Collision collision)
    {
    }



    /*******************************************************************************************************       
    Below are a set of functions you can use. These reference the functions in the AITank Abstract class
    and are protected. These are simply to make access easier if you an not familiar with inheritance and modifiers
    when dealing with reference to this class. This does mean you will have two similar function names, one in this
    class and one from the AIClass. 
    *******************************************************************************************************/


    /// <summary>
    /// Generate a path from current position to pointInWorld (GameObject). If no heuristic mode is set, default is Euclidean,
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    public void GeneratePathToWorldPoint(GameObject pointInWorld)
    {
        a_FindPathToPoint(pointInWorld);
    }

    /// <summary>
    /// Generate a path from current position to pointInWorld (GameObject)
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    /// <param name="heuristic">Chosen heuristic for path finding</param>
    public void GeneratePathToWorldPoint(GameObject pointInWorld, HeuristicMode heuristic)
    {
        a_FindPathToPoint(pointInWorld, heuristic);
    }

    /// <summary>
    ///Generate and Follow path to pointInWorld (GameObject) at normalizedSpeed (0-1). If no heuristic mode is set, default is Euclidean,
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    public void FollowPathToWorldPoint(GameObject pointInWorld, float normalizedSpeed)
    {
        a_FollowPathToPoint(pointInWorld, normalizedSpeed);
    }

    /// <summary>
    ///Generate and Follow path to pointInWorld (GameObject) at normalizedSpeed (0-1). 
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    /// <param name="heuristic">Chosen heuristic for path finding</param>
    public void FollowPathToWorldPoint(GameObject pointInWorld, float normalizedSpeed, HeuristicMode heuristic)
    {
        a_FollowPathToPoint(pointInWorld, normalizedSpeed, heuristic);
    }

    /// <summary>
    ///Generate and Follow path to a randome point at normalizedSpeed (0-1). Go to a randon spot in the playfield. 
    ///If no heuristic mode is set, default is Euclidean,
    /// </summary>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    public void FollowPathToRandomWorldPoint(float normalizedSpeed)
    {
        a_FollowPathToRandomPoint(normalizedSpeed);
    }

    /// <summary>
    ///Generate and Follow path to a randome point at normalizedSpeed (0-1). Go to a randon spot in the playfield
    /// </summary>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    /// <param name="heuristic">Chosen heuristic for path finding</param>
    public void FollowPathToRandomWorldPoint(float normalizedSpeed, HeuristicMode heuristic)
    {
        a_FollowPathToRandomPoint(normalizedSpeed, heuristic);
    }

    /// <summary>
    ///Generate new random point
    /// </summary>
    public void GenerateNewRandomWorldPoint()
    {
        a_GenerateRandomPoint();
    }

    /// <summary>
    /// Stop Tank at current position.
    /// </summary>
    public void TankStop()
    {
        a_StopTank();
    }

    /// <summary>
    /// Continue Tank movement at last know speed and pointInWorld path.
    /// </summary>
    public void TankGo()
    {
        a_StartTank();
    }

    /// <summary>
    /// Face turret to pointInWorld (GameObject)
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    public void TurretFaceWorldPoint(GameObject pointInWorld)
    {
        a_FaceTurretToPoint(pointInWorld);
    }

    /// <summary>
    /// Reset turret to forward facing position
    /// </summary>
    public void TurretReset()
    {
        a_ResetTurret();
    }

    /// <summary>
    /// Face turret to pointInWorld (GameObject) and fire (has delay).
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    public void TurretFireAtPoint(GameObject pointInWorld)
    {
        a_FireAtPoint(pointInWorld);
    }

    /// <summary>
    /// Returns true if the tank is currently in the process of firing.
    /// </summary>
    public bool TankIsFiring()
    {
        return a_IsFiring;
    }

    /// <summary>
    /// Returns float value of remaining health.
    /// </summary>
    /// <returns>Current health.</returns>
    public float TankCurrentHealth
    {
        get
        {
            return a_GetHealthLevel;
        }
    }

    /// <summary>
    /// Returns float value of remaining ammo.
    /// </summary>
    /// <returns>Current ammo.</returns>
    public float TankCurrentAmmo
    {
        get
        {
            return a_GetAmmoLevel;
        }
    }

    /// <summary>
    /// Returns float value of remaining fuel.
    /// </summary>
    /// <returns>Current fuel level.</returns>
    public float TankCurrentFuel
    {
        get
        {
            return a_GetFuelLevel;
        }
    }

    /// <summary>
    /// Returns list of friendly bases. Does not include bases which have been destroyed.
    /// </summary>
    /// <returns>List of your own bases which are. </returns>
    protected List<GameObject> MyBases
    {
        get
        {
            return a_GetMyBases;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject target, float distance) of visible targets (tanks in TankMain LayerMask).
    /// </summary>
    /// <returns>All enemy tanks currently visible.</returns>
    protected Dictionary<GameObject, float> VisibleEnemyTanks
    {
        get
        {
            return a_TanksFound;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject consumable, float distance) of visible consumables (consumables in Consumable LayerMask).
    /// </summary>
    /// <returns>All consumables currently visible.</returns>
    protected Dictionary<GameObject, float> VisibleConsumables
    {
        get
        {
            return a_ConsumablesFound;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject base, float distance) of visible enemy bases (bases in Base LayerMask).
    /// </summary>
    /// <returns>All enemy bases currently visible.</returns>
    protected Dictionary<GameObject, float> VisibleEnemyBases
    {
        get
        {
            return a_BasesFound;
        }
    }

}
