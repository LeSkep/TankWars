using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// The base state creates functions to be used within the states using overrides
// BaseState class also uses polymorphism as it can be employed by all three scripts with no added changes
public abstract class BaseState 
{
    public abstract Type StateUpdate();
    public abstract Type StateEnter();
    public abstract Type StateExit();
}
