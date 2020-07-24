using System;
using System.ComponentModel;
using System.Linq;
using CoreSystem.Game;
using CoreSystem.Game.Skill;
using UnityEngine;

namespace CoreSystem.ProfileComponents
{
    [CreateAssetMenu(menuName = "GameData/Skill/ChainProfile")]
    [Description("연속되는 스킬 체인의 생성에 필요한 정보를 미리 담는 프로필 객체다")]
    public class SkillChainProfile : ScriptableObject
    {
        public int priority = 0;
        public string chainName;
        public SkillProfile[] profiles;
        public bool useLocalTimer;
        public float coolDuration = 0f;

        public SkillChain CreateSkillChain(GameActor owner, Action<SkillChain> onFinishCool = null)
        {
            var skills = new SkillBase[profiles.Length];
            for (var i = 0; i < profiles.Length; i++)
            {
                var sName = profiles[i].skillName;
                var entry = SkillFactory.GetSkillEntry(sName, owner);
                skills[i] = entry;
            }
            var result = SkillChain.Create(priority, chainName, owner, onFinishCool, skills);
            if (!useLocalTimer) return result;

            if (coolDuration < 0f)
            {
                var sumDuration = profiles.Sum(x => x.coolTime);
                result.SetLocalTimer(sumDuration);
            }
            else
                result.SetLocalTimer(coolDuration);
            return result;
        }
    }
}
