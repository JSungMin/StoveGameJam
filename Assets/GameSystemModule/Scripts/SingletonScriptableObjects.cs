using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject {
    protected static T _instance = null;
    public static T Instance
    {
        get
        {
            if (!_instance)
            {
#if UNITY_EDITOR
                _instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
#else
                _instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
#endif
            }
            return _instance;
        }
        set
        {
            if (!_instance)
                _instance = value;
        }
    }
}
/// <summary>
/// Create Singleton Sciptable Object That Manage UnityEngine.Object>
/// T1 is Manager Script Type, T2 is Managed Object Script Type
/// </summary>
/// <typeparam name="T1">Manager Script Name</typeparam>
/// <typeparam name="T2">Managed Script Name</typeparam>
public abstract class SingletonManagerObject<T1, T2> : SingletonScriptableObject<T1>
    where T1 : SingletonManagerObject<T1, T2>
    where T2 : UnityEngine.Object
{
    public List<T2> elements;

    public static T2 GetElement(int idx)
    {
        if (Instance.elements.Count <= idx)
            return null;
        return Instance.elements[idx];
    }
    public static T2 GetCloneElement(int idx)
    {
        var origin = GetElement(idx);
        var clone = (T2)ScriptableObject.Instantiate(origin);
        Type cloneType = origin.GetType();
        FieldInfo[] fields = cloneType.GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            var fieldType = fields[i].FieldType;
            var fieldVal = fields[i].GetValue(clone);
            bool isUnityObject = TypeHelper.HaveBaseType(fieldType, typeof(UnityEngine.Object));
            if (isUnityObject)
            {
                var cloneField = Instantiate((UnityEngine.Object)fieldVal);
                fields[i].SetValue(clone, cloneField);
            }
        }
        return clone;
    }
    public static T2 GetCloneElement(T2 origin)
    {
        var clone = (T2)Instantiate(origin);
        Type cloneType = origin.GetType();
        FieldInfo[] fields = cloneType.GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            var fieldType = fields[i].FieldType;
            var fieldVal = fields[i].GetValue(clone);
            bool isUnityObject = TypeHelper.HaveBaseType(fieldType, typeof(UnityEngine.Object));
            if (isUnityObject)
            {
                var cloneField = Instantiate((UnityEngine.Object)fieldVal);
                fields[i].SetValue(clone, cloneField);
            }
        }
        return clone;
    }
    public static int CountOfElements()
    {
        return Instance.elements.Count;
    }
#if UNITY_EDITOR
    public static void AddElement(T2 e)
    {
        UnityEditor.EditorUtility.SetDirty(Instance);
        if (!Instance.elements.Contains(e))
            Instance.elements.Add(e);
    }
    public static void RemoveElement(T2 e)
    {
        Instance.elements.Remove(e);
        Instance.elements.RemoveAll(x => x == null);
        string assetPath = UnityEditor.AssetDatabase.GetAssetOrScenePath(e);
        Debug.Log("Delete Element <" + typeof(T2).ToString() + ">" + " : " + assetPath);
        UnityEditor.AssetDatabase.DeleteAsset(assetPath);
    }
    public static void RemoveElement(int idx, T2 e)
    {
        Instance.elements.Remove(e);
        Instance.elements.RemoveAll(x => x == null);
        string assetPath = UnityEditor.AssetDatabase.GetAssetOrScenePath(e);
        Debug.Log("Delete Element <" + typeof(T2).ToString() + ">" + " : " + assetPath);
        UnityEditor.AssetDatabase.DeleteAsset(assetPath);
    }
#endif
}