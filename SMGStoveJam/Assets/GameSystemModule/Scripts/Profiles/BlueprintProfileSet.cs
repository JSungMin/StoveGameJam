using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreSystem
{
    public class BlueprintProfileSet : SingletonManagerObject<BlueprintProfileSet,BlueprintProfile> {

    #if UNITY_EDITOR
        public static BlueprintProfileSet CreateInstance()
        {
            var dataSet = CreateInstance<BlueprintProfileSet>();
            UnityEditor.AssetDatabase.CreateAsset(dataSet, SystemPathes.GetSystemPath(SystemPathes.PathType.BlueprintSet));
            UnityEditor.EditorUtility.SetDirty(dataSet);
            var mItems = PathHelper.FindAssets<BlueprintProfile>();
            dataSet.elements = mItems.Select(data => data).ToList();
            UnityEditor.AssetDatabase.Refresh();
            return dataSet;
        }
        public static void ReloadItems()
        {
            var mItems = PathHelper.FindAssets<BlueprintProfile>();
            Instance.elements = mItems.Select(data => data).ToList();
        }
    #endif
    }
}
