using System;
using System.Collections.Generic;
using CoreSystem.Additional.CommandAction;
using CoreSystem.Game.Skill;
using UnityEngine;

namespace CoreSystem.Game.ActorComponents
{
    public class PlayerBrain : ActorBrain
    {
        public FrameAnimatorController animateController;

        [SerializeField]
        private static CommandActionProcessor CAP => CommandActionProcessor.Instance;

        public Vector2 moveKeyValue = Vector2.zero;
        public bool moveKeyDown = false;
        public bool lookDownKeyDown = false;
        public bool lookUpKeyDown = false;

        public override void LoadWithAwake(GameActor a)
        {
            behaviours.Add(moveBehaviour);
            behaviours.Add(animateController);
            AnimateEntry.controller = animateController;

            base.LoadWithAwake(a);
            CAP.Initialize("Player_Skill_Tree", actor.SkillView.Elements);
            CAP.onCompleteProcess += OnReceiveCommandSkill;

            //  Set Default State
            FSM.SetState(0);
        }

        public override void BuildStateMap(ActorFSM fsm)
        {
            fsm.AddState("IDLE", OnEnterIdle, OnUpdateIdle);
            fsm.AddState("MOVE", OnEnterRunning, OnUpdateRunning, OnExitRunning);
            fsm.AddState("SIT", OnEnterSit,  OnUpdateSit, OnExitSit);
            fsm.AddState("FALL", OnEnterFall, OnUpdateFall, OnExitFall);
            fsm.AddState("JUMP", OnEnterJump, OnUpdateJump, OnExitJump);
            fsm.AddState("SPELL", OnEnterSpell, OnUpdateSpell);
            //  6 : DAMAGE
            fsm.AddState("DAMAGE", OnEnterDamage, OnUpdateDamage, OnExitDamage);
            //  7 : PARRY
            fsm.AddState("PARRY", OnEnterParry);
            //  8 : AVOID
            //  9 : DEAD
        }

        public override void BuildStateEdges(ActorFSM fsm)
        {
            //  IDLE Condition
            fsm.AddCondition(0, 1, IdleToRunning)
                .AddCondition(0, 2, IdleToSit)
                .AddCondition(0, 3, IsFall)
                .AddCondition(0, 4, IsGround)
                .AddCondition(0, 5, IsSpellable)
                .AddCondition("IDLE","DAMAGE", AlwaysTrue);
            //  MOVE Conditions
            fsm.AddCondition(1, 0, RunningToIdle)
                .AddCondition(1, 1, RunningCircle)
                .AddCondition(1, 2, RunningToSit)
                .AddCondition(1, 3, IsFall)
                .AddCondition(1, 4, IsGround)
                .AddCondition(1, 5, IsSpellable)
                .AddCondition("MOVE","DAMAGE", AlwaysTrue);
            //  SIT Conditions
            fsm.AddCondition(2, 0, SitToIdle)
                .AddCondition(2, 1, SitToMove)
                .AddCondition(2, 3, IsFall)
                .AddCondition(2, 4, IsGround)
                .AddCondition(2, 5, IsSpellable)
                .AddCondition("SIT", "DAMAGE", AlwaysTrue);
            //  FALL Conditions
            fsm.AddCondition(3, 0, IsGround)
                .AddCondition(3, 1, IdleToRunning)
                .AddCondition(3, 4, IsGround)
                .AddCondition(3, 5, IsSpellable)
                .AddCondition("FALL", "DAMAGE", AlwaysTrue);
            //  JUMP Conditions
            fsm.AddCondition(4, 3, IsFall)
                .AddCondition(4, 4, IsGround)
                .AddCondition(4, 5, IsSpellable)
                .AddCondition("JUMP", "DAMAGE", AlwaysTrue);
            //  SPELL Conditions
            fsm.AddCondition(5, 0, IsGround)
                .AddCondition(5, 3, IsFall)
                .AddCondition(5, 5, SpellToSpell)
                .AddCondition("SPELL", "DAMAGE", AlwaysTrue);
            //  DAMAGE Conditions
            fsm.AddCondition("DAMAGE", "IDLE", IsNotKnockOut)
                .AddCondition("DAMAGE", "MOVE", IsNotKnockOut)
                .AddCondition("DAMAGE", "FALL", IsNotKnockOut);
            //  PARRY Conditions
            fsm.AddCondition("PARRY", "SPELL", ParryToSpell)
                .AddCondition("PARRY", "IDLE", IsAnimateEnd)
                .AddCondition("PARRY", "MOVE", IsGround)
                .AddCondition("PARRY", "FALL", IsFall)
                .AddCondition("PARRY", "DAMAGE", AlwaysTrue);

        }

        protected override void Imagine(ActorBehaviourEntry entry)
        {

        }

        protected override void Decide(ActorBehaviourEntry entry)
        {
            animateController.SetFloat("SpeedX", Mathf.Abs(MoveEntry.prevVelocity.x));
            animateController.SetFloat("SpeedY", Mathf.Abs(MoveEntry.prevVelocity.y));
        }

        protected void SpellSkill(SkillBase skillBase)
        {
            SpellEntry.FillEntry(skillBase, true);
            BattleLogic.CreateChangeStateMsg("SPELL", actor, actor);
        }
        protected void OnReceiveCommandSkill(SkillProfile skillProfile)
        {
            var skillEntry = actor.SkillView.Find(skillProfile);
            var isLearned = null != skillEntry;
            if (!isLearned) return;
            //  Command Action을 축적하기 위해 오버라이드해서 사용
            CAP.AddSkillToBuffer(skillProfile);
            SpellSkill(skillEntry);
        }
        public override void OnReceiveInput(string inputName, object value = null)
        {
            switch (inputName)
            {
                case "OnMove":
                {
                    var vec = (Vector2)value;
                    moveKeyValue = vec;
                    if (moveKeyValue.x != 0f)
                    {
                        if (moveKeyValue.x > 0)
                            moveKeyValue.x = 1f;
                        else
                            moveKeyValue.x = -1f;
                    }
                    moveKeyDown = Math.Abs(moveKeyValue.x) > 0;

                    var curDir = Vector3.right * moveKeyValue.x;
                    
                    if (moveKeyDown)
                    {
                        var prevLook = MoveEntry.curLook;
                        var curLook = curDir.x > 0;
                        var lookChange = (prevLook != curLook);
                        CAP.InputCommandAction(lookChange ? "Back" : "Forward");
                        MoveEntry.SetDirtyDir(moveKeyValue.x * Vector3.right);
                    }
                    else
                    {
                        MoveEntry.direction = Vector3.zero;
                    }
                    Move(curDir, moveBehaviour.CalcSpeed());
                    break;
                }
                case "OnLookUp":
                {
                    lookUpKeyDown = !lookUpKeyDown;
                    if(lookUpKeyDown)
                        CAP.InputCommandAction("Up");
                    break;
                }
                case "OnLookDown":
                {
                    lookDownKeyDown = !lookDownKeyDown;
                    animateController.animator.SetBool("onSit",lookDownKeyDown);
                    if (lookDownKeyDown)
                    {
                        CAP.InputCommandAction("Down");
                        BattleLogic.CreateChangeStateMsg(2, actor);
                    }

                    break;
                }
                case "OnJump":
                {
                    lookUpKeyDown = !lookUpKeyDown;
                    if (!lookUpKeyDown) return;
                    Move(MoveEntry.direction, moveBehaviour.CalcSpeed(), true);
                    break;
                }
                case "OnFire":
                    CAP.InputCommandAction("F");
                    break;
                case "OnParry":
                    CAP.InputCommandAction("P");
                    break;
                case "OnAvoid":
                    CAP.InputCommandAction("A");
                    break;
                case "OnInteract":
                    CAP.InputCommandAction("I");
                    break;
            }
        }

        #region State Condition Funcs Region
        private bool IdleToRunning()
        {
            return MoveEntry.isGrounded && Mathf.Abs(moveKeyValue.x) > 0;
        }
        private bool IdleToSit()
        {
            var grounded = MoveEntry.isGrounded;
            return grounded && lookDownKeyDown;
        }
        private bool AlwaysTrue() => true;
        private bool RunningCircle()
        {
            return IsGround();
        }
        private bool RunningToIdle()
        {
            return MoveEntry.isGrounded && !moveKeyDown;
        }
        private bool RunningToSit()
        {
            return lookDownKeyDown;
        }

        private bool SitToIdle()
        {
            return !lookDownKeyDown && !moveKeyDown;
        }
        private bool SitToMove()
        {
            return !lookDownKeyDown && moveKeyDown;
        }

        private bool SpellToSpell()
        {
            return IsAnimateEnd();
        }
        private bool ParryToSpell()
        {
            return true;
        }

        private bool IsGround()
        {
            return MoveEntry.isGrounded;
        }
        private bool IsAnimateEnd()
        {
            var layer = AnimateEntry.trackIndex;
            return AnimateEntry.IsAnimateEnd(layer);
        }
        private bool IsFall()
        {
            return !MoveEntry.isGrounded;
        }
        private bool IsSpellable()
        {
            var dirtySpell = SpellEntry.isDirty;
            if (!dirtySpell) return false;
            var cool = SpellEntry.skillBase.IsCoolDowned;
            if (!cool) return false;
            var ground = SpellEntry.skillBase.IsGroundSkill && IsGround();
            if (ground) return true;
            var air = !SpellEntry.skillBase.IsGroundSkill && !IsGround();
            return air;
        }

        private bool IsNotKnockOut()
        {
            return !IsKnockOut();
        }
        private bool IsKnockOut()
        {
            return DamageEntry.knockTimer.timerState == TimerState.RUNNING;
        }
        #endregion

        #region State Behaviour Region
        #region Idle State Callback
        private BehaviourJob OnEnterIdle()
        {
            return ActorUtility.Action(this, x =>
            {
                AnimateEntry.EraseEntry();
            });
        }
        private BehaviourJob OnUpdateIdle()
        {
            void OnGround(ActorBrain brain)
            {
                if(moveKeyDown)
                    Move();
                if (CAP.CountOfBuffer > 0)
                {
                    var top = CAP.GetBufferTop();
                    var skillEntry = actor.SkillView.Find(top);
                    SpellSkill(skillEntry);
                }
                else
                {
                    CAP.ClearBuffer();
                    BattleLogic.CreateChangeStateMsg(IsGround() ? 0 : 3, actor);
                }
            }
            void OnFall(ActorBrain brain)
            {
                BattleLogic.CreateChangeStateMsg(3, actor);
                //FSM.TransState(3,  OnEnterFall,  OnUpdateFall,  OnExitFall);
            }
            return ActorUtility.LoopMoveWithFall(this, OnGround, OnFall);
        }
        #endregion
        #region Running State Callback
        private BehaviourJob OnEnterRunning()
        {
            MoveEntry.AddSpeed(moveKeyValue.x, moveBehaviour.CalcSpeed());
            return ActorUtility.Action(this, x =>
            {
                AnimateEntry.EraseEntry();
                animateController.SetBool("onMove",true);
            });
        }
        private BehaviourJob OnUpdateRunning()
        {
            void OnGround(ActorBrain brain)
            {
                if (CAP.CountOfBuffer > 0)
                {
                    var top = CAP.GetBufferTop();
                    var skillEntry = actor.SkillView.Find(top);
                    SpellSkill(skillEntry);
                    return;
                }
                
                if (lookDownKeyDown)
                {
                    BattleLogic.CreateChangeStateMsg(2, actor);
                }
                else if (!moveKeyDown && !lookDownKeyDown)
                {
                    BattleLogic.CreateChangeStateMsg(0, actor);
                }
                var dirX = MoveEntry.direction.x;
                MoveEntry.AddSpeed(dirX, moveBehaviour.CalcSpeed());
                if (dirX == 0)
                    return;
                LookAt(Vector3.right * dirX);
            }
            void OnFall(ActorBrain brain)
            {
                var dirX = MoveEntry.direction.x;
                if (dirX == 0)
                    return;
                LookAt(Vector3.right * dirX);
            }
            return ActorUtility.LoopMoveWithFall(this, onGround: OnGround, onFall: OnFall);
        }
        private BehaviourJob OnExitRunning()
        {
            return ActorUtility.Action(this, x =>
            {
                AnimateEntry.EraseEntry();
                animateController.SetBool("onMove", false);
            });
        }
        #endregion
        #region Fall State Callback
        private BehaviourJob OnEnterFall()
        {
            return ActorUtility.Action(this, x =>
            {
                AnimateEntry.EraseEntry();
                animateController.SetBool("onFall", true);
            });
        }
        private BehaviourJob OnUpdateFall()
        {
            void OnLand(ActorBrain brain)
            {
                BattleLogic.CreateChangeStateMsg("IDLE", actor);
            }
            void OnFall(ActorBrain brain)
            {
                if (CAP.CountOfBuffer > 0)
                {
                    var top = CAP.GetBufferTop();
                    var skillEntry = actor.SkillView.Find(top);
                    SpellSkill(skillEntry);
                }
                
                var dirX = moveKeyValue.x;
                if (dirX == 0)
                {
                    return;
                }
                MoveEntry.SetSpeed(dirX, moveBehaviour.CalcSpeed());
                LookAt(Vector3.right * dirX);
            }
            return ActorUtility.LoopMoveWithFall(this, onGround: null, onFall: OnFall, onLanding: OnLand);
        }
        private BehaviourJob OnExitFall()
        {
            return ActorUtility.Action(this, x =>
            {
                AnimateEntry.EraseEntry();
                animateController.SetBool("onFall", false);
            });
        }
        #endregion
        #region Jump State Callback
        private BehaviourJob OnEnterJump()
        {
            MoveEntry.prevVelocity = Vector3.zero;
            MoveEntry.addVelocity = Vector3.zero;
            MoveEntry.SetJump(7.5f);
            return ActorUtility.Action(this, x =>
            {
                AnimateEntry.EraseEntry();
                animateController.SetBool("onJump", true);
            });
        }
        private BehaviourJob OnUpdateJump()
        {
            void OnLand(ActorBrain brain)
            {
                BattleLogic.CreateChangeStateMsg("IDLE", actor);
            }
            void OnFall(ActorBrain brain)
            {
                if (CAP.CountOfBuffer > 0)
                {
                    var top = CAP.GetBufferTop();
                    var skillEntry = actor.SkillView.Find(top);
                    SpellSkill(skillEntry);
                }

                if (MoveEntry.prevVelocity.y < -0.01f)
                {
                    BattleLogic.CreateChangeStateMsg("FALL", actor);
                    return;
                }
                var dirX = moveKeyValue.x;
                if (dirX == 0)
                { 
                    return;
                }
                MoveEntry.SetSpeed(dirX, moveBehaviour.CalcSpeed());
                LookAt(Vector3.right * dirX);
            }
            return ActorUtility.LoopMoveWithFall(this, onGround: null, onFall: OnFall, onLanding: OnLand);
        }
        private BehaviourJob OnExitJump()
        {
            return ActorUtility.Action(this, x =>
            {
                AnimateEntry.EraseEntry();
                animateController.SetBool("onJump", false);
            });
        }
        #endregion
        #region Sit State Callback
        private BehaviourJob OnEnterSit()
        {
            return ActorUtility.Action(this, x =>
            {
                MoveEntry.SetSpeed(MoveEntry.direction.x, 0f);
                AnimateEntry.EraseEntry();
                animateController.SetBool("onSit", true);
            });
        }
        private BehaviourJob OnUpdateSit()
        {
            void OnGround(ActorBrain brain)
            {
                var dirX = MoveEntry.direction.x;
                if (dirX == 0)
                    return;
                LookAt(Vector3.right * dirX);
                //if (Math.Abs(MoveEntry.direction.x) > 0)
                //    animateController.FlipX(MoveEntry.direction.x > 0);
                if (!lookDownKeyDown && !moveKeyDown)
                    BattleLogic.CreateChangeStateMsg(0, actor);
                else if (moveKeyDown)
                    BattleLogic.CreateChangeStateMsg(1, actor);
            }
            return ActorUtility.LoopFall(this, OnGround);
        }
        private BehaviourJob OnExitSit()
        {
            return ActorUtility.Action(this, x =>
            {
                AnimateEntry.EraseEntry();
                animateController.SetBool("onSit", false);
            });
        }

        #endregion
        private BehaviourJob OnEnterSpell()
        {
            //  TODO : SkillFactory에서 사용할 
            void OnSpell(SpellResult sr)
            {
                Debug.Log(sr.PrintResult());
                CAP.ConsumeBuffer();
            }
            void OnFail(SpellResult sr)
            {
                Debug.Log(sr.PrintResult());
                CAP.ClearBuffer();
                BattleLogic.CreateChangeStateMsg(0, actor);
            }
            var spellJob = ActorUtility.SpellProcess(this, SpellEntry.skillBase, onSpell: OnSpell, onFail: OnFail);
            return spellJob;
        }
        private BehaviourJob OnUpdateSpell()
        {
            void UpdateState(ActorBrain obj)
            {
                var curLayer = AnimateEntry.trackIndex;
                if (animateController.isAnimate[curLayer]) return;
                if (CAP.CountOfBuffer > 0)
                {
                    var top = CAP.GetBufferTop();
                    var skillEntry = actor.SkillView.Find(top);
                    SpellSkill(skillEntry);
                }
                else
                {
                    CAP.ClearBuffer();
                    BattleLogic.CreateChangeStateMsg(IsGround() ? 0 : 3, actor);
                }
            }
            var updateJob = ActorUtility.LoopAction(this, UpdateState);
            return updateJob;
        }

        private BehaviourJob OnEnterDamage()
        {
            var animName = "Player01_Damaged";
            if (DamageEntry.EnemySkill.SkillCoverageType == SkillCoverageType.High)
            {
                animName = "Player01_Damaged_Down";
            }
            DamageEntry.knockTimer.StartTimer();
            SpellEntry.skillBase?.Stop();
            var vel = new Vector3(.5f,.5f,0f);
            vel.x *= Mathf.Sign(DamageEntry.dirToSpeller.x);
            vel.x *= DamageEntry.EnemySkill.skillProfile.knockBack;
            vel.y *= DamageEntry.EnemySkill.skillProfile.knockUp;
            Debug.Log(vel);
            MoveEntry.SetVelocity(vel);
            void OnEnter(Animator animator)
            {
                var dir = DamageEntry.dirToSpeller;
                dir.x *= -1f;
                LookAt(dir);
            }
            return ActorUtility.PlayAnimation(this, animName,0, 1f, false, animateController, OnEnter);
        }

        private BehaviourJob OnUpdateDamage()
        {
            void OnKnockFinish(ActorBehaviourEntry.DamageEntry entry)
            {
                DamageEntry.knockTimer.StopTimer();
                DamageEntry.knockTimer.timerState = TimerState.FINISH;
                Debug.Log("KnockFinish");
                if (moveKeyDown) BattleLogic.CreateChangeStateMsg("MOVE", actor);
                else BattleLogic.CreateChangeStateMsg("IDLE", actor);
            }
            void OnGround(ActorBrain brain)
            {
                if(MoveEntry.isGrounded)
                    DamageEntry.UpdateKnockTimer(null, OnKnockFinish);
            }
            void OnFall(ActorBrain brain)
            {
                
            }
            void OnLanding(ActorBrain brain)
            {
                
            }
            return ActorUtility.LoopMoveWithFall(this, OnGround);
        }
        private BehaviourJob OnExitDamage()
        {
            void UpdateState(ActorBrain obj)
            {
                DamageEntry.knockTimer.StopTimer();
                DamageEntry.knockTimer.timerState = TimerState.FINISH;
            }
            var updateJob = ActorUtility.Action(this, UpdateState);
            return updateJob;
        }
        private BehaviourJob OnEnterParry()
        {
            //  DESC : 적의 공격을 패리하는데 성공한 이후 들어오는 스테이트
            //  TODO : 다음 행동을 조건없이 수행할 수 있다.
            //  TODO : 그래도 데미지를 받게 되면 풀리긴 함
            CAP.Reset();
            void OnGround(ActorBrain brain)
            {
                if (moveKeyDown)
                {
                    BattleLogic.CreateChangeStateMsg("MOVE", actor);
                    return;
                }
                var curLayer = AnimateEntry.trackIndex;
                if (AnimateEntry.IsAnimateEnd(curLayer))
                {
                    BattleLogic.CreateChangeStateMsg("IDLE", actor);
                    return;
                }
            }
            void OnFall(ActorBrain brain)
            {
                var curLayer = AnimateEntry.trackIndex;
                if (AnimateEntry.IsAnimateEnd(curLayer))
                {
                    BattleLogic.CreateChangeStateMsg("FALL", actor);
                    return;
                }
            }
            var parryJob = ActorUtility.LoopFall(this, OnGround, OnFall);
            return parryJob;
        }
        #endregion
    }
}
