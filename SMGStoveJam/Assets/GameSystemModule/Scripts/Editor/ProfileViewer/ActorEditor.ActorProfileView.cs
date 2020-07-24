using System.Collections;
using System.Collections.Generic;
using CoreSystem.ProfileComponents;
using Spine.Unity;
using UnityEngine;
using UnityEditor;

namespace CoreSystem.Editor.ProfileViewer
{
    public partial class ActorEditor : EditorWindow
    {
        public GUILayoutOption[] startBttnOptions;

        private void InitializeActorView()
        {
            startBttnOptions = new GUILayoutOption[]
            {
                GUILayout.Height(25)
            };
        }

        private static void DrawPortraitSet(ActorProfile.PortraitDescSet set)
        {
            var prevCount = set.Descriptions.Count;
            CGUI.VObjectDropdown("Portraits", set.Descriptions, ref flagShowPortraitList, DrawPortraitDesc);
            if (set.Descriptions.Count != prevCount)
            {
                EditorUtility.SetDirty(selectedActor);
            }
        }

        private static void DrawPortraitDesc(int idx, ActorProfile.PortraitDesc desc)
        {
            if (desc.flagFoldoutDesc)
            {
                EditorGUI.BeginChangeCheck();
                var label = desc.name;
                if (label == "")
                    label = "NoName";
                desc.name = EditorGUILayout.DelayedTextField(label);
                desc.portraitType = (ActorProfile.PortraitType)CGUI.VLabeledEnumField("Type", desc.portraitType);
                if (desc.portraitType == ActorProfile.PortraitType.Animation)
                {
                    DrawPortraitAnimation(desc.animation);
                }
                else if (desc.portraitType == ActorProfile.PortraitType.Sprite)
                {
                    var spriteBoxOption = new GUILayoutOption[]
                    {
                        GUILayout.Width(225),
                        GUILayout.Height(225)
                    };
                    desc.sprite = (Sprite)EditorGUILayout.ObjectField(desc.sprite, typeof(Sprite));
                    GUIContent content;
                    if (desc.sprite == null)
                        content = new GUIContent("No Image");
                    else
                        content = new GUIContent(desc.sprite.texture);
                    GUILayout.Box(content, spriteBoxOption);
                }
                else
                {
                    desc.sprite = (Sprite)EditorGUILayout.ObjectField(desc.sprite, typeof(Sprite));
                    CGUI.SLabelField("Animation Name");
                    desc.animation.name = EditorGUILayout.DelayedTextField(desc.animation.name);
                }
                if (EditorGUI.EndChangeCheck())
                    EditorUtility.SetDirty(selectedActor);
            }
        }

        private static void DrawPortraitAnimation(AnimationDesc desc)
        {
            EditorGUILayout.BeginHorizontal();
            desc.animationType =  (AnimationType)EditorGUILayout.EnumPopup(desc.animationType);
            desc.name = EditorGUILayout.DelayedTextField(desc.name);
            desc.speedMultiplier = EditorGUILayout.DelayedFloatField(desc.speedMultiplier);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (desc.animationType == AnimationType.Frame)
            {
                desc.frameAnimator = (RuntimeAnimatorController)EditorGUILayout.ObjectField("FrameAnimator",desc.frameAnimator, typeof(RuntimeAnimatorController));
            }
            else if (desc.animationType == AnimationType.Spine)
            {
                desc.spineDataAsset = (SkeletonDataAsset)EditorGUILayout.ObjectField("SpineDataAsset", desc.spineDataAsset, typeof(SkeletonDataAsset));
            }
            EditorGUILayout.EndHorizontal();
        }
        private static void OnActorView()
        {
            var scdRect = EditorGUILayout.BeginVertical();
            //  Profile Select, Create, Delete Area (SCD Area)
            #region SCD Area
            EditorGUILayout.BeginHorizontal();
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
            if (selectedActor == null && ActorProfileSet.CountOfElements() != 0)
            {
                selectedActor = ActorProfileSet.GetElement(0);
            }
            selectedActor = (ActorProfile)EditorGUILayout.ObjectField("Select Profile", selectedActor, typeof(ActorProfile));
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.

            if (GUILayout.Button("Create Actor"))
            {
                selectedActor = ActorProfile.CreateInstance();
                ActorProfileSet.AddElement(selectedActor);
            }
            if (GUILayout.Button("Remove Actor") && selectedActor)
            {
                ActorProfileSet.RemoveElement(selectedActor);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            #endregion

            #region Content Area
            if (null != selectedActor)
            {
                //EditorGUI.BeginChangeCheck();
                contentScrollPos = EditorGUILayout.BeginScrollView(contentScrollPos);
                #region Introduce Area
                EditorGUILayout.BeginHorizontal();
                vSpriteScrollPos = EditorGUILayout.BeginScrollView(vSpriteScrollPos, spriteScrollViewOption);
                var scrollRect = EditorGUILayout.BeginVertical();
                
                EditorGUI.BeginChangeCheck();
                DrawPortraitSet(selectedActor.portraitSet);
                if(EditorGUI.EndChangeCheck())
                {
                    selectedActor.portraitSet.UpdateAllPortraitType();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
                var nameHMPRect = EditorGUILayout.BeginVertical(GUILayout.MaxHeight(250 + EditorGUIUtility.singleLineHeight));
                //  Name and Name Color Rect
                EditorGUILayout.BeginHorizontal();
                var nameRect = EditorGUILayout.BeginVertical();
                CGUI.SLabelField("Name", GUILayout.Width(40));
                var prevName = selectedActor.actorName;
                selectedActor.actorName = EditorGUILayout.DelayedTextField(selectedActor.actorName, GUILayout.MinWidth(150));
                if (prevName != selectedActor.actorName)
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedActor), "Profile_" + selectedActor.actorName);
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                CGUI.SLabelField("Color");
                selectedActor.actorNameColor = EditorGUILayout.ColorField(selectedActor.actorNameColor, GUILayout.Width(50));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                var hmpRect = EditorGUILayout.BeginVertical(GUILayout.MinWidth(50));
                //EditorGUI.DrawRect(hmpRect, EditorResourceSet.GetColor("DarkerGrass"));
                selectedActor.HP = CGUI.DynamicGaugeField("HP", selectedActor.HP, statFieldOption);
                selectedActor.MP = CGUI.DynamicGaugeField("MP", selectedActor.MP, statFieldOption);
                selectedActor.Chegan = CGUI.DynamicGaugeField("Chegan", selectedActor.Chegan, statFieldOption);
                
                CGUI.SLabelField("Description");
                selectedActor.description = EditorGUILayout.TextArea(selectedActor.description, GUILayout.Height(120));
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();  // Name, HMP
                EditorGUILayout.EndHorizontal();
                #endregion
                
                EditorGUILayout.BeginHorizontal();
                #region Start Setting
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Start Setting");
                #region Equipment View
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Start Equipments");
                if (GUILayout.Button("Equipments", window.startBttnOptions))
                {
                    EquipmentViewer.ShowWindow(selectedActor);
                }
                EditorGUILayout.EndVertical();
                #endregion
                #region Inventory View
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Start Inventory");
                if (GUILayout.Button("Inventory", window.startBttnOptions))
                {
                    InventoryViewer.ShowWindow(selectedActor);
                }
                EditorGUILayout.EndVertical();
                #endregion
                #region Skill View
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Start Skills");
                if (GUILayout.Button("Skills", window.startBttnOptions))
                {
                    SkillViewer.ShowWindow(selectedActor);
                }
                EditorGUILayout.EndVertical();
                //GUILayout.FlexibleSpace();
                #endregion
                EditorGUILayout.EndVertical();
                #endregion

                #region Stat Area
                EditorGUILayout.BeginVertical(statFieldAreaOption);
                EditorGUI.BeginChangeCheck();
                selectedActor.basicStat = CGUI.StatField("Basic", selectedActor.basicStat, statFieldOption);
                CGUI.StatFieldUnActive("Additional", selectedActor.additionalStat, statFieldOption);
                if(EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(selectedActor);
                    AssetDatabase.SaveAssets();
                }
                EditorGUILayout.EndVertical();
                #endregion
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
                //if (EditorGUI.EndChangeCheck())
                //{
                //    EditorUtility.SetEntryToDirty(selectedActor);
                //    AssetDatabase.SaveAssets();
                //}
            }
            #endregion
        }
    }
}
