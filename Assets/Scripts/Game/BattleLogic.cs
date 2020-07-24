using System;
using System.Collections.Generic;
using CoreSystem.Game;
using CoreSystem.Game.Skill;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class LogicMessage
{
    public enum MessageID
    {
        CHANGE_STATE,
        SPELL_START,
        SPELL_BEHAVE,
        SPELL_END,
        PARRY_TRY,
        PARRY_SUCCESS,
        PARRY_FAIL,
        NOTICE,
        USE_ITEM
    }

    public MessageID mId;
    public UnityEngine.Object sender;
    public List<UnityEngine.Object> receiverList = new List<UnityEngine.Object>();
    public Action<LogicMessage> msgAction;
    private readonly object[] parameters;

    public bool GetBool(int idx) => (bool)parameters[idx];
    public float GetFloat(int idx) => (float)parameters[idx];
    public int GetInt(int idx) => (int)parameters[idx];
    public string GetString(int idx) => (string)parameters[idx];
    public object GetObject(int idx) => parameters[idx];

    public LogicMessage(MessageID id, [NotNull] Action<LogicMessage> handleAction, [NotNull] UnityEngine.Object s,
        [NotNull] List<UnityEngine.Object> receivers, object[] paramObjects = null)
    {
        mId = id;
        sender = s;
        msgAction = handleAction;
        receiverList = receivers;
        parameters = paramObjects;
        BattleLogic.Instance.logicMessages.Add(this);
    }

    public void HandleMsg()
    {
        msgAction?.Invoke(this);
    }

    public ActorBehaviourEntry BehaviourEntry => ((GameActor)sender)?.brain?.behaviourEntry;
    public ActorBehaviourEntry.MoveEntry MoveEntry => BehaviourEntry.moveEntry;
    public ActorBehaviourEntry.AnimateEntry AnimateEntry => BehaviourEntry.animateEntry;
    public ActorBehaviourEntry.SpellEntry SpellEntry => BehaviourEntry.spellEntry;
    public ActorBehaviourEntry.DamageEntry DamageEntry => BehaviourEntry.damageEntry;
}

public class BattleLogic : SingletonGameObject<BattleLogic>
{
    public GameActor player;
    public List<LogicMessage> logicMessages = new List<LogicMessage>();

    //  1. 플레이어가 패링을 시도, 이 패링에 맞은 액터가
    //  공격을 시도중인지 확인
    //  2. 액터간 공격과 피격 가능 여부 판단과 수행
    //  => 매 프레임마다 상호작용 이벤트를 축적, 가공, 소모해야함
    //  => 이벤트에 필요한 프로퍼티
    //  ==> [행위ID][시행자][피수행자][Param]

    [Serializable]
    public class FrameMonitor
    {
        public int pCurFsmState = -1;
        public int pTargetFsmState = -1;

        public bool needCheckParry = false;
        public bool flagParrySuccess = false;
        public List<LogicMessage> msgEnemyBehaveSpellList = new List<LogicMessage>();
        public void Reset()
        {
            needCheckParry = false;
            flagParrySuccess = false;
        }
    }

    public FrameMonitor monitor;

    #region Create Message Funcs
    public static LogicMessage CreateChangeStateMsg(int stateIdx, GameActor sender, GameActor receiver = null)
    {
        if (receiver == null)
            receiver = sender;
        if (receiver == Instance.player)
        {
            Instance.monitor.pCurFsmState = Instance.player.FSM.curIdx;
            Instance.monitor.pTargetFsmState = stateIdx;
        }
        var receivers = new List<UnityEngine.Object> { receiver };
        var parameters = new object[1];
        parameters[0] = stateIdx;
        var msg = new LogicMessage(LogicMessage.MessageID.CHANGE_STATE, HandleActorFsmChangeMSG, sender, receivers, parameters);
        return msg;
    }

    public static LogicMessage CreateChangeStateMsg(string stateName, GameActor sender, GameActor receiver = null)
    {
        var stateIdx = sender.FSM.nameToIdxMap[stateName];
        return CreateChangeStateMsg(stateIdx, sender, receiver);
    }

    public static LogicMessage CreateStartSpellMsg(SkillBase skill, Action<Object, GameActor> skillBehave)
    {
        var receivers = new List<UnityEngine.Object>{skill.speller};
        var parameters = new object[]{skill, skillBehave};
        
        var msg = new LogicMessage(LogicMessage.MessageID.SPELL_START, HandleSpellStartMSG, skill.speller, receivers, parameters);
        return msg;
    }
    //  스펠된 스킬의 범위 안에 타겟이 하나 이상 존재 할 때 생성됨
    //  => 해당 타겟에게 행동하기 위한 메시지기 때문이다.
    public static LogicMessage CreateBehaveSpellMsg(SkillBase skill, Action<Object, GameActor> skillBehave, GameActor sender,
        GameActor victim)
    {
        var receivers = new List<UnityEngine.Object> {victim};
        var parameters = new object[]{skill, skillBehave};
    
        var msg = new LogicMessage(LogicMessage.MessageID.SPELL_BEHAVE, HandleSpellBehaveMSG, sender, receivers,
            parameters);
        if (sender.CompareTag("Enemy"))
        {
            Instance.monitor.msgEnemyBehaveSpellList.Add(msg);
            if(victim.CompareTag("Player") && victim.brain.SpellEntry.isColliderActivated) Instance.monitor.needCheckParry = true;
        }
        return msg;
    }

    public static LogicMessage CreateEndSpellMsg(SkillBase skill, Action<Object, GameActor> skillBehave)
    {
        var receivers = new List<UnityEngine.Object> { skill.speller};
        var parameters = new object[] { skill , skillBehave };
        var msg = new LogicMessage(LogicMessage.MessageID.SPELL_END, HandleSpellEndMSG, skill.speller, receivers, parameters);
        return msg;
    }

    public static LogicMessage CreateTryParryMsg(LogicMessage eSpellMSG)
    {
        var enemy = ((SkillBase)eSpellMSG.GetObject(0)).speller;
        var receivers = eSpellMSG.receiverList;
        var parameters = new object[]{eSpellMSG};
        
        var msg = new LogicMessage(LogicMessage.MessageID.PARRY_TRY,HandleParryTryMSG, enemy, receivers,parameters);
        
        return msg;
    }
    public static LogicMessage CreateParrySuccessMsg(LogicMessage eSpellMSG)
    {
        var enemy = ((SkillBase)eSpellMSG.GetObject(0)).speller;
        var receivers = eSpellMSG.receiverList;
        var parameters = new object[]{eSpellMSG};
        
        var msg = new LogicMessage(LogicMessage.MessageID.PARRY_SUCCESS,HandleParrySuccessMSG, enemy, receivers,parameters);
        return msg;
    }
    public static LogicMessage CreateParryFailMsg(LogicMessage eSpellMSG)
    {
        var enemy = ((SkillBase)eSpellMSG.GetObject(0)).speller;
        var receivers = eSpellMSG.receiverList;
        var parameters = new object[]{eSpellMSG};
        
        var msg = new LogicMessage(LogicMessage.MessageID.PARRY_FAIL,HandleParryFailMSG, enemy, receivers,parameters);
        return msg;
    }
    #endregion

    #region Message Handle Funcs
    private static void HandleActorFsmChangeMSG(LogicMessage msg)
    {
        for (var i = 0; i < msg.receiverList.Count; i++)
        {
            var receiver = (GameActor)msg.receiverList[i];
            var targetState = msg.GetInt(i);
            receiver.FSM_TransState(targetState);
        }
    }

    private static void HandleSpellStartMSG(LogicMessage msg)
    {
        var sender = msg.sender;
        var skill = (SkillBase) msg.GetObject(0);
    }
    private static void HandleSpellBehaveMSG(LogicMessage msg)
    {
        var sender = (GameActor)msg.sender;
        var skill = (SkillBase) msg.GetObject(0);
        var skillBehave = (Action<UnityEngine.Object, GameActor>)msg.GetObject(1);
        for (var i = 0; i < msg.receiverList.Count; i++)
        {
            var victim = (GameActor)msg.receiverList[i];
            var dirToSpeller = (victim.transform.position - skill.speller.transform.position).normalized;
            victim.brain.DamageEntry.FillEntry(skill, dirToSpeller);
            skillBehave?.Invoke(sender, victim);
        }

    }
    private static void HandleSpellEndMSG(LogicMessage msg)
    {
        var sender = msg.sender;
        var skill = (SkillBase)msg.GetObject(0);
        if (skill.speller.CompareTag("Enemy"))
        {
            var behaveMsg = Instance.monitor.msgEnemyBehaveSpellList.Find(x => x.sender == msg.sender);
            Instance.monitor.msgEnemyBehaveSpellList.Remove(behaveMsg);
        }
    }

    private static void HandleParryTryMSG(LogicMessage msg)
    {
        //  Extract MSG From TryParryMSG
        var eSpellMSG = (LogicMessage)msg.GetObject(0);
        var enemy  = (GameActor)eSpellMSG.sender;
        var player = (GameActor)eSpellMSG.receiverList[0];
        //  Extract Skill rom SpellBehaveMSG
        var eSkill = (SkillBase)eSpellMSG.GetObject(0);
        var pSkill = player.brain.SpellEntry.skillBase;
        Debug.Log("PARRY_TRY");
        var reactor = enemy.ParryReactor;
        if(null == reactor)
            return;
        var result = reactor.Test(pSkill, eSkill);
        Instance.monitor.flagParrySuccess = result;
        /* Parry Logic
           패리에 성공한 SpellBehaveMSG는 삭제된다.
           패리가 성공했음을 알리는 ParrSuccessMSG가 생성된다.
        */
    }
    private static void HandleParrySuccessMSG(LogicMessage msg)
    {
         Debug.Log("PARRY_SUCCESS");
        //  Extract MSG From ParrySuccessMSG
        var eSpellMSG = (LogicMessage)msg.GetObject(0);
        var enemy = (GameActor)eSpellMSG.sender;
        var player = (GameActor)eSpellMSG.receiverList[0];

        /* 패리 성공시 행동양상
        -- Enemy의 기절게이지 가감
        -- Player의 다음 행동이 딜레이 없이 바로 수행 => Parry State를 만들어서 여기서 다른 스테이트로 넘어가게 지정
        -- Enemy는 해당 공격에 해당하는 튕겨나가는 모션 수행 후 다음 행동 진행 (해봐야 함)
        */  
        //  Extract Skill From SpellBehaveMSG
        var eSkill = (SkillBase)eSpellMSG.GetObject(0);
        var pSkill = player.brain.SpellEntry.skillBase;
        player.FSM_SetState("PARRY");
        enemy.ParryReactor.onSuccessAction?.Invoke(pSkill, eSkill);
    }
    private static void HandleParryFailMSG(LogicMessage msg)
    {
         Debug.Log("PARRY_FAIL");
          //  Extract MSG From TryParryMSG
        var eSpellMSG = (LogicMessage)msg.GetObject(0);
        var victim = (GameActor)eSpellMSG.receiverList[0];
        //  Extract Skill rom SpellBehaveMSG
        var eSkill = (SkillBase)eSpellMSG.GetObject(0);
        var pSkill = victim.brain.SpellEntry.skillBase;
    }
    #endregion
    private void Update()
    {
        //  TODO : 1차 메시지 가공
        //  Check Attack_PARRY Situation 
        if (monitor.needCheckParry)
        {
            //  pSpellMsg : PlayerSkill Spell Msg
            for (var i = 0; i < monitor.msgEnemyBehaveSpellList.Count; i++)
            {
                //  eSpellMsg : playerSkill
                var eSpellMsg = monitor.msgEnemyBehaveSpellList[i];
                //  이 메시지를 플레이어와 적의 스킬 행동 처리 메시지보다
                //  앞에 오게해서 적의 스킬 행동으로 인한 플레이어의 피해를 제거해야 한다.
                var msg = CreateTryParryMsg(eSpellMsg);
                //  Invoke TestParryMsg
                msg.HandleMsg();
                logicMessages.Remove(msg);
                if(monitor.flagParrySuccess)
                {
                    msg = CreateParrySuccessMsg(eSpellMsg);
                    //  Enemy가 Player에게 스킬을 적용하는 메시지 삭제
                    logicMessages.Remove(eSpellMsg);
                }
                else
                {
                    msg = CreateParryFailMsg(eSpellMsg);
                }
            }
        }
        //  TODO : Consume Messages
        while (logicMessages.Count != 0)
        {
            var msg = logicMessages[0];
            logicMessages.RemoveAt(0);
            msg.HandleMsg();
        }
        monitor.Reset();
    }
}
