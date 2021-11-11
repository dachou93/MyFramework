using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Reflection;
using System;

public class UIEventBindMgr : MonoSingleton<UIEventBindMgr>
{
    public void BindUIEvent(GameObject go, UIBindEvent uiEvent, object uiEventReciver, string bindMethodNameOrFieldName/*UnityAction<BaseEventData> callback*/)
    {
        // 结束编辑
        if (uiEvent == UIBindEvent.InputField_EndEdit)
        {
            InputField inputField = go.GetComponent<InputField>();
            if (inputField)
            {
                Type t = uiEventReciver.GetType();
                MethodInfo methodInfo = t.GetMethod(bindMethodNameOrFieldName);
                if (methodInfo != null)
                {
                    inputField.onEndEdit.AddListener((string inputStr) =>
                    {
                        methodInfo.Invoke(uiEventReciver, new object[] { inputStr });
                    });
                }
            }
            else
            {
                Debug.LogError("GameObject need add InputField Component");
            }
        }
        else if (uiEvent == UIBindEvent.InputField_ValidateInput) // 每次输入单个字符验证
        {
            InputField inputField = go.GetComponent<InputField>();
            if (inputField)
            {
                Type t = uiEventReciver.GetType();
                MethodInfo methodInfo = t.GetMethod(bindMethodNameOrFieldName);
                if (methodInfo != null)
                {
                    inputField.onValidateInput = (string text, int charIndex, char addedChar) =>
                    {
                        return (char)methodInfo.Invoke(uiEventReciver, new object[] { text, charIndex, addedChar });
                    };
                }
            }
            else
            {
                Debug.LogError("GameObject need add InputField Component");
            }
        }
        else if (uiEvent == UIBindEvent.InputField_ValueChanged) // 输入数据发生变化
        {
            InputField inputField = go.GetComponent<InputField>();
            if (inputField)
            {
                Type t = uiEventReciver.GetType();
                MethodInfo methodInfo = t.GetMethod(bindMethodNameOrFieldName);
                if (methodInfo != null)
                {
                    inputField.onValueChanged.AddListener((string inputStr) =>
                    {
                        methodInfo.Invoke(uiEventReciver, new object[] { inputStr });
                    });
                }
            }
            else
            {
                Debug.LogError("GameObject need add InputField Component");
            }
        }
        else if (uiEvent == UIBindEvent.Click)
        {
            Type t = uiEventReciver.GetType();
            MethodInfo methodInfo = t.GetMethod(bindMethodNameOrFieldName);

            // 需要完善错误处理
            Button btn = go.GetComponent<Button>();
            if (btn)
            {
                btn.onClick.AddListener(() => {
                    methodInfo.Invoke(uiEventReciver, new object[] { });
                });
            }
            else
            {
                Debug.LogError("GameObject need add Button Component");
            }
        }
        else if (uiEvent == UIBindEvent.Get_Component)
        {
            Type viewType = uiEventReciver.GetType();

            FieldInfo fieldInfo = viewType.GetField(bindMethodNameOrFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            Type componentType = fieldInfo.FieldType;

            Component component = go.GetComponent(componentType);
            if (component)
            {
                // 需要完善错误处理
                fieldInfo.SetValue(uiEventReciver, component);
            }
            else
            {
                Debug.LogError($"GameObject need add {component.name} Component");
            }
        }
        else if (uiEvent == UIBindEvent.Init_Component)
        {
            Type viewType = uiEventReciver.GetType();
            // 需要完善错误处理
            viewType.GetMethod(bindMethodNameOrFieldName).Invoke(uiEventReciver, new object[] { go });
        }
        else if (uiEvent == UIBindEvent.Dropdown_options) // 选择数据发生变化
        {
            Dropdown dp = go.GetComponent<Dropdown>();
            if (dp)
            {
                Type t = uiEventReciver.GetType();
                MethodInfo methodInfo = t.GetMethod(bindMethodNameOrFieldName);
                if (methodInfo != null)
                {
                    dp.onValueChanged.AddListener((value) =>
                    {
                        methodInfo.Invoke(uiEventReciver, new object[] { value });
                    });
                }
            }
            else
            {
                Debug.LogError("GameObject need add Dropdown Component");
            }
        }
        else
        {
            var eventTrigger = go.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (!eventTrigger)
            {
                eventTrigger = go.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            }

            var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
            eventTrigger.triggers.Add(entry);

            switch (uiEvent)
            {
                case UIBindEvent.PointerEnter:
                    entry.eventID = EventTriggerType.PointerEnter;
                    Type t = uiEventReciver.GetType();
                    MethodInfo methodInfo = t.GetMethod(bindMethodNameOrFieldName);
                    entry.callback.AddListener((BaseEventData d) => {
                        methodInfo.Invoke(uiEventReciver, new object[] { d });
                    });
                    break;
                case UIBindEvent.PointerExit:
                    entry.eventID = EventTriggerType.PointerExit;
                    t = uiEventReciver.GetType();
                    methodInfo = t.GetMethod(bindMethodNameOrFieldName);
                    entry.callback.AddListener((BaseEventData d) => {
                        methodInfo.Invoke(uiEventReciver, new object[] { d });
                    });
                    break;
                case UIBindEvent.Click:
                    entry.eventID = EventTriggerType.PointerClick;
                    t = uiEventReciver.GetType();
                    methodInfo = t.GetMethod(bindMethodNameOrFieldName);
                    entry.callback.AddListener((BaseEventData d) => {
                        methodInfo.Invoke(uiEventReciver, new object[] { d });
                    });
                    break;
                default:
                    break;
            }
        }
    }

    public void AddMouseEventHandle()
    {

    }

    private void Update()
    {

    }
}

public class UIBind
{
    public string uiPathInParent;
    public UIBindEvent uiEvent;
    public string bindMethodNameOrFieldName;
    public object bindConfig;
}

public enum UIBindEvent
{
    // base event
    PointerEnter,
    PointerExit,
    PointerDown,
    PointerUp,
    DragBegin,
    Drag,
    DragEnd,
   

    // component event
    Init_Component,
    Click,
    InputField_EndEdit,
    InputField_ValidateInput,
    InputField_ValueChanged,
    Get_Component,
    Get_Components,
    Dropdown_options,

    // extend event
}

public struct EventArgs
{
    public Button button;
    public Text text;
    public Image img;
    public Slider slider;
    public ScrollRect scrollRect;
    public Dropdown dropdown;   
}

