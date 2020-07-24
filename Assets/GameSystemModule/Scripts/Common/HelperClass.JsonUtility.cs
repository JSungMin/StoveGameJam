using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonHelper
{
    public static string PersistentObjectToJsonFile(object obj, string relativePath)
    {
        string filePath = "";
        filePath = PathHelper.Bind(Application.persistentDataPath, relativePath);

        var json = JsonUtility.ToJson(obj);
        File.WriteAllText(filePath, json);
        Debug.Log("Json Saved : " + filePath);
        return json;
    }
    public static void PersistentJsonFileToObject(string relativePath, object obj)
    {
        string filePath = "";
        filePath = PathHelper.Bind(Application.persistentDataPath, relativePath);
        var json = File.ReadAllText(filePath);
        JsonUtility.FromJsonOverwrite(json, obj);
    }
    public static string DataObjectToJsonFile(object obj, string relativePath)
    {
        string filePath = "";
        filePath = PathHelper.Bind(Application.dataPath, relativePath);

        var json = JsonUtility.ToJson(obj);
        File.WriteAllText(filePath, json);
        Debug.Log("Json Saved : " + filePath);
        return json;
    }
    public static void DataJsonFileToObject(string relativePath, object obj)
    {
        string filePath = "";
        filePath = PathHelper.Bind(Application.dataPath, relativePath);
        var json = File.ReadAllText(filePath);
        JsonUtility.FromJsonOverwrite(json, obj);
    }
}