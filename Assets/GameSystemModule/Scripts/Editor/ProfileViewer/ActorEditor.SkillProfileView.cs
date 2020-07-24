using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CoreSystem.Editor.ProfileViewer
{
    public partial class ActorEditor : EditorWindow
    {
        private static void OnSkillView()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
            if (selectedSkill == null && SkillProfileSet.CountOfElements() != 0)
            {
                selectedSkill = SkillProfileSet.GetElement(0);
            }
            selectedSkill = (SkillProfile)EditorGUILayout.ObjectField("Select Skill", selectedSkill, typeof(SkillProfile));
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.

            if (GUILayout.Button("Create Skill"))
            {
                selectedSkill = SkillProfile.CreateInstance();
                SkillProfileSet.AddElement(selectedSkill);
            }
            if (GUILayout.Button("Remove Skill") && selectedSkill)
            {
                SkillProfileSet.RemoveElement(selectedSkill);
            }
            EditorGUILayout.EndHorizontal();
            if (selectedSkill)
            {
                EditorGUI.BeginChangeCheck();
                contentScrollPos = EditorGUILayout.BeginScrollView(contentScrollPos);
                #region Introduce Region
                var imageNameArea = EditorGUILayout.BeginHorizontal();
                //EditorGUI.DrawRect(imageNameArea, EditorResourceSet.GetColor("Scalet"));
                var imageArea = EditorGUILayout.BeginVertical();
                selectedSkill.skillIcon = (Sprite)EditorGUILayout.ObjectField(selectedSkill.skillIcon, typeof(Sprite));
                GUIContent content;
                if (selectedSkill.skillIcon == null)
                    content = new GUIContent("No Image");
                else
                    content = new GUIContent(selectedSkill.skillIcon.texture);
                GUILayout.Box(content, spriteBoxOption);
                EditorGUILayout.EndVertical();
                var nameArea = EditorGUILayout.BeginVertical();
                CGUI.SLabelField("Name", GUILayout.Width(40));
                var prevName = selectedSkill.skillName;
                selectedSkill.skillName = EditorGUILayout.DelayedTextField(selectedSkill.skillName, GUILayout.MinWidth(50));
                if (prevName != selectedSkill.skillName)
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedSkill), "Skill_" + selectedSkill.skillName);
                selectedSkill.skillType = (SkillType)CGUI.VLabeledEnumField("Type", selectedSkill.skillType);
                selectedSkill.skillCoverageType = (SkillCoverageType)CGUI.VLabeledEnumField("Coverage", selectedSkill.skillCoverageType);
                CGUI.SLabelField("Use FX");
                selectedSkill.useFX = EditorGUILayout.ObjectField(selectedSkill.useFX, typeof(GameObject)) as GameObject;
                CGUI.SLabelField("Success FX");
                selectedSkill.successFX = EditorGUILayout.ObjectField(selectedSkill.successFX, typeof(GameObject)) as GameObject;

                CGUI.SLabelField("Description");
                selectedSkill.description = EditorGUILayout.TextArea(selectedSkill.description, GUILayout.Height(85));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                #endregion
                #region Contents Region
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                //  Animation Description
                EditorGUILayout.BeginVertical();
                selectedSkill.animationDesc = CGUI.VObjectField(selectedSkill.animationDesc);
                EditorGUILayout.EndVertical();
                //  Sound Description
                EditorGUILayout.BeginVertical();
                selectedSkill.soundEffects = CGUI.VObjectField(selectedSkill.soundEffects);
                EditorGUILayout.EndVertical();  // End Sound Desc
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                selectedSkill.physDmg = EditorGUILayout.DelayedFloatField("Phys Damage", selectedSkill.physDmg);
                selectedSkill.magicDmg = EditorGUILayout.DelayedFloatField("Magic Damage", selectedSkill.magicDmg);
                selectedSkill.cheganDmg = EditorGUILayout.DelayedFloatField("Chegan Damage", selectedSkill.cheganDmg);
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                selectedSkill.hpCost = EditorGUILayout.DelayedFloatField("HP Cost", selectedSkill.hpCost);
                selectedSkill.mpCost = EditorGUILayout.DelayedFloatField("MP Cost", selectedSkill.mpCost);
                selectedSkill.coolTime = EditorGUILayout.DelayedFloatField("CoolTime",selectedSkill.coolTime);
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                selectedSkill.knockBack = EditorGUILayout.DelayedFloatField("KnockBack", selectedSkill.knockBack);
                selectedSkill.knockUp = EditorGUILayout.DelayedFloatField("KnockUp", selectedSkill.knockUp);
                selectedSkill.knockTime = EditorGUILayout.DelayedFloatField("KnockTime", selectedSkill.knockTime);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                selectedSkill.statusEffects = CGUI.VObjectDropdown(
                    "Status Effects",
                    selectedSkill.statusEffects,
                    ref flagStatusEffectFoldout
                );
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                #endregion
                EditorGUILayout.EndScrollView();
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(selectedSkill);
                    AssetDatabase.SaveAssets();
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }
    }
}
