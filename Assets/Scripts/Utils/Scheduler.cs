using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Scheduler : MonoSingleton<Scheduler>
{
    event Action updateEvent;

    event Action fixedUpdateEvent;

    public void Awake()
    {
        isGlobal = true;
    }

    public void AddUpdateEvent(Action action)
    {
        updateEvent += action;
    }

    public void AddFixedUpdateEvent(Action action)
    {
        fixedUpdateEvent += action;
    }

    public void RemoveUpdateEvent(Action action)
    {
        updateEvent -= action;
    }

    public void RemoveFixedUpdateEvent(Action action)
    {
        fixedUpdateEvent -= action;
    }

    public void ClearAllEvent()
    {
        updateEvent = null;
    }

    public void ClearAllFixedUpdateEvent()
    {
        fixedUpdateEvent = null;
    }

    // Update is called once per frame
    void Update()
    {
        updateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }
}
