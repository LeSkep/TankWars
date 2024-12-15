using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesFSMRBS  
{
    // AddRule function
    public void AddRule(RuleFSMRBS rule)
    {
        GetRules.Add(rule); // Adds the rule to the GetRules dictionary
    }
    // New dictionary called GetRules
    public List<RuleFSMRBS> GetRules { get; } = new List<RuleFSMRBS>();
}
