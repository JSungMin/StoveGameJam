using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseVariableObject<T1> : ScriptableObject, ISerializationCallbackReceiver
{
    public T1 initialValue;
    [System.NonSerialized]
    public T1 runtimeValue;

    public static T2 GetClone<T2>(T2 e) where T2 : UnityEngine.Object
    {
        return ScriptableObject.Instantiate(e);
    }
    public void OnAfterDeserialize()
    {
        runtimeValue = initialValue;
    }
    public void OnBeforeSerialize() { }
}
public abstract class BaseVariableObjectArray<T1> : ScriptableObject, ISerializationCallbackReceiver
{
    public List<T1> initialValue = new List<T1>();
    [System.NonSerialized]
    public List<T1> runtimeValue = new List<T1>();
    public static T2 GetClone<T2>(T2 e) where T2 : ScriptableObject
    {
        return ScriptableObject.Instantiate(e) as T2;
    }
    public void OnAfterDeserialize()
    {
        runtimeValue = initialValue;
    }
    public void OnBeforeSerialize() { }
}