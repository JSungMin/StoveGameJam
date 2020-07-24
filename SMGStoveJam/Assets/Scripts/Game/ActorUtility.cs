using System;
using System.Collections;
using CoreSystem.Game.ActorComponents;
using CoreSystem.Game.Skill;
using Spine.Unity;
using UnityEngine;

namespace CoreSystem.Game
{
    public static class ActorUtility
    {
        #region Static Functions Region
        public static BehaviourJob SpellProcess(ActorBrain brain, SkillBase skill,
            Action<SpellResult> onSpell = null, Action<SpellResult> onFail = null,
            Action<ActorBehaviourEntry.AnimateEntry> onAnimateEnd = null)
        {
            return BehaviourJob.Make(ISpellProcess(brain, skill, onSpell, onFail, onAnimateEnd), false);
        }
        public static BehaviourJob PlayAnimation(ActorBrain brain, string animName, int track, float speed,
            bool useLoop, FrameAnimatorController controller, Action<Animator> animatorAction = null)
        {
            return BehaviourJob.Make(IPlayAnimation(brain, animName, track, speed, useLoop, controller, animatorAction),false);
        }
        public static BehaviourJob PlayAnimation(ActorBrain brain, string animName, int track, float speed,
            bool useLoop, SpineController controller, Action<SkeletonAnimation> animatorAction = null)
        {
            return BehaviourJob.Make(IPlayAnimation(brain, animName, track, speed, useLoop, controller, animatorAction),false);
        }
        public static BehaviourJob LoopMoveWithFall(ActorBrain brain, Action<ActorBrain> onGround = null,
            Action<ActorBrain> onFall = null, Action<ActorBrain> onLanding = null)
        {
            Debug.Log("Get MOVE JOB : " + brain.FSM.Current.name);
            return brain.moveBehaviour
                .SetCallback(onGround, onFall, onLanding)
                .GetJob(false);
        }

        public static BehaviourJob LoopFall(ActorBrain brain,
            Action<ActorBrain> onGround = null, Action<ActorBrain> onFall = null, Action<ActorBrain> onLanding = null)
        {
            return brain.moveBehaviour
                .SetCallback(onGround, onFall, onLanding)
                .GetJob(false);
            //brain.moveBehaviour?.SetCallback(onGround, onFall, onLanding);
            //return BehaviourJob.Make(ILoopFall(brain, brain.moveBehaviour), false);
        }

        public static BehaviourJob WaitForEndOfAnimate(ActorBrain brain)
        {
            return BehaviourJob.Make(IWaitForEndOfAnimate(brain), false);
        }

        public static BehaviourJob Action(ActorBrain brain, Action<ActorBrain> action)
        {
            return BehaviourJob.Make(IAction(brain, action), false);
        }
        public static BehaviourJob LoopAction(ActorBrain brain, Action<ActorBrain> action)
        {
            return BehaviourJob.Make(ILoopAction(brain, action), false);
        }
        #endregion

        #region Common Behaviour IEnumerator Region

        private static IEnumerator IAction(ActorBrain brain, Action<ActorBrain> action)
        {
            action?.Invoke(brain);
            yield return null;
        }
        private static IEnumerator ILoopAction(ActorBrain brain, Action<ActorBrain> action)
        {
            while (true)
            {
                action?.Invoke(brain);
                yield return null;
            }
        }
        #endregion
        private static IEnumerator ISpellProcess(ActorBrain brain, SkillBase skill,
            Action<SpellResult> onSpell,
            Action<SpellResult> onFail, Action<ActorBehaviourEntry.AnimateEntry> onAnimateEnd)
        {
            if (null != skill)
            {
                var spellEntry = brain.SpellEntry;
                var animateEntry = brain.AnimateEntry;
                var spellResult = skill.Spell(spellEntry.useCondition);
                if (spellResult.resultID != 1)
                    onFail?.Invoke(spellResult);
                else
                    onSpell?.Invoke(spellResult);
                yield return IWaitForEndOfAnimate(brain);
                onAnimateEnd?.Invoke(animateEntry);
            }
            yield return null;
        }
        #region Animation Behaviour IEnumerator Region
        private static IEnumerator IPlayAnimation(ActorBrain brain, string animName, int track, float speed,
            bool useLoop, FrameAnimatorController controller, Action<Animator> animatorAction)
        {
            brain.AnimateEntry.FillEntry(track, animName, useLoop, speed);
            animatorAction?.Invoke(controller.animator);
            yield return controller.BehaveAsCoroutine();
        }

        private static IEnumerator IPlayAnimation(ActorBrain brain, string animName, int track, float speed,
            bool useLoop, SpineController controller, Action<SkeletonAnimation> animatorAction)
        {
            brain.AnimateEntry.FillEntry(track,animName,useLoop,speed);
            animatorAction?.Invoke(controller.animator);
            yield return controller.BehaveAsCoroutine();
        }

        public static IEnumerator IWaitForEndOfAnimate(ActorBrain brain)
        {
            var animateEntry = brain.behaviourEntry.animateEntry;
            var isFrame = animateEntry.IsFrameBase;
            if (isFrame)
            {
                var derivedController = animateEntry.GetFrameAnimator;
                var layer = animateEntry.trackIndex;
                while (derivedController.isAnimate[layer])
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                var derivedController = animateEntry.GetSpineAnimator;
                var layer = animateEntry.trackIndex;
                while (!derivedController.IsComplete)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            yield return null;
        }
        #endregion
        #region Physics Behaviour IEnumerator Region
        
        private static IEnumerator ILoopFall(ActorBrain brain, ActorMoveBehaviour behaviour)
        {
            while (true)
            {
                brain.MoveEntry.addVelocity.x = 0f;
                yield return behaviour.BehaveAsCoroutine();
            }
        }
        #endregion
        #region Skill Behaviour IEnumerator Region

        private static IEnumerator ISkillAffect()
        {

            yield return null;
        }
        #endregion
    }
}
