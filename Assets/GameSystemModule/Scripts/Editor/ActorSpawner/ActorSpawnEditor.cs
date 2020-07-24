using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CoreSystem.Editor.ProfileViewer;
using CoreSystem.Game;

namespace CoreSystem.Editor
{
    public class ActorSpawnEditor : EditorWindow
    {
        public static ActorSpawnEditor window;
        public static Vector2 windowPoint = new Vector2(300, 300);
        public static Vector2 windowSize = new Vector2(1000, 650);

        public List<ActorProfile> actorProfiles;
        public ActorProfile selectedProfile;
        public GameActor originActor;
        public Vector2 profileListScrollView;

        public GUILayoutOption[] profileListOptions;
        public GUILayoutOption[] startBttnOptions;
        public GUILayoutOption[] statFieldOption;

        public GameActor.ActorType curActorType;

        [MenuItem("Window/ActorSpawner")]
        public static void ShowWindow()
        {
            window = GetWindowWithRect<ActorSpawnEditor>(new Rect(windowPoint, windowSize));
            window.Initialize();
        }
        private void Initialize()
        {
            window.minSize = windowSize;
            window.titleContent.text = "Actor Spawner";
            CoreDataSet.Instance = AssetDatabase.LoadAssetAtPath<CoreDataSet>(SystemPathes.GetSystemPath(SystemPathes.PathType.CoreDataSet));
            actorProfiles = ActorProfileSet.Instance.elements;

            profileListOptions = new GUILayoutOption[]
            {
                GUILayout.MaxWidth(200)
            };
            startBttnOptions = new GUILayoutOption[]
            {
                GUILayout.Width(200),
                GUILayout.Height(30)
            };
            statFieldOption = new GUILayoutOption[]
            {
                GUILayout.MaxWidth(60)
            };
        }
        private void OnSelectionChange()
        {
            // Replace Equipment
            var tmpSelected = Selection.activeObject as ActorProfile;
            selectedProfile = tmpSelected;
        }
        private void OnGUI()
        {
            selectedProfile = Selection.activeObject as ActorProfile;
            if (null == selectedProfile && ActorProfileSet.CountOfElements() != 0)
                selectedProfile = ActorProfileSet.GetElement(0);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, profileListOptions);
            bool foldout = true;
            profileListScrollView = EditorGUILayout.BeginScrollView(profileListScrollView);
            actorProfiles = CGUI.VObjectDropdown("Actor", actorProfiles, ref foldout, null, profileListOptions);
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            if (null != selectedProfile)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Name");
                var color = selectedProfile.actorNameColor;
                var hexColor = ColorUtility.ToHtmlStringRGBA(color);
                var richStyle = new GUIStyle(GUI.skin.label);
                richStyle.richText = true;
                EditorGUILayout.LabelField("<b><color=#"+hexColor +">" + selectedProfile.actorName + "</color></b>", richStyle);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                originActor = EditorGUILayout.ObjectField("Target Actor", originActor, typeof(GameActor), true) as GameActor;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                curActorType = (GameActor.ActorType)EditorGUILayout.EnumPopup(curActorType);
                bool isSaveable = curActorType == GameActor.ActorType.Player ? true : false;
                isSaveable = EditorGUILayout.Toggle("Is Saveable", isSaveable);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Description");
                EditorGUILayout.TextArea(selectedProfile.description);
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                CGUI.DynamicGaugeField("HP", selectedProfile.HP);
                GUILayout.Space(32.5f);
                CGUI.DynamicGaugeField("MP", selectedProfile.MP);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                #region Start Setting
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Start Setting");
                #region Equipment View
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Start Equipments");
                if (GUILayout.Button("Equipments", startBttnOptions))
                {
                    EquipmentViewer.ShowWindow(selectedProfile);
                }
                EditorGUILayout.EndVertical();
                #endregion
                #region Inventory View
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Start Inventory");
                if (GUILayout.Button("Inventory", startBttnOptions))
                {
                    InventoryViewer.ShowWindow(selectedProfile);
                }
                EditorGUILayout.EndVertical();
                #endregion
                #region Skill View
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Start Skills");
                if (GUILayout.Button("Skills", startBttnOptions))
                {
                    SkillViewer.ShowWindow(selectedProfile);
                }
                EditorGUILayout.EndVertical();
                //GUILayout.FlexibleSpace();
                #endregion
                EditorGUILayout.EndVertical();
                #endregion
                if (GUILayout.Button("Open Profile", GUILayout.Height(42.5f)))
                {
                    ActorEditor.ShowWindow(selectedProfile, null, null, null);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                CGUI.StatFieldUnActive("Basic", selectedProfile.basicStat, statFieldOption);
                CGUI.StatFieldUnActive("Additional", selectedProfile.additionalStat, statFieldOption);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Update", GUILayout.Width(150)))
                {
                    UpdateGameActorProfile(originActor,selectedProfile);
                }
                if (GUILayout.Button("Spawn",GUILayout.Width(150)))
                {
                    SpawnGameActor(originActor, isSaveable, true);
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void UpdateGameActorProfile(GameActor sceneActor, ActorProfile newOrigin)
        {
            if (sceneActor == null)
                return;
            sceneActor.copiedProfile = ActorProfileSet.GetCloneElement(newOrigin);
        }
        private void SpawnGameActor(GameActor targetActor, bool isSaveable, bool useClone)
        {
            GameActor.SpawnGameActor(selectedProfile, curActorType, isSaveable, useClone, targetActor?.gameObject);
        }
    }
}
