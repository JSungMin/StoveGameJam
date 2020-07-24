using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.ProfileComponents
{
    [Serializable]
    public class MetaEquipPartData : VariableStringArray
    {
        public List<TString> Elements => initialValue;
        public List<string> Names => Elements.Select(x => x.name).ToList();
        public List<string> Values => Elements.Select(x => x.val).ToList();
        public int Count => initialValue.Count;
        
        public void CopyTo(MetaEquipPartData to)
        {
            to.initialValue = initialValue;
        }
    #if UNITY_EDITOR
        public static MetaEquipPartData CreateMetaData()
        {
            MetaEquipPartData n = CreateInstance<MetaEquipPartData>();
            n.name = "MetaEquipPartData";
            string[] arr = new string[] { "Helmet", "Arm","Body","Leg","Weapon","Shield"};
            for (int i = 0; i < arr.Length; i++)
            {
                TString tmpString = new TString();
                tmpString.name = tmpString.val = arr[i];
                n.initialValue.Add(tmpString);
            }
            UnityEditor.AssetDatabase.CreateAsset(
                n,
                SystemPathes.pathOfMetaEquipPartData
            );
            CoreDataSet.Instance.metaEquipPartData = n;
            return n;
        }
    #endif
    }
}
