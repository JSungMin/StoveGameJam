using System;
using System.Collections;
using System.Collections.Generic;
using CoreSystem.Game.Skill;
using UnityEngine;

namespace CoreSystem.Game.ActorComponents
{
    public class ParryReactor : MonoBehaviour
    {
        public GameActor actor;

        public enum SituationType
        {
            ATTACK_PARRY,
            INTERACT_PARRY
        }

        public interface ISituation
        {
            bool Test(SkillBase ps, SkillBase es);
        }
        [System.Serializable]
        public class AttackParrySituation : ISituation
        {
            public SkillCoverageType targetCoverage;
            public SkillProfile enemySkillProfile;

            public bool Test(SkillBase ps, SkillBase es)
            {
                var compCoverage = targetCoverage == ps.SkillCoverageType;
                var compEnemySkill = enemySkillProfile == es.skillProfile;
                return compCoverage && compEnemySkill;
            }
        }

        [System.Serializable]
        public class InteractParrySituation : ISituation
        {
            public SkillCoverageType targetCoverage;

            public bool Test(SkillBase ps, SkillBase es)
            {
                return false;
            }
        }
        public List<AttackParrySituation> attackParrySituations = new List<AttackParrySituation>();
        public List<InteractParrySituation> interactParrySituations = new List<InteractParrySituation>();

        public Action<SkillBase, SkillBase> onSuccessAction;
        public Action<SkillBase, SkillBase> onFailAction;

        public bool TestWithBehave(SkillBase ps, SkillBase es)
        {
            var result = Test(ps, es);
            if (result)
                BehaveSuccess(ps, es);
            else
                BehaveFail(ps, es);
            return result;
        }
        public bool Test(SkillBase ps, SkillBase es)
        {
            var result = false;
            for (var i = 0; i < attackParrySituations.Count; i++)
            {
                var situation = attackParrySituations[i];
                var pResult = situation.Test(ps, es);
                if (pResult)
                    result = pResult;
            }
            for (var i = 0; i < interactParrySituations.Count; i++)
            {
                var situation = interactParrySituations[i];
                var pResult = situation.Test(ps, es);
                if (pResult)
                    result = pResult;
            }
            return result;
        }

        public void BehaveSuccess(SkillBase ps, SkillBase es)
        {
            onSuccessAction?.Invoke(ps, es);
        }

        public void BehaveFail(SkillBase ps, SkillBase es)
        {
            onFailAction?.Invoke(ps, es);
        }
    }
}
