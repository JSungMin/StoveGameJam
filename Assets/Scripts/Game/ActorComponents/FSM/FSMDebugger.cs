using System.Collections;
using System.Collections.Generic;
using CoreSystem.Game.ActorComponents;
using UnityEngine;

namespace CoreSystem.Game.ActorComponents
{
    [RequireComponent(typeof(ActorFSM))]
    public class FSMDebugger : MonoBehaviour
    {
        private GameActor traceActor;
        public ActorFSM fsm;
        public List<string> frameLogList = new List<string>();

        public string TraceActorName => traceActor.gameObject.name;
        
        // Start is called before the first frame update
        void Start()
        {
            fsm = GetComponent<ActorFSM>();
            traceActor = fsm.actor;
            fsm.onTransSuccess += OnTransSuccess;
            fsm.onTransFail += OnTransFail;
        }

        private void OnTransFail(ActorFSM.TransResult result, ActorFSM.State arg1, ActorFSM.State arg2)
        {
            var log = "";
            if (result.errorType == 1)
            {
                log = "NO COND : " + arg1.name + "->" + arg2.name;
            }
            else if (result.errorType == 2)
            {
                log = "FALSE COND : " + arg1.name + "->" + arg2.name;
            }
            frameLogList.Add(log);
        }

        private void OnTransSuccess(ActorFSM.State arg1, ActorFSM.State arg2)
        {
            var log = "SUCCESS : " + arg1.name + "->" + arg2.name;
            frameLogList.Add(log);
        }
        
        void LateUpdate()
        {
            if (frameLogList.Count <= 0) return;
            var log = "<"+TraceActorName+"_FSM_LOG>\n";
            for (var i = 0; i < frameLogList.Count; i++)
            {
                log += i + " : "+frameLogList[i] + "\n";
            }
            Debug.Log(log);
            frameLogList.Clear();
        }
    }
}
