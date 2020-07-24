using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.ProfileComponents
{
    [Serializable]
    public class MetaStatData : VariableFloatArray
    {
        public List<TFloat> Elements => initialValue;
        public List<string> Names => Elements.Select(x => x.name).ToList();
        public List<float> Values => Elements.Select(x => x.val).ToList();
        public int Count => initialValue.Count;

        public void CopyTo(MetaStatData to)
        {
            to.initialValue = initialValue;
        }
    #if UNITY_EDITOR
        public static MetaStatData CreateMetaData()
        {
            MetaStatData n = CreateInstance<MetaStatData>();
            n.name = "MetaStatData";
            for (int i = 0; i < 5; i++)
            {
                TFloat tmpFloat = new TFloat
                {
                    name = "Stat" + i,
                    val = 0
                };
                n.initialValue.Add(tmpFloat);
            }
            UnityEditor.AssetDatabase.CreateAsset(
                n,
                SystemPathes.pathOfMetaStatData
            );
            UnityEditor.EditorUtility.SetDirty(n);
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.SaveAssets();
            CoreDataSet.Instance.metaStatData = n;
            return n;
        }
    #endif
    }
}