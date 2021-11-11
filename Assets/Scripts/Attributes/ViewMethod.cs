using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AttributeUsage(AttributeTargets.Method)]
public class ViewMethod : Attribute
{
    public string FieldName { get; set; }

    public UIBindEvent UIBindEvent { get; set; }
    public ViewMethod(string fieldName, UIBindEvent uievent)
    {
        FieldName = fieldName;
        UIBindEvent = uievent;
    }
}
