using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreSystem.Game.Skill;
using UnityEngine;

namespace CoreSystem.Game
{
    //  SkillChainDesc를 통해 생성될 수 있어야 함
    [System.Serializable]
    public class SkillChain
    {
        public enum ChainState
        {
            RUNNING,
            PAUSE,
            FINISH
        }

        public string Name;
        [SerializeField]
        private bool useLocalTimer = false;
        private GameActor owner;
        public int priority = 0;
        public ChainState state = ChainState.FINISH;
        private int index = -1;
        
        [SerializeField]
        private SkillBase[] chainedElements;
        private Action<SkillChain> onFinishCool;
        private Action<SkillChain> onFinishPattern;
        [SerializeField]
        private Timer localTimer;
        private BehaviourJob chainJob;

        public static SkillChain Create(int prior, string name, GameActor o, Action<SkillChain> onFinishCool,
            params SkillBase[] skillElements)
        {
            var pattern = new SkillChain {Name = name, owner = o, onFinishCool = onFinishCool, chainedElements = skillElements};
            return pattern;
        }
        private void Next(SpellResult r)
        {
            index = Mathf.Clamp(++index, 0, chainedElements.Length);
            if (chainedElements.Length > index)
            {
                Debug.Log("Next");
                Use(false);
            }
            else
            {
                //if(index - 1 == chainedElements.Length)
                //    chainedElements[index - 1].RemoveFinishCallback(Next);
                Debug.Log("END");
                onFinishPattern?.Invoke(this);
                Stop();
                return;
            }
        }
        private SpellResult Use(bool useCool = true)
        {
            var skill = chainedElements[index];
            //Debug.Log("Index : "+index+" | Use : " + skill.SkillName);
            var sr = new SpellResult();
            if (useCool && !IsCoolDowned)
            {
                sr.FillResult(0, skill,null);
                return sr;
            }
            skill.AddFinishCallback(Next);
            owner.SpellSkill(skill, useCool);
            sr.FillResult(1, skill,null);
            return sr;
        }
        private void Reset()
        {
            index = -1;
            ResetCool();
            for (var i = 0; i < chainedElements.Length; i++)
            {
                var e = chainedElements[i];
                e.RemoveFinishCallback(Next);
            }
        }
        #region Public Exposed Funcs Region 
        public SkillChain Start(bool worryCool)
        {
            if (state == ChainState.PAUSE) Resume(worryCool);
            else
            {
                index = 0;
                state = ChainState.RUNNING;
                var result = Use(worryCool);
            }
            return this;
        }

        public bool CurrentlyRunning => state == ChainState.RUNNING;
        public SkillChain Stop()
        {
            state = ChainState.FINISH;
            Reset();
            return this;
        }

        public SkillChain Pause()
        {
            state = ChainState.PAUSE;
            //  기존 패턴은 생략해버린다.
            chainedElements[index].RemoveFinishCallback(Next);
            //++index;
            return this;
        }

        public SkillChain Resume(bool worryCool)
        {
            index = Mathf.Clamp(index, 0, chainedElements.Length - 1);
            state = ChainState.RUNNING;
            Next(null);
            //var result = Use(worryCool);
            return this;
        }
        public SkillChain SetLocalTimer(float duration)
        {
            useLocalTimer = true;
            localTimer = new Timer(this, Name, duration);
            return this;
        }

        public SkillChain SetFinishCoolAction(Action<SkillChain> onAction, bool add = true)
        {
            if(add) onFinishCool += onAction;
            else    onFinishCool -= onAction;
            return this;
        }
        public SkillChain SetFinishPatternAction(Action<SkillChain> onAction, bool add = true)
        {
            if(add) onFinishPattern += onAction;
            else    onFinishPattern -= onAction;
            return this;
        }
        
        public SkillChain CoolDown()
        {
            if (useLocalTimer)
            {
                if (localTimer.timerState != TimerState.RUNNING)
                {
                    localTimer.StartTimer();
                }
                localTimer.UpdateTimer(Time.deltaTime,null, onFinishCool);
            }
            else
            {
                foreach (var skill in chainedElements)
                {
                    skill.CoolDown(Time.deltaTime);
                }
                if(IsCoolDowned) onFinishCool?.Invoke(this);
            }
            return this;
        }

        public SkillChain ResetCool()
        {
            if (useLocalTimer)
            {
                localTimer.ResetTimer(localTimer.timerState);
            }
            else
            {
                foreach (var skill in chainedElements)
                {
                    skill.ResetCool();
                }
            }
            return this;
        }
        #endregion

        public bool UseLocalTimer => useLocalTimer;
        public bool IsCoolDowned {
            get
            {
                if (useLocalTimer)
                {
                    return localTimer.IsOver();
                }
                else
                {
                    var coolCount = chainedElements.Select(skill => skill.IsCoolDowned).Count(flag => flag);
                    return coolCount == chainedElements.Length;
                }
            }
        }
    }
}
