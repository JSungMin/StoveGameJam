using System.Collections.Generic;
using CoreSystem.ProfileComponents;
using UnityEngine;

namespace CoreSystem
{
    public class BlueprintProfile : ScriptableObject {
        public ItemProfile targetItem;
        public List<ItemAmountWrapper> stuffList = new List<ItemAmountWrapper>();
        public bool consumeStuff = true;
        //public BigInteger price;

    #if UNITY_EDITOR
        public static BlueprintProfile CreateInstance()
        {
            var blueprint = CreateInstance<BlueprintProfile>();
            var pathOfBlueprint = SystemPathes.GetSystemPath(SystemPathes.PathType.BlueprintSet);
            var parentPath = PathHelper.GetParentPath(pathOfBlueprint);
            var blueprintCount = BlueprintProfileSet.CountOfElements();
            UnityEditor.AssetDatabase.CreateAsset(
                blueprint,
                PathHelper.Bind(parentPath, "NewBlueprint_" + blueprintCount + ".asset")
            );
            UnityEditor.EditorUtility.SetDirty(blueprint); UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.SaveAssets();
            return blueprint;
        }
    #endif
    }
}
