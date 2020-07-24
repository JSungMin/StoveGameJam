using System.Collections.Generic;
using CoreSystem.Game.Skill;
using CoreSystem.ProfileComponents;
using UnityEngine;

namespace CoreSystem
{
    public class SkillProfile : ScriptableObject
    {
        [System.Serializable]
        public class SoundEffects
        {
            public SoundProfile trySound;
            public SoundProfile successSound;
            public SoundProfile failSound;
        }
        public string skillName;
        public Sprite skillIcon;
        public string description;
        public SkillType skillType;
        public SkillCoverageType skillCoverageType;

        public AnimationDesc animationDesc;
        public SoundEffects soundEffects;
        public GameObject useFX;
        public GameObject successFX;

        public bool isGroundSkill = true;
        public bool isGlobalSkill = false;

        public float physDmg;
        public float magicDmg;
        public float cheganDmg;
        public float hpCost;
        public float mpCost;
        public float coolTime;

        public float knockBack;
        public float knockUp;
        public float knockTime;

        public List<StatusEffectProfile> statusEffects = new List<StatusEffectProfile>();

    #if UNITY_EDITOR
        public static SkillProfile CreateInstance()
        {
            var skillData = CreateInstance<SkillProfile>();
            var pathOfSkills = SystemPathes.GetSystemPath(SystemPathes.PathType.SkillProfileSet);
            var parentPath = PathHelper.GetParentPath(pathOfSkills);
            var skillCount = SkillProfileSet.CountOfElements();
            UnityEditor.AssetDatabase.CreateAsset(
                skillData,
                PathHelper.Bind(parentPath, "NewSkill_" + skillCount + ".asset")
            );
            UnityEditor.EditorUtility.SetDirty(skillData);
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.SaveAssets();
            return skillData;
        }
    #endif
    }
}