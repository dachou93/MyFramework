using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalScheduler : MonoSingleton<GlobalScheduler>
{
    event Action updateEvent;
    event Action GUIEvent;
    event Action destoryEvent;
    event Action fixedUpdateEvent;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void AddUpdateEvent(Action action)
    {
        updateEvent += action;
    }

    public void RemoveUpdateEvent(Action action)
    {
        updateEvent -= action;
    }

    public void ClearAllEvent()
    {
        updateEvent = null;
    }

    // Update is called once per frame
    void Update()
    {
        updateEvent?.Invoke();
    }

    public void AddGUIEvent(Action action)
    {
        GUIEvent += action;
    }

    public void RemoveGUIEvent(Action action)
    {
        GUIEvent -= action;
    }

    public void ClearAllGUIEvent()
    {
        GUIEvent = null;
    }

    void OnGUI()
    {
        GUIEvent?.Invoke();
    }

    public void AddDestoryEvent(Action action)
    {
        destoryEvent += action;
    }

    public void RemoveDestoryEvent(Action action)
    {
        destoryEvent -= action;
    }

    public void ClearAllDestoryEvent()
    {
        destoryEvent = null;
    }

    private void OnDestroy()
    {
        destoryEvent?.Invoke();
    }

    public void RemoveFixedUpdateEvent(Action action)
    {
        fixedUpdateEvent -= action;
    }

    public void ClearAllFixedUpdateEvent()
    {
        fixedUpdateEvent = null;
    }

    public void AddFixedUpdateEvent(Action action)
    {
        fixedUpdateEvent += action;
    }

    private void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }
}
