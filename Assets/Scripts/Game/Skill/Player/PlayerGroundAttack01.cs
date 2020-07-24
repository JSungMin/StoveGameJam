using System.Collections;
using UnityEngine;

namespace CoreSystem.Game.Skill
{
    public class PlayerGroundAttack01 : SkillBase
    {
        private MainCollider usedCollider;
        public PlayerGroundAttack01(GameActor a, SkillProfile p) : base(a, p) { }

        protected override IEnumerator ISkillBehave(SpellResult sResult)
        {
            var dirX = speller.brain.MoveEntry.dirtyDirection.x;
            ScaleX(speller, dirX);
            //  Set Animate Entry
            yield return IAnimate(speller, skillProfile.animationDesc);
            //  Create/Get Skill Collider For Finding Out Victim
            usedCollider = speller.GetCollider("Weapon").LoadSkillDesc(this, BehaveToVictim);
            if (usedCollider == null) yield break;
            //  Sensor Event Called By Animation Trigger Event
            yield return IWaitReceiveSense("Hit_Start", OnHitStart);
            //  Sensor Event Called By Animation Trigger Event
            yield return IWaitReceiveSense("Hit_End", OnHitEnd);
        }

        private void OnHitStart()
        {
            usedCollider.Activate();
        }

        public void OnHitEnd()
        {
            usedCollider.DeActivate();
        }

        protected override IEnumerator ISkillFinish(SpellResult sResult)
        {
            yield return IWaitForAnimateEnd();
        }

        protected override void BehaveToVictim(UnityEngine.Object sender, GameActor victim)
        {
            //  TODO : Damage To Victim
            var log = "";
            var spellerName = speller.gameObject.name;
            log = "[" + spellerName + "] Used <" + SkillName + ">\n";
            log += "-->[" + victim.gameObject.name + "] Affected";
            Debug.Log(log);

            //  Calculate Damage
            var dmg = 0f;
            switch (skillProfile.skillType)
            {
                case SkillType.MagicalAttack:
                    dmg = skillProfile.magicDmg + skillProfile.physDmg * 0.25f;
                    break;
                case SkillType.PhysicalAttack:
                    dmg = skillProfile.physDmg + skillProfile.magicDmg * 0.25f;
                    break;
            }
            victim.Damage(this, dmg);
        }
    }
}
