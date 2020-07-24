using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameData/VMMacroSet")]
public class VMMacroSet : ScriptableObject
{
    public DataWrapper[] macroSet;

    public TaggedData[] GetMatchedParameters(string methodName)
    {
        var wrapper = macroSet.FirstOrDefault(x => x.methodName == methodName);
        if (null == wrapper)
            return null;
        var result = new TaggedData[wrapper.parameters.Length + 1];
        result[0] = new TaggedData("MethodName", TaggedData.DataType.String, methodName);
        for (var i = 0; i < wrapper.parameters.Length; i++)
        {
            result[i + 1] = wrapper.parameters[i];
        }
        return result;
    }

#if UNITY_EDITOR
    public static VMMacroSet CreateInstance(string path, string name)
    {
        var data = CreateInstance<VMMacroSet>();
        var pathOfData = PathHelper.Bind(path, name + ".asset");
        UnityEditor.AssetDatabase.CreateAsset(
            data,
            pathOfData
        );
        UnityEditor.EditorUtility.SetDirty(data);

        UnityEditor.AssetDatabase.Refresh();
        UnityEditor.AssetDatabase.SaveAssets();
        return data;
    }
#endif

    [System.Serializable]
    public class DataWrapper
    {
        public string methodName;
        public TaggedData[] parameters;
    }
}
