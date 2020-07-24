using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreSystem.Game.Dialogue;
using UnityEngine;

namespace CoreSystem.Game.Dialogue
{
    public class DialoguePlayer : SingletonGameObject<DialoguePlayer>
    {
        [SerializeField]
        private Track[] tracks;
        [SerializeField]
        private GameObject[] prefOfScripts;

        public Scenario curScenario;
        public TrackManager curTrackManager;
        public List<TrackManager> trackManagers;

        public void OnRemoveManager(TrackManager manager)
        {
            trackManagers.Remove(manager);
    #if UNITY_EDITOR
            DestroyImmediate(manager.gameObject);
    #else
            Destroy(trackManager.gameObject);
    #endif
        }

        public TrackManager GetManager(string trackName)
        {
            return trackManagers.FirstOrDefault(x => x.Name == trackName);
        }
        /* ---------------- How To Use Player
         * 1. PlayTrack : 수행 할 트랙을 선택한다.
         * 2. Next : 특정 입력을 통해 해당 트랙을 진행한다.
         * 3. PauseTrack : 현재 재생중인 혹은 파라미터에 해당하는 트랙을 멈춘다.
         * 4. StopTrack : 
         */
        public void PlayTrack(string trackName)
        {
            var manager = GetManager(trackName);
            if (null == manager)
                return;
            if (manager.state == TrackManager.ManagerState.PAUSE)
                manager.OnResume();
            else
                manager.OnPlay();
            curTrackManager = manager;
        }
        public void PauseTrack(string trackName)
        {
            var manager = GetManager(trackName);
            manager?.OnPause();
        }
        public void StopTrack(string trackName)
        {
            var manager = GetManager(trackName);
            manager?.OnDone();
        }
        public void Next()
        {
            curTrackManager?.OnNext();
        }
        //  Initialize Funcs
        public void StagingScenario(Scenario scenario)
        {
            //  Copy Tracks(Synopses) in Scenario
            tracks = ClonningTracks(scenario);
            trackManagers = new List<TrackManager>(CountOfSynopsis);
            //  Staging Tracks(Synopses)
            for (var i = 0; i < CountOfSynopsis; i++)
            {
                var manager = TrackManager.Employ(this, tracks[i]);
                manager.StagingTrack();
                trackManagers.Add(manager);
            }
        }
        public void UnStagingScenario()
        {
            for (var i = 0; i < trackManagers.Count; i++)
            {
                var manager = trackManagers[i];
                manager.UnStagingTrack();
            }
        }

        public ScriptObject CreateScriptObject(BaseScript script)
        {
            var scriptObj = Instantiate(prefOfScripts[(int) script.type]).GetComponent<ScriptObject>();
            scriptObj.script = script;
            return scriptObj;
        }
        private Track[] ClonningTracks(Scenario scenario)
        {
            var result = new Track[CountOfSynopsis];
            for (var i = 0; i < CountOfSynopsis; i++)
            {
                result[i] = Track.GetClone(scenario.trackList[i]);
            }

            return result;
        }

        public void Awake()
        {
            for (var i = 0; i < trackManagers.Count; i++)
            {
                var m = trackManagers[i];
                m.OnInitializeGame();
            }
            //StagingScenario(curScenario);
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)||Input.GetMouseButtonDown(0))
                Next();
        }

        public int CountOfSynopsis => curScenario.trackList.Count;
        public string ScenarioName => curScenario?.scenarioName;
    }
}
