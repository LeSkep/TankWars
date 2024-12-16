using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RuleFSMRBS 
{
    public string antecedentA;
    public string antecedentB;
    public Type consequent;
    public Predicate compare;
    public enum Predicate
    {And , Or, nAnd};

    public RuleFSMRBS(string antecedentA, string antecedentB, Type consequent, Predicate compare)
    {
        this.antecedentA = antecedentA;
        this.antecedentB = antecedentB;
        this.consequent = consequent;
        this.compare = compare;
    }

    public Type CheckRule(Dictionary<string, bool> facts)
    {
        bool antecedentABool = facts[antecedentA];
        bool antecedentBBool = facts[antecedentB];

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
