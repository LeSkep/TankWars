using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RuleFSMRBS 
{
    public string antecedentA; // variable to store antecedentA
    public string antecedentB; // variable to store antecedentB
    public Type consequent; // variable to store consequent
    public Predicate compare; // variable to store predicated component, And, Or, nAnd
    public enum Predicate
    {And , Or, nAnd};
    // Creates the format and types for creating a rule
    public RuleFSMRBS(string antecedentA, string antecedentB, Type consequent, Predicate compare)
    {
        this.antecedentA = antecedentA;
        this.antecedentB = antecedentB;
        this.consequent = consequent;
        this.compare = compare;
    }
    // New dictionary called check rule of type Type that stores strings and bools
    public Type CheckRule(Dictionary<string, bool> facts)
    {
        bool antecedentABool = facts[antecedentA]; // A variable to see if the stored antecedent is true or false
        bool antecedentBBool = facts[antecedentB]; // A variable to see if the stored antecedent is true or false
        // A switch statement to compare antecedents set the consequent to true or false
        // For each case, And, nAnd, Or
        switch (compare)
        {
            case Predicate.And :
                if (antecedentABool && antecedentBBool)
                {
                    return consequent;
                }
                else
                {
                    return null;
                }
                

            case Predicate.Or :
                if (antecedentABool || antecedentBBool)
                {
                    return consequent;
                }
                else
                {
                    return null;
                }
                

            case Predicate.nAnd :
                if(!antecedentABool && !antecedentBBool)
                {
                    return consequent;
                }
                else
                {
                    return null;
                }
                
            default:
                return null;
        }
    }
}
