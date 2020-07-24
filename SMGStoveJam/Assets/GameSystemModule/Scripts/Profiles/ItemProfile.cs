using System.Collections.Generic;
using CoreSystem.ProfileComponents;
using UnityEngine;

namespace CoreSystem
{
    public class ItemProfile : ScriptableObject
    {
        [System.Serializable]
        public class SoundEffects
        {
            public SoundProfile pickSound;
            public SoundProfile useSound;
            public SoundProfile dropSound;
        }
        public string itemName;
        public string description;
        public Sprite itemIcon;
        public ItemType itemType;
        public FoodCookDiv foodCookDiv;
        public FoodBigDiv foodBigDiv;
        public FoodMiddleDiv foodMiddleDiv;
        //  When Type is Equipment then Developer need to decide which part
        public int equipPartID;
        //  Equipment 일경우엔 Max HP, MP가 증감
        public DynamicGauge hp;
        
        //  When Developer Create Blueprint ItemProfile
        //  Must care about Set Correct Blueprint 
        public BlueprintProfile blueprint;
        public Stat influence = new Stat();
        public float price = 0f;
        //  조리준비 시간
        public float prepareDuration = 0f;
        //  조리 시간
        public float cookDuration = 0f;
        public SoundEffects soundEffects;
        public GameObject fxObject;
        public List<StatusEffectProfile> statusEffects = new List<StatusEffectProfile>();

        public bool HavePrepareDuration => (foodCookDiv == FoodCookDiv.구이 ||
                                            foodCookDiv == FoodCookDiv.튀김 ||
                                            foodCookDiv == FoodCookDiv.탕);

        public void Initialize()
        {
            influence.Initialize();
        }
    #if UNITY_EDITOR
        public static ItemProfile CreateInstance()
        {
            var itemData = CreateInstance<ItemProfile>();
            var pathOfItems = SystemPathes.GetSystemPath(SystemPathes.PathType.ItemProfileSet);
            var parentPath = PathHelper.GetParentPath(pathOfItems);
            var itemCount = ItemProfileSet.CountOfElements();
            UnityEditor.AssetDatabase.CreateAsset(
                itemData,
                PathHelper.Bind(parentPath, "NewItem_" + itemCount + ".asset")
            );
            UnityEditor.EditorUtility.SetDirty(itemData);
            itemData.Initialize();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.SaveAssets();
            return itemData;
        }
    #endif
    }
}