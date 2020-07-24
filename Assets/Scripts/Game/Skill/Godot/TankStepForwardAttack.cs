using System.Collections;
using System.Collections.Generic;
using CoreSystem.Game.ActorComponents;
using CoreSystem.Utility.CurveMap;
using UnityEngine;

namespace CoreSystem.Game.Skill
{
    public class TankStepForwardAttack : SkillBase
    {
        private MainCollider usedCollider;
        private BehaviourJob moveForwardJob;
        private float dirX;
        private AnimationCurve speedCurve;

        public TankStepForwardAttack(GameActor a, SkillProfile p) : base(a, p)
        {
            speedCurve = CurveMap.GetCurveData("EXP_Increase");
        }
        protected override IEnumerator ISkillBehave(SpellResult sResult)
        {
            dirX = ((EnemyBrain) Brain).DirToPlayer.x;
            dirX = (dirX > 0) ? 1f : -1f;
            FlipX(speller, dirX > 0);
            yield return IAnimate(speller, skillProfile.animationDesc);
            //  Create/Get Skill Collider For Finding Out Victim
            if (null == usedCollider)
            {
                usedCollider = GetCollider("Weapon")?.LoadSkillDesc(this, BehaveToVictim);
                if(null == usedCollider) yield break;
            }
            //  Sensor Event Called By Animation Trigger Event
            yield return IWaitReceiveSense("Hit_Start", OnHitStart);
            
            //  Sensor Event Called By Animation Trigger Event
            yield return IWaitReceiveSense("Hit_End", OnHitEnd);
        }

        private void OnMoveGround(ActorBrain brain)
        {
            var t = brain.AnimateEntry.PlaybackRatio;
            var eval = (1f - speedCurve.Evaluate(t)) * 3f;
            brain.MoveEntry.SetSpeed(dirX, 15f*eval);
        }

        private void OnMoveFall(ActorBrain brain)
        {
            var t = brain.AnimateEntry.PlaybackRatio;
            var eval = (1f - speedCurve.Evaluate(t)) * 3f;
            brain.MoveEntry.SetSpeed(dirX, 15f * eval);
        }
        private void OnHitStart()
        {
            usedCollider.Activate();
            Brain.MoveEntry.SetSpeed(dirX,0f);
            moveForwardJob = ActorUtility.LoopMoveWithFall(speller.brain,OnMoveGround, OnMoveFall);
            moveForwardJob.Start();
        }
        
        private void OnHitEnd()
        {
            usedCollider.DeActivate();
        }
        protected override IEnumerator ISkillFinish(SpellResult sResult)
        {
            yield return IWaitForAnimateEnd();
            moveForwardJob?.Kill();
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