using System.IO;
using System.Collections;
using System.Collections.Generic;
using CoreSystem.Editor.ProfileViewer;
using UnityEngine;
using UnityEditor;

namespace CoreSystem.Editor
{
    public class JsonEditor : EditorWindow
    {
        public static JsonEditor window;
        public MonoScript targetScript;
        public System.Type targetScriptType;
        public object targetInstance;
        public string jsonData;
        public Vector2 contentScrollVector2;

        [MenuItem("Window/JsonEditor")]
        public static void ShowWindow()
        {
            window = GetWindow<JsonEditor>();
            window.titleContent = new GUIContent("Json Editor");
        }
        private void OnEnable()
        {
            window = GetWindow<JsonEditor>();
        }
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();
            targetScript = EditorGUILayout.ObjectField("Script Type", targetScript, typeof(MonoScript)) as MonoScript;
            if (EditorGUI.EndChangeCheck())
            {
                if (targetScript != null)
                    targetInstance = CreateTargetInstance();
            }

            if (targetScript != null)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Save JSON"))
                {
                    SaveTargetInstanceToJson();
                }

                if (GUILayout.Button("Load JSON"))
                {
                    LoadTargetInstanceFromJson();
                }
                EditorGUILayout.EndHorizontal();
            }

            if (null != targetInstance)
            {
                contentScrollVector2 = EditorGUILayout.BeginScrollView(contentScrollVector2);
                targetInstance = CGUI.VObjectField(targetInstance, targetScriptType);
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private object CreateTargetInstance()
        {
            targetScriptType = TypeHelper.GetType(GetClassFullName(targetScript));
            var newInstance = TypeHelper.CreateInstance(targetScriptType);
            return newInstance;
        }

        private void SaveTargetInstanceToJson()
        {
            //EditorUtility.SetEntryToDirty(jsonData);
            jsonData = JsonUtility.ToJson(targetInstance);
            var absPath = EditorUtility.OpenFilePanel("Save To JSON", "", "json");
            File.WriteAllText(absPath, jsonData);
            var debugText = "<Save JsonData>\n";
            debugText += "PATH : " + absPath + "\n";
            debugText += "JSON : " + jsonData + "\n";
            Debug.Log(debugText);
        }

        private void LoadTargetInstanceFromJson()
        {
            var absPath = EditorUtility.OpenFilePanel("Save To JSON", "", "json");
            jsonData = File.ReadAllText(absPath);
            targetInstance = JsonUtility.FromJson(jsonData, targetScriptType);
            var debugText = "<Load JsonData>\n";
            debugText += "PATH : " + absPath + "\n";
            debugText += "JSON : " + jsonData + "\n";
            Debug.Log(debugText);
        }

        private string GetClassName(MonoScript script) => script.GetClass().Name;
        private string GetClassFullName(MonoScript script) => script.GetClass().FullName;
    }
}
