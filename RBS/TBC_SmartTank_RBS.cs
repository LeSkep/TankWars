using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using static AStar;

/// <summary>
/// Class <c>DumbTank</c> is an example class used to demonstrate how to use the functions available from the <c>AITank</c> base class. 
/// Copy this class when creating your smart tank class.
/// </summary>
public class SmartTank_RBS : AITank
{
    public Dictionary<GameObject, float> enemyTanksFound = new Dictionary<GameObject, float>();     /*!< <c>enemyTanksFound</c> stores all tanks that are visible within the tanks sensor. */
    public Dictionary<GameObject, float> consumablesFound = new Dictionary<GameObject, float>();    /*!< <c>consumablesFound</c> stores all consumables that are visible within the tanks sensor. */
    public Dictionary<GameObject, float> enemyBasesFound = new Dictionary<GameObject, float>();     /*!< <c>enemyBasesFound</c> stores all enemybases that are visible within the tanks sensor. */

    public Dictionary<string, bool> facts = new Dictionary<string, bool>();
    Rules rules = new Rules();

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
    float attackDistance = 25f;
    //public bool lowHealth;
    public HeuristicMode heuristicMode; /*!< <c>heuristicMode</c> Which heuristic used for find path. */

    /// <summary>
    ///WARNING, do not use void <c>Start()</c> function, use this <c>AITankStart()</c> function instead if you want to use Start method from Monobehaviour.
    ///Use this function to initialise your tank variables etc.
    /// </summary>
    public override void AITankStart()
    {
        InitialiseFacts();
        InitialiseRules();
    }

    void InitialiseFacts()
    {
       // facts.Add("baseReached", false);
        facts.Add("targetReached", false);
        facts.Add("lowHealth", false);
        //facts.Add("goodHealth", false);
        facts.Add("followTarget", false);
        facts.Add("targetSpotted", false);
        facts.Add("lowAmmo", false);
        facts.Add("lowFuel", false);
        facts.Add("flee", false);
        facts.Add("attack", false);
        facts.Add("canAttack", false);
        facts.Add("roam", false);
       // facts.Add("attackBase", false);
    }

    void InitialiseRules()
    {
        rules.AddRule(new Rule("attack", "lowHealth", "flee", Rule.Predicate.And));
        rules.AddRule(new Rule("followTarget", "lowHealth", "flee", Rule.Predicate.And));
        rules.AddRule(new Rule("targetSpotted", "roam", "followTarget", Rule.Predicate.And));
        rules.AddRule(new Rule("targetSpotted", "lowHealth", "roam", Rule.Predicate.nAnd));
        rules.AddRule(new Rule("roam", "targetSpotted", "roam", Rule.Predicate.nAnd));
        rules.AddRule(new Rule("lowAmmo", "lowHealth", "canAttack", Rule.Predicate.nAnd));
        rules.AddRule(new Rule("lowHealth", "lowFuel", "canAttack", Rule.Predicate.nAnd));
        rules.AddRule(new Rule("targetReached", "canAttack", "attack", Rule.Predicate.And));
       // rules.AddRule(new Rule("baseReached", "canAttack", "attackBase", Rule.Predicate.And));

    }

    
    public void RandomRoam()
    {
        //searching
        enemyTank = null;
        consumable = null;
        enemyBase = null;
        FollowPathToRandomWorldPoint(1f, heuristicMode);
        t += Time.deltaTime;
        if (t > 10)
        {
            GenerateNewRandomWorldPoint();
            t = 0;
        }

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
    }

    public void ChaseTarget()
    {
        if (enemyTank != null)
        {
            //Follow target
            FollowPathToWorldPoint(enemyTank, 1f, heuristicMode);
        }
    }

    public void AttackTarget()
    {
        //Debug.Log(facts["attack"]);
        if (Vector3.Distance(transform.position, enemyTank.transform.position) < 25f)
        {
            //get closer to target, and fire
            TurretFireAtPoint(enemyTank);
        }
        else if (facts["lowHealth"] == true)
        {
            facts["attack"] = false;
        }
    }

    public void Flee()
    {
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
        else
        {
            enemyTank = null;
            consumable = null;
            enemyBase = null;
            GenerateNewRandomWorldPoint();
            FollowPathToRandomWorldPoint(1f, heuristicMode);
        }
    }

    public void AttackBase()
    {

        if (enemyBase != null)
        {
            //go close to it and fire
            if (Vector3.Distance(transform.position, enemyBase.transform.position) < 25f)
            {
                TurretFireAtPoint(enemyBase);
            }
            else
            {
                FollowPathToWorldPoint(enemyBase, 1f, heuristicMode);

            }
        }
    }


    void UpdateFacts()
    {
        facts["lowHealth"] = TankCurrentHealth < 45f ? true : false;

        if (enemyTank != null) 
        {
            facts["targetSpotted"] = Vector3.Distance(transform.position, enemyTank.transform.position) < 30f ? true : false;
            facts["targetReached"] = Vector3.Distance(transform.position, enemyTank.transform.position) < attackDistance ? true : false;
        }

        //if(enemyBase != null)
        //{
        //    facts["baseReached"] = Vector3.Distance(transform.position, enemyBase.transform.position) < attackDistance ?
        //    true : false;
        //}
        //Debug.Log("Tank Health: " + TankCurrentHealth);
        //Debug.Log(facts["lowHealth"]);
        
        //facts["goodHealth"] = TankCurrentHealth > 50 ? true : false;
        facts["lowAmmo"] = TankCurrentAmmo < 4 ? true : false;
        facts["lowFuel"] = TankCurrentFuel < 50 ? true : false;
    }

    void ShouldFlee()
    {
        foreach (var item in rules.GetRules)
        {
            if (item.CheckRule(facts) != null)
            {
                facts = item.CheckRule(facts);
            }
        }
        if (facts["flee"])
        {
            Flee();
        }
    }

    void ShouldAttack()
    {
        foreach (var item in rules.GetRules)
        {
            if (item.CheckRule(facts) != null)
            {
                facts = item.CheckRule(facts);
            }
        }
        if (facts["attack"])
        {
            AttackTarget();
        }
    }

    void ShouldFollowTarget()
    {
        foreach (var item in rules.GetRules)
        {
            if (item.CheckRule(facts) != null)
            {
                facts = item.CheckRule(facts);
            }
        }
        if (facts["followTarget"])
        {
            ChaseTarget();
        }    
    }

    void ShouldRoam()
    {
        foreach (var item in rules.GetRules)
        {
            if (item.CheckRule(facts) != null)
            {
                facts = item.CheckRule(facts);
            }
        }
        if (facts["roam"])
        {
            RandomRoam();
        }
    }

    void ShouldAttackBase()
    {
        foreach (var item in rules.GetRules)
        {
            if (item.CheckRule(facts) != null)
            {
                facts = item.CheckRule(facts);
            }
        }
        if (facts["attackBase"])
        {
            AttackBase();
        }
    }

    /// <summary>
    ///WARNING, do not use void <c>Update()</c> function, use this <c>AITankUpdate()</c> function instead if you want to use Start method from Monobehaviour.
    ///Function checks to see what is currently visible in the tank sensor and updates the Founds list. <code>First().Key</code> is used to get the first
    ///element found in that dictionary list and is set as the target, based on tank health, ammo and fuel. 
    /// </summary>
    public override void AITankUpdate()
    {
        UpdateFacts();
        ShouldFlee();
        ShouldFollowTarget();
        ShouldRoam();
        ShouldAttack();
        //Update all currently visible.
        enemyTanksFound = VisibleEnemyTanks;
        consumablesFound = VisibleConsumables;
        enemyBasesFound = VisibleEnemyBases;

        if (enemyTanksFound.Count > 0 && enemyTanksFound.First().Key != null)
        {
            enemyTank = enemyTanksFound.First().Key;
        }

        if (enemyBasesFound.Count > 0 && enemyBasesFound.First().Key != null)
        {
            enemyBase = enemyBasesFound.First().Key;
        }

       

        
        
        
        //ShouldAttackBase();

        

        //if low health or ammo, go searching
        /*if (TankCurrentHealth < 30 || TankCurrentAmmo < 4)
        {
            //if there is more than 0 consumables visible
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
            else
            {
                enemyTank = null;
                consumable = null;
                enemyBase = null;
                FollowPathToRandomWorldPoint(1f, heuristicMode);
            }
        }
        else
        {
            //if there is a enemy tank found
            if (enemyTanksFound.Count > 0 && enemyTanksFound.First().Key != null)
            {
                enemyTank = enemyTanksFound.First().Key;
                if (enemyTank != null)
                {
                    //get closer to target, and fire
                    if (Vector3.Distance(transform.position, enemyTank.transform.position) < attackDistance)
                    {
                        TurretFireAtPoint(enemyTank);
                    }
                    //else follow
                    else
                    {
                        FollowPathToWorldPoint(enemyTank, 1f, heuristicMode);
                    }
                }
            }
            else if (consumablesFound.Count > 0)
            {
                //if consumables are found, go to it.
                consumable = consumablesFound.First().Key;
                FollowPathToWorldPoint(consumable, 1f, heuristicMode);
            }
            else if (enemyBasesFound.Count > 0)
            {
                //if base if found
                enemyBase = enemyBasesFound.First().Key;
                if (enemyBase != null)
                {
                    //go close to it and fire
                    if (Vector3.Distance(transform.position, enemyBase.transform.position) < attackDistance)
                    {
                        TurretFireAtPoint(enemyBase);
                    }
                    else
                    {
                        FollowPathToWorldPoint(enemyBase, 1f, heuristicMode);

                    }
                }
            }
            else
            {
                //searching
                enemyTank = null;
                consumable = null;
                enemyBase = null;
                FollowPathToRandomWorldPoint(1f, heuristicMode);
                t += Time.deltaTime;
                if (t > 10)
                {
                    GenerateNewRandomWorldPoint();
                    t = 0;
                }
            }
            
        }*/
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
