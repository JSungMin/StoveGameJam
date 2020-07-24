using System;
using System.Collections;
using System.Collections.Generic;
using CoreSystem.Game.ActorComponents;
using CoreSystem.Utility.CurveMap;
using UnityEngine;
using Object = UnityEngine.Object;


namespace CoreSystem.Game.Skill
{
    public class PlayerRoll : SkillBase
    {
        public BehaviourJob moveJob;
        public float speed = 15f;
        public AnimationCurve speedCurve;
        public float dirX;

        public PlayerRoll(GameActor a, SkillProfile p) : base(a, p)
        {
            speedCurve = CurveMap.GetCurveData("EXP_Increase");
        }

        protected override IEnumerator ISkillBehave(SpellResult sResult)
        {
            //  TODO : 구르기 전진 필요
            yield return IAnimate(speller, skillProfile.animationDesc);
            var brain = speller.brain;
            if (brain.FSM.Current.name == "DAMAGE")
            {
                Stop();
                yield break;
            }
            //  Set Animate Entry
            dirX = ((PlayerBrain)brain).moveKeyValue.x;
            if (Math.Abs(dirX) <= 0)
            {
                dirX = brain.MoveEntry.dirtyDirection.x;
            }
            brain.MoveEntry.SetSpeed(0f,0f);
            ScaleX(speller, dirX);
            moveJob = ActorUtility.LoopMoveWithFall(brain, OnGround, OnFall);
            moveJob.Start();
        }

        private void OnGround(ActorBrain obj)
        {
            var t = obj.AnimateEntry.PlaybackRatio;
            var eval = speedCurve.Evaluate(t) * 0.8f;
            obj.MoveEntry.SetSpeed(dirX, speed * (1f-eval));
        }

        private void OnFall(ActorBrain obj)
        {
            var t = obj.AnimateEntry.PlaybackRatio;
            var eval = speedCurve.Evaluate(t) * 0.8f;
            obj.MoveEntry.SetSpeed(dirX, speed * (1f - eval));
        }

        protected override IEnumerator ISkillFinish(SpellResult sResult)
        {
            yield return IWaitForAnimateEnd();
            
        }

        protected override void BehaveToVictim(Object sender, GameActor victim)
        {
        }

        protected override void OnStopSkillProcess()
        {
            
        }
    }
}
