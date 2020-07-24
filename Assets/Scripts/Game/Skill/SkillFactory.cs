using System;
using System.Collections.Generic;
using UnityEngine;
using CoreSystem.Game.Skill;

namespace CoreSystem.Game
{
    public static class SkillFactory
    {
        private static readonly Dictionary<string, Type> gSkillTypeMap= new Dictionary<string, Type>()
        {
            { "Player_Ground_Attack_01", typeof(PlayerGroundAttack01) },
            { "Player_Ground_Attack_02", typeof(PlayerGroundAttack02) },
            { "Player_High_Parry", typeof(PlayerHighParry)},
            { "Player_Middle_Parry", typeof(PlayerMiddleParry)},
            { "Player_Low_Parry", typeof(PlayerLowParry)},
            { "Tank_High_Attack_01", typeof(TankHighAttack01)},
            { "Tank_Laser",typeof(TankLaser)},
            { "Tank_Step_Stop_Attack",typeof(TankStepStopAttack)},
            { "Tank_Step_Forward_Attack",typeof(TankStepForwardAttack)},
            { "Tank_Low_Attack",typeof(TankLowAttack)},
            { "Tank_Jump_Attack",typeof(TankJumpAttack)}
        };

        
        public static SkillBase MakeSkill(GameActor speller, SkillProfile skillProfile)
        {
            if (null == skillProfile) return null;
            var skillName = skillProfile.skillName;
            var cachedEntry = GetSkillEntry(skillName, speller);
            if (null != cachedEntry)
            {
                return cachedEntry;
            }
            var skillType = gSkillTypeMap[skillName];
            var skillEntry = (SkillBase)TypeHelper.CreateInstance(skillType, speller, skillProfile);
            
            return skillEntry;
        }

        public static SkillBase GetSkillEntry(string skillName, GameActor actor)
        {
            var skill = actor.SkillView.Find(skillName);
            return skill;
        }
    }
}
