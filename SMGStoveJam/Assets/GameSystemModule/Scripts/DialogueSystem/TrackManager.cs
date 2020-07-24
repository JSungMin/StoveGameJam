using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CoreSystem.Game.Dialogue;

namespace CoreSystem.Game.Dialogue
{
    
    public class TrackManager : MonoBehaviour, ICallListener
    {
        /*  TODO : Track TODO LIST
         * 1. 시놉시스에 있는 대사와 말풍성을 이어줘야함
         * 2. 대본에서 이 시놉시스를 읽을 수 있도록 함수 지원
         * 3. 대본에서 시놉시스 내 변수 변화를 감지 할 수 있게 이벤트 콜 지원
         */
        public enum ManagerState
        {
            SLEEP,
            RUNNING,
            WAITING,
            PAUSE,
            SKIPPING,
            DONE
        }
        private DialoguePlayer player;
        public PortraitManager portraitManager;
        public Track targetTrack;
        public ManagerState state;
        public int curScriptIdx = 0;

        public DialogueActionVM VM => targetTrack.actionVm;
        public List<VMScript> ActionScripts => targetTrack.actionScripts;

        public List<ScriptObject> stagedScripts = new List<ScriptObject>();
        public List<GameActor> StagedActors => StagedObjectManager.Instance.allGameActors;

        public RectTransform rectTransform;
        public RectTransform scriptPool;

        private BehaviourJob managerJob;


        public static TrackManager Employ(DialoguePlayer employee, Track track)
        {
            var cachedManager = employee.trackManagers.FirstOrDefault(x => x.targetTrack.trackName == track.trackName);
            if (null != cachedManager)
                return cachedManager;
            var obj = new GameObject("M_"+track.trackName);
            obj.transform.parent = employee.transform;
            var manager = obj.AddComponent<TrackManager>();
            manager.player = employee;
            manager.targetTrack = track;
            manager.state = ManagerState.SLEEP;
            manager.rectTransform = obj.AddComponent<RectTransform>();
            manager.rectTransform.localScale = Vector3.one;
            manager.rectTransform.anchorMin = Vector2.zero;
            manager.rectTransform.anchorMax = Vector2.one;
            manager.rectTransform.anchoredPosition = Vector2.zero;
            manager.rectTransform.offsetMin = Vector2.zero;
            manager.rectTransform.offsetMax = Vector2.zero;
            manager.portraitManager = PortraitManager.Employ(manager.rectTransform);

            var pool = new GameObject("ScriptPool");
            pool.transform.SetParent(manager.transform);
            manager.scriptPool = pool.AddComponent<RectTransform>();
            manager.scriptPool.localPosition = Vector3.zero;
            manager.scriptPool.localScale = Vector3.one;
            manager.scriptPool.anchorMin = Vector2.zero;
            manager.scriptPool.anchorMax = Vector3.one;
            manager.scriptPool.offsetMin = Vector3.zero;
            manager.scriptPool.offsetMax = Vector3.zero;
            return manager;
        }

        public void OnInitializeGame()
        {
            //  Initialize VM and Invoke AwakeRun
            //  VM Copied When TrackCopied
            VM.Initialize(ActionScripts, this);
            for (var i = 0; i < stagedScripts.Count; i++)
            {
                var obj = stagedScripts[i];
                obj.OnInitializeGame();
            }
        }

        public void OnNext()
        {
            VM.OnEventInvoke(this, "OnNext");
            if (curScriptIdx >= ScriptObjectCount)
            {
                if (state != ManagerState.DONE)
                    OnFinishTrack();
                return;
            }
            if (state == ManagerState.RUNNING||state == ManagerState.SKIPPING)
            {
                //  TODO : OnSkip
                OnSkip();
            }
            else if (state == ManagerState.SLEEP || state == ManagerState.WAITING)
            {
                OnPlay();
            }
        }
        public void OnPlay()
        {
            if (state == ManagerState.WAITING)
            {
                var prevObj = stagedScripts[curScriptIdx - 1];
                prevObj.Stop();
                prevObj.UnActivate();
            }
            state = ManagerState.RUNNING;
            if (curScriptIdx == 0)
            {
                OnBeginTrack();
            }
            var curObj = stagedScripts[curScriptIdx];
            curObj.Activate();
            curObj.StartPrint();
        }

        public void OnSkip()
        {
            state = ManagerState.SKIPPING;
            var curObj = stagedScripts[curScriptIdx];
            curObj.SetSkip();
            VM.OnEventInvoke(this, "OnSkip");
        }

        public void OnPause()
        {
            state = ManagerState.PAUSE;
            var curObj = stagedScripts[curScriptIdx];
            curObj.PausePrint();
            VM.OnEventInvoke(this, "OnPause");
        }

        public void OnResume()
        {
            state = ManagerState.RUNNING;
            var curObj = stagedScripts[curScriptIdx];
            curObj.ResumePrint();
            VM.OnEventInvoke(this, "OnResume");
        }
        public void OnDone()
        {
            state = ManagerState.DONE;
            var curObj = stagedScripts[ScriptObjectCount - 1];
            curObj.Stop();
        }
        public void OnPrintComplete(ScriptObject obj, bool comp)
        {
            Wait();
            ++curScriptIdx;
        }
        //  시놉시스 내용이 실행되기 시작할 때 호출
        public void OnBeginTrack()
        {
            Debug.Log("Start Track : " + targetTrack.trackName);
            VM.OnEventInvoke(this, "OnBegin");
        }
        // 시놉시스가 끝까지 진행된 후 호출
        public void OnFinishTrack()
        {
            Debug.Log("Finished Track : " + targetTrack.trackName);
            VM.OnEventInvoke(this, "OnFinish");
            OnDone();
        }

        public BehaviourJob OnCall(string methodName, List<TaggedData> parameters)
        {
            if (methodName == "PopAllPortrait")
            {
                portraitManager.RemoveAllPortrait();
            }
            return null;
        }
        //  대본 리더에서 호출 할 예정
        public void StagingTrack()
        {
            StagingScriptObjects();
        }

        public void UnStagingTrack()
        {
            UnStagingActors();
            UnStagingScriptObjects();
            UnStagingPortraitManager();
            player?.OnRemoveManager(this);
        }
        public void StagingScriptObjects()
        {
            for (var i = 0; i < targetTrack.ScriptCount; i++)
            {
                var script = targetTrack.scripts[i];
                var scriptObj = CreateScriptObject(script);
                scriptObj.transform.SetParent(scriptPool);
                scriptObj.Stage(this, script);
            }
        }

        public void UnStagingScriptObjects()
        {
            var count = stagedScripts.Count;
            for (var i = 0; i < count; i++)
            {
                var obj = stagedScripts[i].gameObject;
    #if UNITY_EDITOR
                DestroyImmediate(obj);
    #else
                Destroy(obj);
    #endif
            }
            stagedScripts.Clear();
        }

        public GameActor StagingActor(ActorProfile profile, GameActor.ActorType actorType)
        {
            if (profile == null)
                return null;
            var cachedActor = StagedActors.FirstOrDefault(x => x.Profile == profile);
            if (null != cachedActor)
                return cachedActor;
            var spawnActor = GameActor.SpawnGameActor(profile, actorType, false, true);
            return spawnActor;
        }

        public void UnStagingActor(GameActor actor)
        {
            GameActor.KillGameActor(actor);
        }
        public void UnStagingActors()
        {
            while(StagedActors.Count != 0)
            {
                var result = GameActor.KillGameActor(StagedActors[0]);
            }
        }

        public void UnStagingPortraitManager()
        {
            portraitManager?.RemoveAllPortrait();
            DestroyImmediate(portraitManager.gameObject);
        }

        private ScriptObject CreateScriptObject(BaseScript script)
        {
            var scriptObj = DialoguePlayer.Instance.CreateScriptObject(script);
            scriptObj.transform.parent = transform;
            stagedScripts.Add(scriptObj);
            return scriptObj;
        }

        private void DestroyScriptObject(ScriptObject obj)
        {
            stagedScripts.Remove(obj);
            Destroy(obj.gameObject);
        }

        public void Wait()
        {
            state = ManagerState.WAITING;
            //var curObj = stagedScripts[curScriptIdx];
            //curObj.Stop();
        }
        

        public string Name => targetTrack.trackName;
        public int ScriptObjectCount => stagedScripts.Count;
    }
}
