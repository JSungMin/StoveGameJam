using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CoreSystem.Game.Dialogue;

namespace CoreSystem.Editor.Dialogue
{
    public class TrackEditor : EditorWindow
    {
        public static TrackEditor window;
        public static Rect windowRect = new Rect(300, 300, 900, 800);
        public static string copyData = "";

        public Track curTrack;
        public bool isSpeakerInfoDirty = true;
        public bool isDirtyAsset = false;
        public List<ScriptWrapper> scriptWrappers = new List<ScriptWrapper>();

        public bool foldCharacter;
        public bool foldStayCharacter;
        public bool foldScripts;
        public bool foldActionDesc;
        public bool foldActionParam;

        public Vector2 scrollSpeaker;
        public Vector2 scrollStaySpeaker;
        public Vector2 scrollScripts;

        public GUILayoutOption[] synopsisParamOptions;
        public GUILayoutOption[] speakerLayoutOptions;
        public GUILayoutOption[] scriptContentOptions;

        private void OnEnable()
        {
            isSpeakerInfoDirty = true;
            scriptWrappers = new List<ScriptWrapper>();
            synopsisParamOptions = new GUILayoutOption[]
            {
                GUILayout.MinWidth(80),
                GUILayout.MaxWidth(100),
                GUILayout.Height(45)
            };
            speakerLayoutOptions = new GUILayoutOption[]
            {
                GUILayout.ExpandHeight(true)
            };
            scriptContentOptions = new GUILayoutOption[]
            {
                GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(false),
                GUILayout.Width(600),
                GUILayout.Height(100)
            };
        }
        
        private void OnMakeScript()
        {
            isDirtyAsset = true;
            var newOwn = new BaseScript();
            var newWrapper = new ScriptWrapper(newOwn);
            scriptWrappers.Add(newWrapper);
        }
        private void OnRemoveScript(int idx, ScriptWrapper sw)
        {
            isDirtyAsset = true;
            scriptWrappers.Remove(sw);
            curTrack.scripts.Remove(sw.baseScript);
        }
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            curTrack = EditorGUILayout.ObjectField("Track", curTrack, typeof(Track)) as Track;
            DrawCreateLayout();
            EditorGUILayout.EndHorizontal();
            if (null != curTrack)
            {
                DrawSynopsis();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            if (isDirtyAsset)
            {
                EditorUtility.SetDirty(curTrack);
            }
        }

        private void InitializeScriptWrappers()
        {
            var delta = scriptWrappers.Count - curTrack.scripts.Count;
            for (var i = 0; i < delta; i++)
            {
                curTrack.scripts.Add(new BaseScript());
            }
            for (var i = 0; i < -delta; i++)
            {
                scriptWrappers.Add(new ScriptWrapper(new BaseScript()));
            }
            for (var i = 0; i < curTrack.scripts.Count; i++)
            {
                if (scriptWrappers[i] == null)
                    scriptWrappers[i] = new ScriptWrapper(curTrack.scripts[i]);
                else
                    scriptWrappers[i].baseScript = curTrack.scripts[i];
            }
        }

        public void DrawCreateLayout()
        {
            if (GUILayout.Button("Create"))
            {
                var absPath = EditorUtility.SaveFilePanel("Select Path", "", "", "asset");
                var localIndex = absPath.IndexOf("Asset");
                var relativePath = absPath.Substring(localIndex);
                curTrack = Track.CreateInstance(relativePath);
            }
        }
        public void DrawSynopsis()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Track Name");
            curTrack.trackName = EditorGUILayout.DelayedTextField(curTrack.trackName);
            curTrack.trackType = (TrackType)EditorGUILayout.EnumPopup(curTrack.trackType);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(synopsisParamOptions);
            var tmp = true;
            CGUI.VObjectDropdown("Actions", curTrack.actionScripts, ref tmp,
                DrawActionScriptContent);
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
                isDirtyAsset = true;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            curTrack.typingSoundProfile = EditorGUILayout.ObjectField("Typing Sound", curTrack.typingSoundProfile, typeof(SoundProfile)) as SoundProfile;
            EditorGUILayout.EndVertical();
            DrawScriptList();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        public void DrawScriptList()
        {
            scrollScripts = EditorGUILayout.BeginScrollView(scrollScripts);
            InitializeScriptWrappers();
            CGUI.VObjectDropdown("Scripts", scriptWrappers, ref foldScripts,
                null, DrawScript,
                OnMakeScript, OnRemoveScript, null);
            EditorGUILayout.EndScrollView();
        }
        private void DrawScript(int idx, ScriptWrapper sw)
        {
            if (sw?.baseScript == null) return;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            sw.baseScript.type = (ScriptType)EditorGUILayout.EnumPopup("Type",sw.baseScript.type);
            sw.baseScript.speaker = EditorGUILayout.ObjectField("Speaker", sw.baseScript.speaker, typeof(ActorProfile)) as ActorProfile;
            EditorGUILayout.EndVertical();
            if (curTrack.trackType == TrackType.CutScene)
            {
                if (sw.baseScript.type == ScriptType.말풍선_대화)
                    sw.baseScript.type = ScriptType.컷씬_대화;
            }
            EditorGUILayout.EndHorizontal();
            sw.baseScript.content = EditorGUILayout.TextArea(sw.baseScript.content, scriptContentOptions);

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            CGUI.VObjectDropdown("Actions", sw.baseScript.actionScripts, ref sw.baseScript.foldActionScripts,
                 null,DrawActionScriptContent,null,null,DrawActionScriptHeadline);
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                isDirtyAsset = true;
            }
        }

        private void DrawActionScriptHeadline(List<VMScript> scripts)
        {
            if (GUILayout.Button("Copy"))
            {
                var wrapper = new JSONCollectionWrapper<VMScript>(scripts);
                copyData = JsonUtility.ToJson(wrapper);
            }
            if (GUILayout.Button("Paste"))
            {
                var tmp = JsonUtility.FromJson<JSONCollectionWrapper<VMScript>>(copyData);
                scripts.Clear();
                scripts.AddRange(tmp.objs);
            }
        }
        private void DrawActionScriptContent(int idx, VMScript script)
        {
            EditorGUILayout.BeginHorizontal();
            script.name = EditorGUILayout.TextField(script.name);
            if (GUILayout.Button("Edit Script"))
            {
                VMViewer.ShowWindow(script);
                isDirtyAsset = true;
            }

            if (GUILayout.Button("Copy"))
            {
                VMViewer.CopyScript(script);
            }
            if (GUILayout.Button("Paste"))
            {
                VMViewer.PasteScript(script);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            script.runOnAwake = EditorGUILayout.Toggle("Run OnAwake",script.runOnAwake);
            EditorGUILayout.EndHorizontal();
            script.isEventScript = EditorGUILayout.Toggle("Is Event", script.isEventScript);
        }

        public class JSONCollectionWrapper<T>
        {
            public List<T> objs;

            public JSONCollectionWrapper(List<T> t)
            {
                objs = t;
            }
        }
        public class ScriptWrapper
        {
            public BaseScript baseScript;
            public bool foldActionDesc = true;
            public bool foldActionParams = true;

            public ScriptWrapper(){}
            public ScriptWrapper(BaseScript s)
            {
                baseScript = s;
                baseScript.actionScripts = new List<VMScript>();
                var onPlay = new VMScript {name = "OnPlay", isEventScript = true};
                var onStop = new VMScript {name = "OnStop", isEventScript = true};
                var onPause = new VMScript { name = "OnPause", isEventScript = true };
                var onResume = new VMScript { name = "OnResume", isEventScript = true };
                baseScript.actionScripts.Add(onPlay);
                baseScript.actionScripts.Add(onStop);
                baseScript.actionScripts.Add(onPause);
                baseScript.actionScripts.Add(onResume);
                s.actionScripts = baseScript.actionScripts;
            }
        }
        [MenuItem("Window/CoreSystem/Dialogue/Track &t")]
        public static void ShowWindow()
        {
            window = GetWindowWithRect<TrackEditor>(windowRect);
            window.titleContent.text = "Track Editor";
        }

        public static void ShowWindow(Track track)
        {
            window = GetWindowWithRect<TrackEditor>(windowRect);
            window.titleContent.text = "Track Editor";
            window.curTrack = track;
        }
    }
}