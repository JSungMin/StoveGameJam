using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Game.Skill
{
    public class PlayerHighParry : SkillBase
    {
        private MainCollider usedCollider;

        public PlayerHighParry(GameActor a, SkillProfile p) : base(a, p)
        {
        }

        protected override IEnumerator ISkillBehave(SpellResult sResult)
        {
            yield return IAnimate(speller, skillProfile.animationDesc);
            usedCollider = speller.GetCollider("Weapon").LoadSkillDesc(this, BehaveToVictim);
            if (usedCollider == null) yield break;
            yield return IWaitReceiveSense("Hit_Start", OnHitStart);
            yield return IWaitReceiveSense("Hit_End", OnHitEnd);
        }
        private void OnHitStart()
        {
            usedCollider.Activate();
        }
        private void OnHitEnd()
        {
            usedCollider.DeActivate();
        }

        protected override IEnumerator ISkillFinish(SpellResult sResult)
        {
            yield return IWaitForAnimateEnd();
        }

        protected override void BehaveToVictim(Object sender, GameActor victim)
        {
            
        }
    }
}
