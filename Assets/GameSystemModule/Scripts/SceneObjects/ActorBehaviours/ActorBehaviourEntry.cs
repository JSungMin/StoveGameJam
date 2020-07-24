using System;
using System.Collections.Generic;
using CoreSystem.Game.ActorComponents;
using CoreSystem.Game.Skill;
using UnityEngine;

namespace CoreSystem.Game
{
    [System.Serializable]
    public class ActorBehaviourEntry
    {
        public uint dirtyMask = 0;

        public int maxDirtyIndex = -1;
        
        //  Common Used Parameter
        public float deltaTime;

        //  Move Event Parameters
        [System.Serializable]
        public class MoveEntry
        {
            public bool isDirty = false;
            public Vector3 direction = Vector3.zero;
            //  디렉션이 0이었을때를 뺀 모든 값을 그대로 저장한다.
            public Vector3 dirtyDirection;
            public bool curLook = true;
            public bool prevLook = true;
            public Vector3 prevVelocity;
            public Vector3 addVelocity;
            public bool isGrounded = false;

            public void FillEntry(bool grounded, Vector3 vel)
            {
                if (isDirty)
                    return;
                isDirty = true;
                isGrounded = grounded;
                prevVelocity = vel;
                addVelocity = Vector3.zero;
            }

            public void AddSpeed(float dirX, float speed)
            {
                direction.x = dirX;
                if (dirX != 0f)
                    dirtyDirection = direction;
                addVelocity.x += speed;
            }
            public void AddJump(float jump)
            {
                direction.y = 1f;
                dirtyDirection = direction;
                addVelocity.y += jump;
            }

            public void SetDirtyDir(Vector3 dir)
            {
                dirtyDirection = dir;
            }
            public void SetSpeed(float dirX, float speed)
            {
                direction.x = dirX;
                if (dirX != 0f)
                    dirtyDirection = direction;
                prevVelocity.x = direction.x * speed;
                addVelocity.x = 0f;
            }

            public void SetJump(float jump)
            {
                direction.y = 1f;
                dirtyDirection = direction;
                addVelocity.y = jump;
            }
            public void AddVelocity(Vector3 vel)
            {
                addVelocity += vel;
            }

            public void SetVelocity(Vector3 vel)
            {
                prevVelocity = vel;
                addVelocity = Vector3.zero;
            }
            public void EraseEntry()
            {
                isDirty = false;
            }
        }
        //  Spell Event Parameters
        [System.Serializable]
        public class SpellEntry
        {
            public bool isDirty = false;
            public SkillBase skillBase;
            public bool useCondition = true;
            public bool isColliderActivated = false;

            public List<GameActor> victim = new List<GameActor>();
            public List<MainCollider> skillColliders = new List<MainCollider>();


            public void FillEntry(SkillBase skill, bool condition)
            {
                skillBase = skill;
                isColliderActivated = false;
                useCondition = condition;
                isDirty = true;
            }

            public void EraseEntry()
            {
                isDirty = false;
                skillBase = null;
                victim.Clear();
                skillColliders.Clear();
            }
        }
        //  Damaged Event Parameters
        [System.Serializable]
        public class DamageEntry
        {
            public SkillBase EnemySkill { get; private set; }
            public Vector3 dirToSpeller;
            public Timer knockTimer;

            public void FillEntry(SkillBase eSkill, Vector3 dir2S)
            {
                EnemySkill = eSkill;
                dirToSpeller = dir2S;
                knockTimer.ResetTimer(TimerState.FINISH);
                knockTimer = new Timer(this, "KnockTimer", eSkill.skillProfile.knockTime);
                knockTimer.StartTimer();
            }

            public void UpdateKnockTimer(Action<DamageEntry> onUpdate = null, Action<DamageEntry> onFinish = null, Action onPause = null)
            {
                knockTimer.UpdateTimer(Time.deltaTime, onUpdate, onFinish, onPause);
            }
        }

        //  Animate Event Parameters
        [System.Serializable]
        public class AnimateEntry
        {
            public bool isDirty = false;
            public int trackIndex;
            public string animName;
            public bool loop = false;
            public float speed = 1.0f;
            public ActorBehaviour controller;

            public bool IsFrameBase=>controller.GetType() == typeof(FrameAnimatorController);
            

            public bool IsAnimateEnd(int layer)
            {
                if(IsFrameBase)
                    return !GetFrameAnimator.isAnimate[layer];
                else
                    return GetSpineAnimator.IsComplete;
            }

            public float PlaybackRatio => IsFrameBase ? GetFrameAnimator.GetPlaybackRatio(trackIndex) : GetSpineAnimator.GetPlaybackRatio(trackIndex);
            

            public Action<string> onFrameAnimateStart;
            public Action<string> onFrameAnimateComplete;
            public Action<Spine.TrackEntry> onSpineAnimateComplete;
            public Action<Spine.TrackEntry, Spine.Event> onSpineAnimateEvent;

            public void FillEntry(int track=0, string anim="", bool useLoop=false, float speedMul=1f)
            {
                isDirty = true;
                trackIndex = track;
                animName = anim;
                loop = useLoop;
                speed = speedMul;
            }

            public void EraseEntry()
            {
                isDirty = false;
            }
            public FrameAnimatorController GetFrameAnimator => (FrameAnimatorController) controller;
            public SpineController GetSpineAnimator => (SpineController) controller;
        }
        public MoveEntry moveEntry;
        public SpellEntry spellEntry;
        public DamageEntry damageEntry;
        public AnimateEntry animateEntry;
        
        public bool IsDirty(int idx)
        {
            return ((dirtyMask & (1 << idx)) != 0);
        }
        public void SetDirty(int idx)
        {
            maxDirtyIndex = Mathf.Max(idx + 1, maxDirtyIndex);
            var flagValue = (uint)(1 << idx);
            dirtyMask = (dirtyMask | flagValue);
        }
        public void Reset()
        {
            dirtyMask = 0;
            maxDirtyIndex = -1;
            moveEntry.EraseEntry();
            animateEntry.EraseEntry();
        }
    }
}
