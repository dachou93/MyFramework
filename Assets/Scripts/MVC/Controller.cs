using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller
{
    public Controller()
    {
        Type type = this.GetType();
        BeanFactory.DoAutoWire(type, this);
        BeanFactory.DoMsgRegister(type, this);
        BeanFactory.DoUpdateCall(this);
        BeanFactory.DoFixedUpdateCall(this);
    }
}
