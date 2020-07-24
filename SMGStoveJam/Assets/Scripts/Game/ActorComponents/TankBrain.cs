using System;
using System.Collections;
using System.Collections.Generic;
using CoreSystem.Game.Skill;
using CoreSystem.ProfileComponents;
using CoreSystem.Utility.CurveMap;
using UnityEngine;

namespace CoreSystem.Game.ActorComponents
{
    public class TankBrain : EnemyBrain
    {
        private Action[] phaseUpdateActions;

        public CharacterController moveController;
        public ParryReactor ParryReactor => actor.ParryReactor;
        public SpineController animateController;

        private AnimationCurve moveSpeedCurve;

        public override void LoadWithAwake(GameActor a)
        {
            base.LoadWithAwake(a);

            phaseUpdateActions = new Action[]
            {
                UpdatePhase01,
                UpdatePhase02
            };
        }
        public override void LoadWithStart(GameActor a)
        {
            base.LoadWithStart(a);
            //  스파인 초기화 때문에 스타트에서 처리해야함
            FSM.SetState(0);
            moveSpeedCurve = CurveMap.GetCurveData("HalfM");
        }

        public override void BuildStateMap(ActorFSM fsm)
        {
            //  0 : IDLE
            //  1 : MOVE
            //  2 : SPELL
            //  3 : DAMAGE
            //  4 : PARRIED
            //  5 : STUN
            //  6 : DEAD
            fsm.AddState("IDLE", OnEnterIdle, OnUpdateIdle)
                .AddState("MOVE", OnEnterMove, OnUpdateMove)
                .AddState("SPELL", OnEnterSpell, OnUpdateSpell)
                .AddState("DAMAGE", OnEnterIdle, OnUpdateIdle)
                .AddState("PARRIED", OnEnterParried, OnUpdateParried)
                .AddState("STUN", OnEnterIdle, OnUpdateIdle)
                .AddState("DEAD", OnEnterIdle, OnUpdateIdle);
        }

        public override void BuildStateEdges(ActorFSM fsm)
        {
            fsm.AddCondition("IDLE","SPELL", IsSpellable)
                .AddCondition("IDLE","MOVE",IsGround)
                .AddCondition("MOVE", "IDLE", IsGround)
                .AddCondition("MOVE", "MOVE", IsGround)
                .AddCondition("MOVE", "SPELL", IsSpellable)
                .AddCondition("SPELL", "IDLE", IsSkillChainFree)
                .AddCondition("SPELL", "MOVE", IsSkillChainFree)
                .AddCondition("SPELL","SPELL", IsSpellable)
                .AddCondition("PARRIED", "SPELL", IsSpellable)
                .AddCondition("PARRIED", "IDLE", IsSkillChainFree)
                .AddCondition("PARRIED", "MOVE", IsSkillChainFree);
        }

        private bool IsSpellable()
        {
            return true;
        }

        private bool IsSkillChainFree()
        {
            if (mPatterns.current == null) return true;
            return mPatterns.current.state == SkillChain.ChainState.FINISH;
        }
        private bool IsGround()
        {
            return MoveEntry.isGrounded;
        }
        //  TODO : Make Pattern Class 
        /*  Skill List
         * 찌르기
         * 레이저
         * 박치기
         * 상단베기
         * 하단베기
         * 하단베기_찌르기
         * 박치기_레이저
         * 연속찌르기
         * 박치기_하단베기_찌르기
         * 도약
         * 연속도약
         */

        protected override void BuildSkillChain(SkillView view)
        {
            base.BuildSkillChain(view);
            mPatterns.SetGlobalFinishChainAction(OnFinishChain)
                .SetFinishChainAction("LaserOnly", OnFinishCoolLaserOnly)
                .SetFinishChainAction("HighOnly", OnFinishCoolHighOnly)
                .SetFinishChainAction("StepStopOnly", OnFinishStepAttack)
                .SetFinishChainAction("StepForwardOnly", OnFinishStepAttack)
                .SetFinishChainAction("Combo01", OnFinishCombo01);
        }
        #region SkillChain Event Callback

        private void OnFinishChain(SkillChain chain)
        {
            mPatterns.PopFromQ(chain);
        }
        private void OnFinishCoolLaserOnly(SkillChain skillChain)
        {
            
        }
        private void OnFinishCoolHighOnly(SkillChain skillChain)
        {
            
        }
        private void OnFinishCombo01(SkillChain skillChain)
        {
            
        }
        private void OnFinishStepAttack(SkillChain skillChain)
        {
            
        }
        #endregion
        public override void OnParried(SkillBase ps, SkillBase es)
        {
            Debug.Log("TANK ON PARRIED PS :" + ps.SkillName + " | ES : " + es.SkillName);
            /* 패리 성공시 행동양상
           -- Enemy의 체간게이지 가감
           -- Enemy는 해당 공격에 해당하는 튕겨나가는 모션 수행 후 다음 행동 진행 (해봐야 함)
           */
            actor.Profile.Chegan -= es.skillProfile.cheganDmg;
            FSM.SetState("PARRIED");
        }
        private void UpdatePhase01()
        {
            //  Manage Phase Condition
            if (CurHp <= DefHp * 0.5f)
            {
                // Phase 1 To 2
                phase++;
                return;
            }

            //  CoolDown Tank Skills
            if (DisToPlayer >= 2.5f)
            {
                Move(DirToPlayer, moveBehaviour.CalcSpeed());
                mPatterns.CoolDown("StepForwardOnly", true);
            }
            else
            {
                FSM.TransState("IDLE");
                mPatterns.CoolDown("StepStopOnly", true);
                mPatterns.CoolDown("Combo01", true);
            }
            //  Make Decision What to do
            mPatterns.UseWithQ();
        }

        private void UpdatePhase02()
        {

        }

        protected override void Imagine(ActorBehaviourEntry entry)
        {
            phaseUpdateActions[phase].Invoke();
        }

        protected override void Decide(ActorBehaviourEntry entry)
        {

        }

        public override void Damage(SkillBase skillBase, float dmg)
        {
            base.Damage(skillBase, dmg);
        }

        #region FSM State Behaviours
        #region Idle State Callback

        private BehaviourJob OnEnterIdle()
        {
            return ActorUtility.PlayAnimation(this, "WeaponIdle", 0, 1f, true, animateController);
        }

        private BehaviourJob OnUpdateIdle()
        {
            void OnGround(ActorBrain brain)
            {
                var dirX = DirToPlayer.x;
                dirX = (dirX > 0) ? 1f : -1f;
                MoveEntry.direction.x = dirX;
                animateController.FlipX(dirX == 1f);
            }

            void OnFall(ActorBrain brain)
            {

            }
            return ActorUtility.LoopMoveWithFall(this, OnGround, OnFall);
        }
        #endregion
        #region Move State Callback

        private float mT = 0f;
        private BehaviourJob OnEnterMove()
        {
            mT = 0f;
            return ActorUtility.PlayAnimation(this, "WeaponWalk", 0, 1f, true, animateController);
        }
        private BehaviourJob OnUpdateMove()
        {
            void OnGround(ActorBrain brain)
            {
                mT += Time.deltaTime;
                var dirX = DirToPlayer.x;
                dirX = (dirX > 0) ? 1f : -1f;
                var mul = moveSpeedCurve.Evaluate(mT);
                var speed = moveBehaviour.CalcSpeed() * mul;
                MoveEntry.AddSpeed(dirX,  speed);
                animateController.FlipX(dirX == 1f);
            }

            void OnFall(ActorBrain brain)
            {

            }
            return ActorUtility.LoopMoveWithFall(this, OnGround, OnFall);
        }
        #endregion
        #region Spell State Callback

        private BehaviourJob OnEnterSpell()
        {
            void OnSpell(SpellResult sr)
            {
                Debug.Log(sr.PrintResult());
            }

            void OnFail(SpellResult sr)
            {
                Debug.Log(sr.PrintResult());
            }

            return ActorUtility.SpellProcess(this, SpellEntry.skillBase, onSpell: OnSpell, onFail: OnFail);
        }

        private BehaviourJob OnUpdateSpell()
        {
            void OnGround(ActorBrain brain)
            {
                if (AnimateEntry.IsAnimateEnd(0))
                {
                    BattleLogic.CreateChangeStateMsg("IDLE", actor, actor);
                }
            }
            void OnFall(ActorBrain brain)
            {
                if (AnimateEntry.IsAnimateEnd(0))
                {
                    BattleLogic.CreateChangeStateMsg("FALL", actor, actor);
                }
            }
            return ActorUtility.LoopFall(this, OnGround, OnFall);
        }

        #endregion

        private BehaviourJob OnEnterParried()
        {
            void OnEnter(ActorBrain brain)
            {
                var skill = SpellEntry.skillBase;
                mPatterns.PauseChain();
                mPatterns.ResetCool();
                BehaviourJob animJob = null;
                switch (skill.SkillCoverageType)
                {
                    case SkillCoverageType.High:
                        animJob = ActorUtility.PlayAnimation(this, "FootStep", 0, 1f, false, animateController);
                        break;
                    case SkillCoverageType.Middle:
                        animJob = ActorUtility.PlayAnimation(this, "FootStep", 0, 1f, false, animateController);
                        break;
                    case SkillCoverageType.Low:
                        animJob = ActorUtility.PlayAnimation(this, "FootStep", 0, 1f, false, animateController);
                        break;
                }
                animJob?.Start();
            }

            return ActorUtility.Action(this, OnEnter);
        }

        private BehaviourJob OnUpdateParried()
        {
            void OnGround(ActorBrain brain)
            {
                if (AnimateEntry.IsAnimateEnd(0))
                {
                    if(mPatterns.current.state != SkillChain.ChainState.FINISH)
                        mPatterns.StartChain();
                }
            }

            void OnFall(ActorBrain brain)
            {
                if (AnimateEntry.IsAnimateEnd(0))
                {
                    if (mPatterns.current.state != SkillChain.ChainState.FINISH)
                        mPatterns.StartChain();
                }

            }

            return ActorUtility.LoopFall(this, OnGround, OnFall);
        }
        #endregion
    }
}
