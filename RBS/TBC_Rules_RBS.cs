using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rules 
{
    // AddRule function
    public void AddRule(Rule rule)
    {
        GetRules.Add(rule); // Adds the rule to the GetRules dictionary
    }
    // New dictionary called GetRules
    public List<Rule> GetRules { get; } = new List<Rule>();
}
