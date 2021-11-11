using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
[AttributeUsage(AttributeTargets.Field)]
public class AutoWired : Attribute
{
    public string UiPathInParent {
        get; set;
    }

    public UIBindEvent UIBindEvent {
        get;set;
    }

    public AutoWired(string uiPathInParent,UIBindEvent uIBindEvent)
    {
        UiPathInParent = uiPathInParent;
        UIBindEvent = uIBindEvent;
    }

    public AutoWired()
    {

    }
}
