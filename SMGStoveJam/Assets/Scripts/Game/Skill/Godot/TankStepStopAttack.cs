using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Game.Skill
{
    public class TankStepStopAttack : SkillBase
    {
        private MainCollider usedCollider;
        public TankStepStopAttack(GameActor a, SkillProfile p) : base(a, p)
        {
        }
        protected override IEnumerator ISkillBehave(SpellResult sResult)
        {
            yield return IAnimate(speller, skillProfile.animationDesc);
            //  Create/Get Skill Collider For Finding Out Victim
            usedCollider = speller.GetCollider("Weapon")?.LoadSkillDesc(this, BehaveToVictim);
            if (null == usedCollider) yield break;
            //  Sensor Event Called By Animation Trigger Event
            yield return IWaitReceiveSense("Hit_Start", OnHitStart);

            //  Sensor Event Called By Animation Trigger Event
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
            var log = "";
            var spellerName = speller.gameObject.name;
            log = "[" + spellerName + "] Used <" + SkillName + ">\n";
            log += "-->[" + victim.gameObject.name + "] Affected";
            Debug.Log(log);
            if (!victim.condition.isUnbeatable)
                BattleLogic.CreateChangeStateMsg("DAMAGE", victim);
            else
            {
                Debug.Log("Victim Is Unbeatable");
            }
        }
    }
}