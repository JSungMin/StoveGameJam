using System.Collections.Generic;
using CoreSystem.Game.Dialogue;
using UnityEngine;
using UnityEditor;

namespace CoreSystem.Editor.Dialogue
{
    public class ScenarioEditor : EditorWindow
    {
        public static EditorWindow window;
        public static Rect windowRect = new Rect(100, 300, 1000, 800);
        public Scenario scenario;
        public DialoguePlayer player;
        public List<TrackWrapper> wrappers = new List<TrackWrapper>();
        public bool wrapFold = true;

        private void OnEnable()
        {
            if (null != scenario)
                wrappers = WrapTracks(ref scenario.trackList);
        }

        private List<TrackWrapper> WrapTracks(ref List<Track> tracks)
        {
            var trackWrappers = new List<TrackWrapper>();
            for (var i = 0; i < tracks.Count; i++)
            {
                var wrapper = new TrackWrapper(tracks[i]);
                trackWrappers.Add(wrapper);
            }
            return trackWrappers;
        }

        private int FindWrapper(List<TrackWrapper> collections, TrackWrapper w)
        {
            for (var c = 0; c < collections.Count; c++)
            {
                if (collections[c] == w)
                    return c;
            }

            return -1;
        }
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            scenario = EditorGUILayout.ObjectField("Scenario", scenario, typeof(Scenario)) as Scenario;
            if (EditorGUI.EndChangeCheck())
            {
                if (null != scenario)
                    wrappers = WrapTracks(ref scenario.trackList);
            }
            DrawEditorToolLayout();
            EditorGUILayout.EndHorizontal();
            if (null != scenario)
            {
                DrawScenario();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        private void OnMakeWrapper()
        {
            var wrapper = new TrackWrapper(null);
            wrappers.Add(wrapper);
            scenario.trackList.Add(null);
        }
        private void OnRemoveWrapper(int idx, TrackWrapper wrapper)
        {
            scenario.trackList.Remove(wrapper.track);
            wrappers.Remove(wrapper);
        }

        public void DrawEditorToolLayout()
        {
            if (GUILayout.Button("Create"))
            {
                var absPath = EditorUtility.SaveFilePanel("Select Path", "", "", "asset");
                var localIndex = absPath.IndexOf("Asset");
                var relativePath = absPath.Substring(localIndex);
                scenario = Scenario.CreateInstance(relativePath, "NewScenario");
            }

            if (GUILayout.Button("Staging"))
            {
                if (scenario == null)
                    return;
                player = FindObjectOfType<DialoguePlayer>();
                player?.StagingScenario(scenario);
            }

            if (GUILayout.Button("UnStaging"))
            {
                if (scenario == null)
                    return;
                player = FindObjectOfType<DialoguePlayer>();
                player?.UnStagingScenario();
            }
        }

        public void DrawScenario()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scenario Name", scenario.scenarioName);
            scenario.scenarioName = EditorGUILayout.DelayedTextField(scenario.scenarioName);
            EditorGUILayout.EndHorizontal();
            var count = wrappers.Count;
            CGUI.VObjectDropdown("Tracks", wrappers, ref wrapFold, null,DrawTrackWrapper, OnMakeWrapper, OnRemoveWrapper);
            if (count != wrappers.Count)
            {
                EditorUtility.SetDirty(scenario);
            }
        }

        public void DrawTrackWrapper(int idx, TrackWrapper wrapper)
        {
            EditorGUI.BeginChangeCheck();
            wrapper.track = EditorGUILayout.ObjectField("Track", wrapper.track, typeof(Track)) as Track;
            if (EditorGUI.EndChangeCheck())
            {
                scenario.trackList[idx] = wrapper.track;
            }
            if (null == wrapper.track)
                return;
            EditorGUILayout.BeginHorizontal();
            wrapper.track.trackType = (TrackType)EditorGUILayout.EnumPopup(wrapper.track.trackType);
            EditorGUILayout.LabelField("Name : " + wrapper.track.trackName);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Staging"))
            {
                player = FindObjectOfType<DialoguePlayer>();
                var manager = player?.GetManager(wrapper.track.trackName);
                if (null != manager)
                    manager.StagingTrack();
            }
            if (GUILayout.Button("UnStaging"))
            {
                player = FindObjectOfType<DialoguePlayer>();
                var manager = player?.GetManager(wrapper.track.trackName);
                if (null != manager)
                    manager.UnStagingTrack();
            }
            if (GUILayout.Button("Update"))
            {
                player = FindObjectOfType<DialoguePlayer>();
                var manager = player?.GetManager(wrapper.track.trackName);
                if (null != manager)
                {
                    manager.UnStagingScriptObjects();
                    manager.StagingScriptObjects();
                }
            }
            if (GUILayout.Button("Edit"))
            {
                TrackEditor.ShowWindow(wrapper.track);
            }
            EditorGUILayout.EndHorizontal();

        }

        [MenuItem("Window/CoreSystem/Dialogue/Scenario &s")]
        public static void ShowWindow()
        {
            window = GetWindowWithRect<ScenarioEditor>(windowRect);
            window.titleContent.text = "Scenario Editor";
        }

        public class TrackWrapper
        {
            public Track track;
            public bool foldInfo = true;

            public TrackWrapper() {}

            public TrackWrapper(Track t)
            {
                track = t;
            }
        }
    }
}
