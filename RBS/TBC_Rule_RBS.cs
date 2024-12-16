using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Rule 
{
    public string antecedentA;
    public string antecedentB;
    public string consequent;
    public Predicate compare;
    public enum Predicate
    {And , Or, nAnd};

    public Rule(string antecedentA, string antecedentB, string consequent, Predicate compare)
    {
        this.antecedentA = antecedentA;
        this.antecedentB = antecedentB;
        this.consequent = consequent;
        this.compare = compare;
    }

    public Dictionary<string, bool> CheckRule(Dictionary<string, bool> facts)
    {
        bool antecedentABool = facts[antecedentA];
        bool antecedentBBool = facts[antecedentB];

        switch (compare)
        {
            case Predicate.And :
                if (antecedentABool && antecedentBBool)
                {
                    facts[consequent] = true;
                }
                else
                {
                    facts[consequent] = false;
                }
                return facts;

            case Predicate.Or :
                if (antecedentABool || antecedentBBool)
                {
                    facts[consequent] = true;
                }
                else
                {
                    facts[consequent] = false;
                }
                return facts;

            case Predicate.nAnd :
                if(!antecedentABool && !antecedentBBool)
                {
                    facts[consequent] = true;
                }
                else
                {
                    facts[consequent] = false;
                }
                return facts;

            default:
                return null;
        }
    }
}
