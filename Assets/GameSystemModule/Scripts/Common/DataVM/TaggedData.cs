using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[System.Serializable]
public class TaggedData
{
    public enum DataType
    {
        Float,
        Int,
        String,
        Boolean
    }
    public DataType dataType = DataType.Float;
    public string dataName;
    [SerializeField]
    private float floatData;
    [SerializeField]
    private int intData;
    [SerializeField]
    private string stringData;
    [SerializeField]
    private bool boolData;

    public Action<object> onChanged;

    public TaggedData()
    {
        dataName = "";
    }
    public TaggedData(string n, DataType t, object o)
    {
        dataName = n;
        dataType = t;
        switch (dataType)
        {
            case DataType.Float:
                floatData = (float) o;
                break;
            case DataType.Int:
                intData = (int) o;
                break;
            case DataType.String:
                stringData = (string) o;
                break;
            case DataType.Boolean:
                boolData = (bool) o;
                break;
        }
    }

    public static TaggedData operator +(TaggedData a, TaggedData b)
    {
        switch (a.dataType)
        {
            case DataType.Float:
                a.FloatData += b.FloatData;
                break;
            case DataType.Int:
                a.IntData += b.IntData;
                break;
            case DataType.String:
                a.StringData += b.StringData;
                break;
            case DataType.Boolean:
                a.BoolData = a.BoolData || b.BoolData;
                break;
        }
        return a;
    }

    public static TaggedData operator -(TaggedData a, TaggedData b)
    {
        switch (a.dataType)
        {
            case DataType.Float:
                a.FloatData -= b.FloatData;
                break;
            case DataType.Int:
                a.IntData -= b.IntData;
                break;
            case DataType.String:
                var idx = a.StringData.IndexOf(b.StringData);
                if (idx >= 0)
                    a.StringData = a.StringData.Remove(idx, b.StringData.Length);
                break;
            case DataType.Boolean:
                a.BoolData = a.BoolData || b.BoolData;
                break;
        }
        return a;
    }

    public static bool operator <(TaggedData a, TaggedData b)
    {
        switch (a.dataType)
        {
            case DataType.Float:
                return a.FloatData < b.FloatData;
            case DataType.Int:
                return a.IntData < b.IntData;
            case DataType.String:
                return a.StringData.Length < b.StringData.Length;
        }
        return false;
    }

    public static bool operator >(TaggedData a, TaggedData b)
    {
        switch (a.dataType)
        {
            case DataType.Float:
                return a.FloatData > b.FloatData;
            case DataType.Int:
                return a.IntData > b.IntData;
            case DataType.String:
                return a.StringData.Length > b.StringData.Length;
        }
        return false;
    }

    public static bool operator <=(TaggedData a, TaggedData b)
    {
        switch (a.dataType)
        {
            case DataType.Float:
                return a.FloatData <= b.FloatData;
            case DataType.Int:
                return a.IntData <= b.IntData;
            case DataType.String:
                return a.StringData.Length <= b.StringData.Length;
        }
        return false;
    }

    public static bool operator >=(TaggedData a, TaggedData b)
    {
        switch (a.dataType)
        {
            case DataType.Float:
                return a.FloatData >= b.FloatData;
            case DataType.Int:
                return a.IntData >= b.IntData;
            case DataType.String:
                return a.StringData.Length >= b.StringData.Length;
        }
        return false;
    }

    public static bool operator ==(TaggedData a, TaggedData b)
    {
        if (ReferenceEquals(a, null))
            return ReferenceEquals(b, null);
        else if (ReferenceEquals(b, null))
            return false;
        switch (a.dataType)
        {
            case DataType.Float:
                return a.FloatData == b.FloatData;
            case DataType.Int:
                return a.IntData == b.IntData;
            case DataType.String:
                return a.StringData == b.StringData;
            case DataType.Boolean:
                return a.BoolData == b.BoolData;
        }
        return false;
    }

    public static bool operator !=(TaggedData a, TaggedData b)
    {
        switch (a.dataType)
        {
            case DataType.Float:
                return a.FloatData != b.FloatData;
            case DataType.Int:
                return a.IntData != b.IntData;
            case DataType.String:
                return a.StringData != b.StringData;
            case DataType.Boolean:
                return a.BoolData != b.BoolData;
        }
        return false;
    }

    public string PrintData()
    {
        var result = dataName;
        result += "(";
        if (dataType == DataType.Float)
            result += "F:" + FloatData;
        else if (dataType == DataType.Int)
            result += "I:" + IntData;
        else if (dataType == DataType.String)
            result += "S:" + StringData;
        else if (dataType == DataType.Boolean)
            result += "B:" + BoolData;
        result += ")";
        return result;
    }
    public float FloatData
    {
        set
        {
            onChanged?.Invoke(value);
            floatData = value;
        }
        get { return floatData; }
    }
    public int IntData
    {
        set
        {
            onChanged?.Invoke(value);
            intData = value;
        }
        get { return intData; }
    }

    public string StringData
    {
        set
        {
            onChanged?.Invoke(value);
            stringData = value;
        }
        get { return stringData; }
    }

    public bool BoolData
    {
        set
        {
            onChanged?.Invoke(value);
            boolData = value;
        }
        get { return boolData; }
    }
}
