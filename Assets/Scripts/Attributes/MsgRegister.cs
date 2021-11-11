using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgRegister : Attribute
{
    public string Msg { get; set; }
    public MsgRegister(string Msg)
    {
        this.Msg = Msg;
    }
}
