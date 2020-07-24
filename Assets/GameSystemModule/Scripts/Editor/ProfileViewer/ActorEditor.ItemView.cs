using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CoreSystem.Editor.ProfileViewer
{
    public partial class ActorEditor : EditorWindow {

        private static void OnItemView()
        {
            var scdRect = EditorGUILayout.BeginVertical();
            //  Status Effect Select, Create, Delete Area (SCD Area)
            #region SCD Area
            EditorGUILayout.BeginHorizontal();
    #pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
            if (selectedItem == null && ItemProfileSet.CountOfElements() != 0)
            {
                selectedItem = ItemProfileSet.GetElement(0);
            }
            selectedItem = (ItemProfile)EditorGUILayout.ObjectField("Select Item", selectedItem, typeof(ItemProfile));
    #pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.

            if (GUILayout.Button("Create Item"))
            {
                selectedItem = ItemProfile.CreateInstance();
                ItemProfileSet.AddElement(selectedItem);
            }
            if (GUILayout.Button("Remove Item") && selectedItem)
            {
                ItemProfileSet.RemoveElement(selectedItem);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            #endregion
            if (selectedItem)
            {
                EditorGUI.BeginChangeCheck();
                contentScrollPos = EditorGUILayout.BeginScrollView(contentScrollPos);
                #region Introduce Region
                var imageNameArea = EditorGUILayout.BeginHorizontal();
                var imageArea = EditorGUILayout.BeginVertical();
                selectedItem.itemIcon = (Sprite)EditorGUILayout.ObjectField(selectedItem.itemIcon, typeof(Sprite));
                GUIContent content;
                if (selectedItem.itemIcon == null)
                    content = new GUIContent("No Image");
                else
                    content = new GUIContent(selectedItem.itemIcon.texture);
                GUILayout.Box(content, spriteBoxOption);
                EditorGUILayout.EndVertical();
                var nameArea = EditorGUILayout.BeginVertical();
                CGUI.SLabelField("Name", GUILayout.Width(40));
                var prevName = selectedItem.itemName;
                selectedItem.itemName = EditorGUILayout.DelayedTextField(selectedItem.itemName, GUILayout.MinWidth(50));
                if (prevName != selectedItem.itemName)
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedItem), "Item_" + selectedItem.itemName);
               
                EditorGUILayout.BeginHorizontal();
                selectedItem.itemType = (ItemType)CGUI.VLabeledEnumField("Type", selectedItem.itemType, GUILayout.Width(50f));
                var descSize = 120;
                if (selectedItem.itemType == ItemType.장비)
                {
                    EditorGUILayout.LabelField("Equip Part");
                    selectedItem.equipPartID = EditorGUILayout.Popup(selectedItem.equipPartID, CoreDataSet.MetaEquipPartData.Names.ToArray());
                    descSize = 85;
                }
                selectedItem.foodCookDiv = (FoodCookDiv)CGUI.VLabeledEnumField("조리방법", selectedItem.foodCookDiv, GUILayout.Width(50f));
                selectedItem.foodBigDiv = (FoodBigDiv)CGUI.VLabeledEnumField("대분류", selectedItem.foodBigDiv, GUILayout.Width(50f));
                selectedItem.foodMiddleDiv = (FoodMiddleDiv)CGUI.VLabeledEnumField("중분류", selectedItem.foodMiddleDiv, GUILayout.Width(50f));
                EditorGUILayout.EndHorizontal();
                CGUI.SLabelField("FX Object");
                selectedItem.fxObject = EditorGUILayout.ObjectField(selectedItem.fxObject, typeof(GameObject)) as GameObject;
                CGUI.SLabelField("Description");
                selectedItem.description = EditorGUILayout.DelayedTextField(selectedItem.description, GUILayout.Height(descSize));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                #endregion

                #region Contents Region
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                //  Sound Description
                EditorGUILayout.BeginVertical();
                selectedItem.soundEffects = CGUI.VObjectField<ItemProfile.SoundEffects>(selectedItem.soundEffects);
                EditorGUILayout.EndVertical();
                //  End Sound Desc
                EditorGUILayout.BeginVertical();
                if (selectedItem.itemType == ItemType.조리법)
                    selectedItem.blueprint = (BlueprintProfile)EditorGUILayout.ObjectField("Blueprint", selectedItem.blueprint, typeof(BlueprintProfile));
                if (selectedItem.itemType == ItemType.장비)
                {
                    selectedItem.hp = CGUI.DynamicGaugeField("MAX HP", selectedItem.hp);
                    selectedItem.hp.CurrentGauge = selectedItem.hp.DefaultGauge;
                }
                else
                {
                    selectedItem.hp = CGUI.DynamicGaugeField("CUR HP", selectedItem.hp);
                    selectedItem.hp.DefaultGauge = selectedItem.hp.CurrentGauge;
                }
                EditorGUILayout.LabelField("Price");
                selectedItem.price = EditorGUILayout.DelayedFloatField(selectedItem.price);
                if (selectedItem.HavePrepareDuration)
                {
                    EditorGUILayout.LabelField("Prepare Duration");
                    selectedItem.prepareDuration = EditorGUILayout.DelayedFloatField(selectedItem.prepareDuration);
                }
                EditorGUILayout.LabelField("Cook Duration");
                selectedItem.cookDuration = EditorGUILayout.DelayedFloatField(selectedItem.cookDuration);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                selectedItem.influence = CGUI.StatField("Infulence", selectedItem.influence, statFieldOption);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                selectedItem.statusEffects = CGUI.VObjectDropdown<StatusEffectProfile>(
                    "Status Effects",
                    selectedItem.statusEffects,
                    ref flagStatusEffectFoldout
                );
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
                #endregion
                EditorGUILayout.EndScrollView();
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(selectedItem);
                    AssetDatabase.SaveAssets();
                }
            }
        }
    }
}
