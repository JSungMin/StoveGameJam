using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CoreSystem.Editor.ProfileViewer
{
    public partial class ActorEditor : EditorWindow
    {
        private static void OnStatusEffectView()
        {
            var scdRect = EditorGUILayout.BeginVertical();
            //  Status Effect Select, Create, Delete Area (SCD Area)
            #region SCD Area
            EditorGUILayout.BeginHorizontal();
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
            if (selectedStatus == null && StatusEffectProfileSet.CountOfElements() != 0)
            {
                selectedStatus = StatusEffectProfileSet.GetElement(0);
            }
            selectedStatus = (StatusEffectProfile)EditorGUILayout.ObjectField("Select Status Effect", selectedStatus, typeof(StatusEffectProfile));
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.

            if (GUILayout.Button("Create Effect"))
            {
                selectedStatus = StatusEffectProfile.CreateInstance();
                StatusEffectProfileSet.AddElement(selectedStatus);
            }
            if (GUILayout.Button("Remove Effect") && selectedStatus)
            {
                StatusEffectProfileSet.RemoveElement(selectedStatus);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            #endregion
            if (selectedStatus)
            {
                EditorGUI.BeginChangeCheck();
                #region Introduce Region
                var imageNameArea = EditorGUILayout.BeginHorizontal();
                var imageArea = EditorGUILayout.BeginVertical();
                selectedStatus.effectIcon = (Sprite)EditorGUILayout.ObjectField(selectedStatus.effectIcon, typeof(Sprite));
                GUIContent content;
                if (selectedStatus.effectIcon == null)
                    content = new GUIContent("No Image");
                else
                    content = new GUIContent(selectedStatus.effectIcon.texture);
                GUILayout.Box(content, spriteBoxOption);
                EditorGUILayout.EndVertical();
                var nameArea = EditorGUILayout.BeginVertical();
                CGUI.SLabelField("Name", GUILayout.Width(40));
                var prevName = selectedStatus.effectName;
                selectedStatus.effectName = EditorGUILayout.DelayedTextField(selectedStatus.effectName, GUILayout.MinWidth(50));
                if (prevName != selectedStatus.effectName)
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedStatus), "Status_" + selectedStatus.effectName);
                CGUI.SLabelField("Description");
                selectedStatus.description = EditorGUILayout.DelayedTextField(selectedStatus.description, GUILayout.Height(190));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                #endregion

                #region Content Region
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                selectedStatus.statusType = (StatusAffectMethod)CGUI.VLabeledEnumField("Type", selectedStatus.statusType);
                CGUI.SLabelField("Level");
                selectedStatus.level = EditorGUILayout.IntField(selectedStatus.level);
                CGUI.SLabelField("Duration");
                selectedStatus.duration = EditorGUILayout.FloatField(selectedStatus.duration);
                EditorGUILayout.EndVertical();
                selectedStatus.influenceCurve = EditorGUILayout.CurveField(selectedStatus.influenceCurve, infulenceCurveOption);
                EditorGUILayout.EndHorizontal();
                selectedStatus.influence = CGUI.StatField("Infulence", selectedStatus.influence, statFieldOption);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
                #endregion
                GUILayout.FlexibleSpace();
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(selectedStatus);
                    AssetDatabase.SaveAssets();
                }
            }
        }
    }
}
