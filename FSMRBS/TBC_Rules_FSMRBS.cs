using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesFSMRBS  
{
    public void AddRule(RuleFSMRBS rule)
    {
        GetRules.Add(rule);
    }

    public List<RuleFSMRBS> GetRules { get; } = new List<RuleFSMRBS>();
}
