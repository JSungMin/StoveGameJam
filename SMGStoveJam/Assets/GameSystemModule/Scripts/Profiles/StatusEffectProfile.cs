using CoreSystem.ProfileComponents;
using UnityEngine;

namespace CoreSystem
{
    public class StatusEffectProfile : ScriptableObject
    {
        public string effectName;
        public string description;
        public Sprite effectIcon;
        public StatusAffectMethod statusType;
        public int level;
        public float duration;
        public AnimationCurve influenceCurve = new AnimationCurve();
        public Stat influence = new Stat();

        public void OnEnable()
        {
            influence.Initialize();
        }
    #if UNITY_EDITOR
        public static StatusEffectProfile CreateInstance()
        {
            var statusEffect = CreateInstance<StatusEffectProfile>();
            var pathOfStatusEffect = SystemPathes.pathOfStatusEffectSet;
            var parentPath = PathHelper.GetParentPath(pathOfStatusEffect);
            var descCount = StatusEffectProfileSet.CountOfElements();

            UnityEditor.AssetDatabase.CreateAsset(
                statusEffect, 
                PathHelper.Bind(parentPath, "Status_" + descCount + ".asset")
            );
            UnityEditor.EditorUtility.SetDirty(statusEffect);

            ////  Make Stat Data
            //statusEffect.influence = StatMetaData.GetCloneFromMetaData("Infulence",statusEffect);

            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.SaveAssets();
            return statusEffect;
        }
        public static void RemoveInstance(StatusEffectProfile statusEffect)
        {
            string statusPath = UnityEditor.AssetDatabase.GetAssetOrScenePath(statusEffect);
            Debug.Log("Delete Status Effect : " + statusPath);
            UnityEditor.AssetDatabase.DeleteAsset(statusPath);
        }
    #endif
    }
}
