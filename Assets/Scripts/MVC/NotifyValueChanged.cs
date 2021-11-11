using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class NotifyValueChanged<T> where T : new()
{
    T valueField = new T();

    public T value {
        get
        {
            return valueField;
        }
        set
        {
            valueField = value;

            onValueChanged.Invoke(value);
        }
    }

    public NotifyValueChanged()
    {
        if (valueField.GetType().IsClass)
        {
            valueField = new T();
        }
    }

    public class ValueChangedEvent : UnityEvent<T>
    {

    }

    public ValueChangedEvent onValueChanged = new ValueChangedEvent();


    [Obsolete("This method is replaced by simply using this.value. The default behaviour has been changed to notify when changed. If the behaviour is not to be notified, SetValueWithoutNotify() must be used.", false)]
    public void SetValueAndNotify(T newValue)
    {
        value = newValue;

        // 通知数据发生变化
    }

    // void SetValueWithoutNotify(T newValue);

    public void NotifyChanged()
    {
        onValueChanged.Invoke(value);
    }

    //void RemoveOnValueChanged(EventCallback<ChangeEvent<T>> callback);
}
