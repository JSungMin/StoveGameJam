using System.Collections;
using UnityEngine;

namespace CoreSystem.Game.Skill
{
    public class TankLowAttack : SkillBase
    {
        public TankLowAttack(GameActor a, SkillProfile p) : base(a, p)
        {

        }

        protected override IEnumerator ISkillBehave(SpellResult sResult)
        {
            yield return IAnimate(speller, skillProfile.animationDesc);
            //  Create/Get Skill Collider For Finding Out Victim
            var skillCollider = speller.GetCollider("Weapon")?.LoadSkillDesc(this, BehaveToVictim);
            if (null == skillCollider) yield break;

            //  Sensor Event Called By Animation Trigger Event
            void OnHitStart()
            {
                skillCollider.Activate();
            }
            yield return IWaitReceiveSense("Hit_Start", OnHitStart);
            //  Sensor Event Called By Animation Trigger Event
            void OnHitEnd()
            {
                skillCollider.DeActivate();
            }
            yield return IWaitReceiveSense("Hit_End", OnHitEnd);
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