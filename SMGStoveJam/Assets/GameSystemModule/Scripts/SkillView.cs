using System;
using System.Collections.Generic;
using CoreSystem.Game;
using CoreSystem.Game.Skill;

namespace CoreSystem.ProfileComponents
{
    [Serializable]
    public class SkillView : ElementsView<SkillProfile>
    {
        //  Initialized When In Game
        public List<SkillBase> skillEntries = new List<SkillBase>();

        public override void Initialize(GameActor actor)
        {
            base.Initialize(actor);
            foreach (var skillProfile in Elements)
            {
                var entry = SkillFactory.MakeSkill(actor, skillProfile);
                skillEntries.Add(entry);
            }
        }

        public void LearnSkill(SkillProfile profile)
        {
            var cache = Find(x=> x == profile);
            if (null != cache)
                return;
            AddElement(profile);
            var entry = SkillFactory.MakeSkill(viewOwner, profile);
            skillEntries.Add(entry);
        }

        public SkillBase Find(Predicate<SkillBase> match)
        {
            return skillEntries.Find(match);
        }

        public SkillBase Find(SkillProfile profile)
        {
            return skillEntries.Find(x => x?.skillProfile == profile);
        }

        public SkillBase Find(string skillName)
        {
            return skillEntries.Find(x => x?.SkillName == skillName);
        }
        public bool IsLearned(SkillProfile profile)
        {
            var cache = Find(x => x == profile);
            return cache != null;
        }

        public bool CoolDown(float delta, SkillBase skill)
        {
            return skill.CoolDown(delta);
        }
    }
}