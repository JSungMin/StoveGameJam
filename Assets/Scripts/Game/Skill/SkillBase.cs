using System;
using System.Collections;
using System.Collections.Generic;
using CoreSystem.Game.ActorComponents;
using CoreSystem.ProfileComponents;
using UnityEngine;

namespace CoreSystem.Game.Skill
{
    public class SpellResult
    {
        public int resultID;
        public SkillBase usedSkillBase;
        public GameActor[] affectedActors;

        public SpellResult(){}

        public void FillResult(int id, SkillBase @base, GameActor[] affected)
        {
            resultID = id;
            usedSkillBase = @base;
            affectedActors = affected;
        }

        public string PrintResult()
        {
            var result = "<"+usedSkillBase.skillProfile.skillName+"_SPELL_RESULT>\n";
            if (resultID == 0)
            {
                result += "SKILL CONDITION FAIL\n";
            }
            else if (resultID == 1)
            {
                result += "SUCCESS\n";
                if (affectedActors == null) return result;
                for (var i = 0; i < affectedActors.Length;i++)
                {
                    result += i +" AFFECT:"+affectedActors[i].gameObject.name+"\n";
                }
            }

            return result;
        }
    }
    [System.Serializable]
    public abstract class SkillBase
    {
        public bool isInit = false;
        public bool useSpellCondition = true;

        public GameActor speller;
        public SkillProfile skillProfile;

        protected Timer coolTimer;
        protected BehaviourJob skillJob = null;

        protected Action<SpellResult> onSpellCallback, onFailCallback, onFinishCallback;
        protected Action<SkillBase> onFinishCool;

        protected SkillBase(GameActor a, SkillProfile p)
        {
            speller = a;
            skillProfile = p;
            coolTimer = new Timer(this, SkillName, CoolTime);
        }

        private IEnumerator ISkillProcess(SpellResult sResult)
        {
            BattleLogic.CreateStartSpellMsg(this, BehaveToVictim);
            onSpellCallback?.Invoke(sResult);
            yield return ISkillBehave(sResult);
            yield return ISkillFinish(sResult);
            BattleLogic.CreateEndSpellMsg(this, BehaveToVictim);
            onFinishCallback?.Invoke(sResult);
            //speller.brain.SpellEntry.EraseEntry();
        }
        /// <summary>
        /// 시전자와 피시전자가 스킬을 통해 어떤 상호작용을 할지 지정한다.
        /// ISkillBehave가 스킬의 효과등을 꾸미고 전체적인 흐름을 잡는다면
        /// BehaveToVictim은 실질적인 영향을 기제한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="victim"></param>
        protected abstract void BehaveToVictim(UnityEngine.Object sender, GameActor victim);
        protected virtual IEnumerator ISkillBehave(SpellResult sResult){yield return null;}

        protected virtual IEnumerator ISkillFinish(SpellResult sResult)
        {
            yield return null;
        }
        protected IEnumerator IAnimate(GameActor target, AnimationDesc animDesc)
        {
            var animateEntry = target.brain.AnimateEntry;
            var animName = animDesc.name;
            var speed = animDesc.speedMultiplier;
            switch (animDesc.animationType)
            {
                case AnimationType.Frame:
                {
                    var controller = animateEntry.GetFrameAnimator;
                    var animateJob = ActorUtility.PlayAnimation(target.brain, animName, 0, speed, false, controller);
                    yield return animateJob.StartAsCoroutine();
                    break;
                }
                case AnimationType.Spine:
                {
                    var controller = animateEntry.GetSpineAnimator;
                    var animateJob = ActorUtility.PlayAnimation(target.brain, animName, 0, speed, false, controller);
                    yield return animateJob.StartAsCoroutine();
                    break;
                }
            }
        }

        protected void FlipX(GameActor target, bool val)
        {
            var animateEntry = target.brain.AnimateEntry;
            if (animateEntry.IsFrameBase)
            {
                animateEntry.GetFrameAnimator.FlipX(val);
            }
            else
            {
                animateEntry.GetSpineAnimator.FlipX(val);
            }
        }

        protected void ScaleX(GameActor target, float scaleX)
        {
            var animateEntry = target.brain.AnimateEntry;
            if(animateEntry.IsFrameBase)
                target.brain.ScaleX(animateEntry.controller.transform, scaleX);
            else
            {
                animateEntry.GetSpineAnimator.ScaleX = scaleX;
            }
        }

        protected void KnockEffect(GameActor target, Vector3 dir)
        {
            dir.x *= skillProfile.knockBack;
            dir.y *= skillProfile.knockUp;
            target.brain.MoveEntry.SetVelocity(dir);
        }
        protected MainCollider GetCollider(string colName)
        {
            return speller.GetCollider(colName);
        }

        protected IEnumerator IWaitForAnimateEnd()
        {
            yield return ActorUtility.IWaitForEndOfAnimate(speller.brain);
        }

        protected virtual void OnStopSkillProcess()
        {
            skillJob.Kill();
        }
        //  1:Success, 0:FAIL
        public SpellResult Spell(bool useCondition)
        {
            var spellResult = new SpellResult();
            if (useCondition)
            {
                if (!IsCoolDowned)
                {
                    spellResult.FillResult(0, this,null);
                }
                else
                {
                    spellResult.FillResult(1, this, null);
                    skillJob = BehaviourJob.Make(ISkillProcess(spellResult));
                    ResetCool();
                }
            }
            else
            {
                spellResult.FillResult(1, this, null);
                skillJob = BehaviourJob.Make(ISkillProcess(spellResult));
            }
            return spellResult;
        }

        public void Stop()
        {
            OnStopSkillProcess();
        }
        public SkillBase AddCoolDownCallback(Action<SkillBase> onCooled)
        {
            onFinishCool += onCooled;
            return this;
        }

        public SkillBase RemoveCoolDownCallback(Action<SkillBase> onCooled)
        {
            onFinishCool -= onCooled;
            return this;
        }

        public SkillBase AddSpellCallback(Action<SpellResult> onSpell)
        {
            onSpellCallback += onSpell;
            return this;
        }

        public SkillBase AddFailCallback(Action<SpellResult> onFail)
        {
            onFailCallback += onFail;
            return this;
        }

        public SkillBase AddFinishCallback(Action<SpellResult> onFinish)
        {
            onFinishCallback += onFinish;
            return this;
        }
        public SkillBase RemoveSpellCallback(Action<SpellResult> onSpell)
        {
            onSpellCallback -= onSpell;
            return this;
        }
        public SkillBase RemoveFailCallback(Action<SpellResult> onFail)
        {
            onFailCallback -= onFail;
            return this;
        }

        public SkillBase RemoveFinishCallback(Action<SpellResult> onFinish)
        {
            onFinishCallback -= onFinish;
            return this;
        }
        public bool CoolDown(float delta)
        {
            if(coolTimer.timerState != TimerState.RUNNING)
                coolTimer.StartTimer();
            coolTimer.UpdateTimer(delta,null, onFinishCool);
            return IsCoolDowned;
        }

        public void ResetCool()
        {
            coolTimer.ResetTimer(coolTimer.timerState);
        }

        protected IEnumerator IWaitReceiveSense(string sense, Action reaction)
        {
            var waitFlag = false;
            reaction += () => waitFlag = true;
            speller.brain.RegisterSensorEvent(sense, reaction);
            while (!waitFlag)
            {
                yield return null;
            }
            speller.brain.CancelSensorEvent(sense, reaction);
        }

        public bool IsCoolDowned => coolTimer.IsOver();

        public ActorBrain Brain => speller.brain;
        public BehaviourJob SkillJob => skillJob;
        public string SkillName => skillProfile.skillName;
        public SkillType SkillType => skillProfile.skillType;
        public SkillCoverageType SkillCoverageType => skillProfile.skillCoverageType;
        public bool IsGroundSkill => skillProfile.isGroundSkill;
        public bool IsGlobalSkill => skillProfile.isGlobalSkill;
        public float CoolTime => skillProfile.coolTime;
        public Timer CoolTimer => coolTimer;
    }
}
