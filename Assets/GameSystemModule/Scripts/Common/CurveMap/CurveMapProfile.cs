using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Utility.CurveMap
{
    [CreateAssetMenu(menuName = "GameData/Utility/CurveMapProfile")]
    public class CurveMapProfile : ScriptableObject
    {
        public string mapName;

        [System.Serializable]
        public class CurveData
        {
            public string name;
            public AnimationCurve curve;
        }
        public List<CurveData> curves = new List<CurveData>();


#if UNITY_EDITOR
        public static CurveMapProfile CreateInstance(string name, string path)
        {
            var data = CreateInstance<CurveMapProfile>();
            UnityEditor.AssetDatabase.CreateAsset(data, path + name);
            UnityEditor.EditorUtility.SetDirty(data);
            UnityEditor.AssetDatabase.SaveAssets();
            return data;
        }
#endif
    }
}
