using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Game.ActorComponents
{
    public interface ISetActorFSM
    {
        void BuildStateMap(ActorFSM fsm);
        void BuildStateEdges(ActorFSM fsm);
    }
    public class ActorFSM : MonoBehaviour
    {
        public int curIdx = 0;
        public GameActor actor;
        public Action<State, State> onTransSuccess;
        public Action<TransResult, State, State> onTransFail;

        [Serializable]
        public class State
        {
            public int idx;
            public string name;
            private ActorFSM parent;
            public Func<BehaviourJob> enterAction;
            public Func<BehaviourJob> updateAction;
            public Func<BehaviourJob> exitAction;

            private BehaviourJob enterJob;
            private BehaviourJob updateJob;
            private BehaviourJob exitJob;

            
            public State(ActorFSM fsm, int i, string n, Func<BehaviourJob> enterFunc = null,
                Func<BehaviourJob> updateFunc = null, Func<BehaviourJob> exitFunc = null)
            {
                parent = fsm;
                idx = i;
                name = n;
                enterAction = enterFunc;
                updateAction = updateFunc;
                exitAction = exitFunc;
            }

            public void GiveJobs(Func<BehaviourJob> enterFunc = null,
                Func<BehaviourJob> updateFunc = null, Func<BehaviourJob> exitFunc = null)
            {
                if(enterFunc != null)
                    enterAction = enterFunc;
                if(updateFunc != null)
                    updateAction = updateFunc;
                if(exitFunc != null)
                    exitAction = exitFunc;

                enterJob = enterAction?.Invoke();
                updateJob = updateAction?.Invoke();
                exitJob = exitAction?.Invoke();
            }
            public void OnEnter()
            {
                enterJob?.Start();
            }

            public void OnUpdate()
            {
                updateJob?.Start();
            }
            public void OnExit()
            {
                enterJob?.Kill();
                updateJob?.Kill();
                exitJob?.Start();
            }

            public bool IsEnterRunning => null != enterJob && enterJob.IsRunning;
            public bool IsUpdateRunning => null != updateJob && updateJob.IsRunning;
            public bool IsExitRunning => null != exitJob && exitJob.IsRunning;
        }

        public class TransResult
        {
            public bool result;
            //  0 : NO ERROR
            //  1 : NO CONDITION
            //  2 : FALSE CONDITION
            public int errorType;
            public string context;

            public TransResult(bool r, int type = 0, string con = "")
            {
                result = r;
                errorType = type;
                context = con;
            }
        }
        public List<State> states = new List<State>();
        public Dictionary<string, int> nameToIdxMap = new Dictionary<string, int>();
        public Dictionary<Tuple<int, int>, Func<bool>> conditionFuncMap = new Dictionary<Tuple<int, int>, Func<bool>>();

        
        public ActorFSM AddState(string stateName, Func<BehaviourJob> enterFunc = null,
            Func<BehaviourJob> updateFunc = null, Func<BehaviourJob> exitFunc = null)
        {
            var newState = new State(this, states.Count-1, stateName, enterFunc, updateFunc, exitFunc);
            states.Add(newState);
            nameToIdxMap[stateName] = states.Count-1;
            return this;
        }

        public ActorFSM AddCondition(int from, int to, Func<bool> func)
        {
            conditionFuncMap[new Tuple<int, int>(from, to)] = func;
            return this;
        }
        public ActorFSM AddCondition(string from, string to, Func<bool> func)
        {
            var fIdx = nameToIdxMap[from];
            var tIdx = nameToIdxMap[to];
            return AddCondition(fIdx, tIdx, func);
        }
        public TransResult CheckCondition(int fromIdx, int toIdx)
        {
            var fromTo = new Tuple<int, int>(fromIdx, toIdx);
            var transResult = new TransResult(false);
            if (conditionFuncMap.ContainsKey(fromTo))
            {
                transResult.result = conditionFuncMap[fromTo].Invoke();
            }
            else
            {
                transResult.errorType = 1;
            }
            return transResult;
        }
        public void SetState(int toIdx, Func<BehaviourJob> enterFunc = null, Func<BehaviourJob> updateFunc = null,
            Func<BehaviourJob> exitFunc = null)
        {
            var prevIdx = curIdx;
            var prevState = states[prevIdx];
            prevState.OnExit();

            curIdx = toIdx;
            var curState = states[curIdx];
            curState.GiveJobs(enterFunc, updateFunc, exitFunc);
            onTransSuccess?.Invoke(prevState, curState);
            curState.OnEnter();
        }
        public void SetState(string toState, Func<BehaviourJob> enterFunc = null, Func<BehaviourJob> updateFunc = null,
            Func<BehaviourJob> exitFunc = null)
        {
            var toIdx = nameToIdxMap[toState];
            SetState(toIdx, enterFunc, updateFunc, exitFunc);
        }
        public bool TransState(int toIdx, Func<BehaviourJob> enterFunc = null, Func<BehaviourJob> updateFunc = null,
            Func<BehaviourJob> exitFunc = null)
        {
            var checkCondition = CheckCondition(curIdx, toIdx);
            if (!checkCondition.result)
            {
                checkCondition.errorType = 2;
                onTransFail?.Invoke(checkCondition, states[curIdx], states[toIdx]);
                return false;
            }

            SetState(toIdx, enterFunc, updateFunc, exitFunc);
            return true;
        }

        public bool TransState(string stateName, Func<BehaviourJob> enterFunc = null,
            Func<BehaviourJob> updateFunc = null,
            Func<BehaviourJob> exitFunc = null)
        {
            var toIdx = nameToIdxMap[stateName];
            var result = TransState(toIdx, enterFunc, updateFunc, exitFunc);
            return result;
        }

        public State GetState(int idx)
        {
            return states[idx];
        }

        public State GetState(string stateName)
        {
            var idx = nameToIdxMap[stateName];
            return states[idx];
        }

        public State GiveJobs(int idx, Func<BehaviourJob> enterFunc = null, Func<BehaviourJob> updateFunc = null,
            Func<BehaviourJob> exitFunc = null)
        {
            var state = states[idx];
            state.GiveJobs(enterFunc, updateFunc, exitFunc);
            return state;
        }

        public State Current => states[curIdx];
        public IEnumerator Blank() { yield return null; }
        public bool AlwaysTrue() => true;
        public bool AlwaysFalse() => false;
        
        protected virtual void Update()
        {
            if (curIdx >= states.Count)
                return;
            var curState = states[curIdx];
            if(!curState.IsUpdateRunning)
                curState.OnUpdate();
        }
    }
}
