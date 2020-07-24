using System;
using System.Collections;
using System.Collections.Generic;
using CoreSystem.Game.Skill;
using CoreSystem.ProfileComponents;
using UnityEngine;

namespace CoreSystem.Game.ActorComponents
{
    public abstract class ActorBrain : MonoBehaviour, ISetActorFSM
    {
        protected GameActor actor;
        public ActorFSM FSM => actor.FSM;
        public ActorBehaviourEntry behaviourEntry;
        public List<ActorBehaviour> behaviours = new List<ActorBehaviour>();
        public ActorMoveBehaviour moveBehaviour;
        protected SkillView SkillView => actor.SkillView;

        public ActorBehaviourEntry.AnimateEntry AnimateEntry => behaviourEntry.animateEntry;
        public ActorBehaviourEntry.MoveEntry MoveEntry => behaviourEntry.moveEntry;
        public ActorBehaviourEntry.SpellEntry SpellEntry => behaviourEntry.spellEntry;
        public ActorBehaviourEntry.DamageEntry DamageEntry => behaviourEntry.damageEntry;

        public Dictionary<string, Action> senseMap = new Dictionary<string, Action>();

        public virtual void LoadWithAwake(GameActor a)
        {
            actor = a;
            foreach (var b in behaviours)
            {
                b.LoadWithAwake(a);
            }

            if (null != FSM)
            {
                BuildStateMap(FSM);
                BuildStateEdges(FSM);
            }
        }

        public virtual void LoadWithStart(GameActor a)
        {
            actor = a;
            foreach (var b in behaviours)
            {
                b.LoadWithStart(a);
            }
        }
        public abstract void BuildStateMap(ActorFSM fsm);
        public abstract void BuildStateEdges(ActorFSM fsm);

        #region Exposed Utility Funcs

        public virtual void OnReceiveInput(string inputName, object value = null) {
        }
        public virtual SkillBase SpellSkill(SkillBase skill, bool useCondition)
        {
            SpellEntry.FillEntry(skill, useCondition);
            //  Index ID 5 Is Spell State
            BattleLogic.CreateChangeStateMsg("SPELL", actor, actor);
            return skill;
        }
        public virtual SkillBase SpellSkill(SkillProfile skill, bool useCondition)
        {
            var skillEntry = actor.SkillView.Find(skill);
            //  Check Is Learned
            var isLearned = null != skillEntry;
            if (!isLearned) return null;
            return SpellSkill(skillEntry, useCondition);
        }

        public virtual void Move(Vector3 direction, float moveSpeed, bool useJump = false)
        {
            BattleLogic.CreateChangeStateMsg(useJump ? "JUMP" : "MOVE", actor);
        }

        public virtual void Move()
        {
            BattleLogic.CreateChangeStateMsg(MoveEntry.prevVelocity.y > 0 ? "JUMP" : "MOVE", actor);
        }

        public virtual void Damage(SkillBase skillBase, float dmg)
        {

        }
        public virtual Vector3 LookAt(Vector3 dir)
        {
            var trans = transform;
            var absScaleX = 1f;
            if (null != AnimateEntry.controller)
            {
                trans = AnimateEntry.controller.transform;
                if (!AnimateEntry.IsFrameBase)
                {
                    absScaleX = Mathf.Abs(trans.localScale.x);
                    if (dir.x > 0)
                        AnimateEntry.GetSpineAnimator.ScaleX = absScaleX;
                    else
                        AnimateEntry.GetSpineAnimator.ScaleX = -absScaleX;
                    return dir;
                }
            }
            absScaleX = Mathf.Abs(trans.localScale.x);
            if (dir.x > 0)
            {
                ScaleX(trans, absScaleX);
            }
            else
            {
                ScaleX(trans, -absScaleX);
            }
            return dir;
        }
        public virtual Vector3 ScaleX(Transform trans, float x)
        {
            var newScale = trans.localScale;
            var scaleX = Mathf.Abs(newScale.x);
            newScale.x = Mathf.Sign(x) * scaleX;
            trans.localScale = newScale;
            return trans.localScale;
        }
        #endregion
        #region Sense Event Area
        public void OnSensorEvent(string sensorEvent)
        {
            if(senseMap.ContainsKey(sensorEvent)) senseMap[sensorEvent]?.Invoke();
        }

        public void RegisterSensorEvent(string senseEvent, Action reaction)
        {
            var hasCheck = senseMap.ContainsKey(senseEvent);
            if (!hasCheck)
            {
                senseMap.Add(senseEvent, reaction);
            }
            else
                senseMap[senseEvent] += reaction;
        }

        public void CancelSensorEvent(string senseEvent)
        {
            senseMap.Remove(senseEvent);
        }
        public void CancelSensorEvent(string senseEvent, Action reaction)
        {
            if (reaction != null) senseMap[senseEvent] -= reaction;
        }
        #endregion

        #region Inner Unity Event Callbacks
        protected virtual void Update()
        {
            behaviourEntry.deltaTime = Time.deltaTime;
            Imagine(behaviourEntry);
            Decide(behaviourEntry);
        }

        protected virtual void LateUpdate()
        {
            //  애니메이션, 변형과 같은 작업이 모두 끝난 후 초기화
            behaviourEntry.Reset();
        }
        #endregion
        #region Abstract Funcs
        protected abstract void Imagine(ActorBehaviourEntry entry);
        protected abstract void Decide(ActorBehaviourEntry entry);
        #endregion

    }
}
