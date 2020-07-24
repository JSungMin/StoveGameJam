using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class DataContainer
{
    [SerializeField]
    private List<TaggedData> dataList = new List<TaggedData>();

    public DataContainer()
    {
        dataList = new List<TaggedData>();
    }

    public DataContainer(List<TaggedData> datas)
    {
        dataList = new List<TaggedData>(datas);
    }

    public static DataContainer GetClone(DataContainer origin)
    {
        var clone = new DataContainer(origin.Datas);
        Type cloneType = origin.GetType();
        FieldInfo[] fields = cloneType.GetFields();
        for (var i = 0; i < fields.Length; i++)
        {
            var fieldType = fields[i].FieldType;
            var fieldVal = fields[i].GetValue(clone);
            bool isUO = TypeHelper.HaveBaseType(fieldType, typeof(UnityEngine.Object));
            if (isUO)
            {
                var cloneField = ScriptableObject.Instantiate((UnityEngine.Object) fieldVal);
                fields[i].SetValue(clone, cloneField);
            }
        }
        return clone;
    }
    public void Clear()
    {
        dataList.Clear();
    }

    public bool HasKey(string name)
    {
        return dataList.FirstOrDefault(x => x.dataName == name) != null;
    }

    public TaggedData GetTaggedData(string name)
    {
        var data = dataList.FirstOrDefault(x => x.dataName == name);
        return data;
    }

    public void RemoveTaggedData(string name)
    {
        dataList.Remove(GetTaggedData(name));
    }

    public void AddListener(string name, Action<object> action)
    {
        GetTaggedData(name).onChanged += action;
    }

    public void RemoveListener(string name, Action<object> action)
    {
        var onChanged = GetTaggedData(name).onChanged;
        if (onChanged != null) GetTaggedData(name).onChanged -= action;
    }
    public void SetFloat(string name, float val)
    {
        var data = GetTaggedData(name);
        if (null == data)
            dataList.Add(new TaggedData(name, TaggedData.DataType.Float, val));
        else
            data.FloatData = val;
    }

    public float GetFloat(string name)
    {
        var data = GetTaggedData(name);
        return null == data ? 0f : GetTaggedData(name).FloatData;
    }

    public void SetInt(string name, int val)
    {
        var data = GetTaggedData(name);
        if (null == data)
            dataList.Add(new TaggedData(name, TaggedData.DataType.Int, val));
        else
            data.IntData = val;
    }

    public int GetInt(string name)
    {
        var data = GetTaggedData(name);
        return null == data ? 0 : GetTaggedData(name).IntData;
    }

    public void SetString(string name, string val)
    {
        var data = GetTaggedData(name);
        if (null == data)
            dataList.Add(new TaggedData(name, TaggedData.DataType.String, val));
        else
            data.StringData = val;
    }

    public string GetString(string name)
    {
        return GetTaggedData(name)?.StringData;
    }

    public void SetBool(string name, bool val)
    {
        var data = GetTaggedData(name);
        if (null == data)
            dataList.Add(new TaggedData(name, TaggedData.DataType.Boolean, val));
        else
            data.BoolData = val;
    }

    public bool GetBool(string name)
    {
        var data = GetTaggedData(name);
        return null != data && GetTaggedData(name).BoolData;
    }

    public List<TaggedData> Datas
    {
        set
        {
            dataList.Clear();
            foreach (var val in value)
            {
                switch (val.dataType)
                {
                    case TaggedData.DataType.Float:
                        SetFloat(val.dataName, val.FloatData);
                        break;
                    case TaggedData.DataType.Int:
                        SetInt(val.dataName, val.IntData);
                        break;
                    case TaggedData.DataType.String:
                        SetString(val.dataName, val.StringData);
                        break;
                    case TaggedData.DataType.Boolean:
                        SetBool(val.dataName, val.BoolData);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        get { return dataList; }
    }
    public int DataCount
    {
        get
        {
            if (dataList == null)
                dataList = new List<TaggedData>();
            return Datas.Count;
        }
    }

    public int FloatCount => dataList.Count(x => x.dataType == TaggedData.DataType.Float);
    public int IntCount => dataList.Count(x => x.dataType == TaggedData.DataType.Int);
    public int StringCount => dataList.Count(x => x.dataType == TaggedData.DataType.String);
    public int BooleanCount => dataList.Count(x => x.dataType == TaggedData.DataType.Boolean);
}
