using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState 
{
    public abstract Type StateUpdate();
    public abstract Type StateEnter();
    public abstract Type StateExit();
}
