using CoreSystem.Game.Skill;
using CoreSystem.ProfileComponents;
using UnityEngine;
using UnityEditor;

namespace CoreSystem.Editor.ProfileViewer
{
    public class SkillViewer : EditorWindow
    {
        public static SkillViewer window;
        public static ActorProfile selectedActor;
        public SkillProfile selectedProfile;

        public Vector2 skillListScrollView;

        public GUILayoutOption[] skillListOptions;
        public GUILayoutOption[] gridElementOptions;
        public GUILayoutOption[] skillIconOptions;
        public GUILayoutOption[] skillDescOptions;

        public static void ShowWindow(ActorProfile actor)
        {
            selectedActor = actor;
            window = GetWindow<SkillViewer>();
            window.titleContent.text = actor.actorName + "Learned Skills";
        }
        private void OnEnable()
        {
            skillListOptions = new GUILayoutOption[]
            {
                GUILayout.MaxWidth(200)
            };
            gridElementOptions = new GUILayoutOption[]
            {
                GUILayout.Width(200),
                GUILayout.Height(150)
            };
            skillIconOptions = new GUILayoutOption[]
            {
                GUILayout.Width(50),
                GUILayout.Height(50)
            };
            skillDescOptions = new GUILayoutOption[]
            {
                GUILayout.Width(200),
                GUILayout.Height(50)
            };
        }
        private void OnSelectionChange()
        {
            // Replace Equipment
            var tmpSelected = Selection.activeObject as SkillProfile;
            selectedProfile = tmpSelected;
            var cache = View.Find(x => x == tmpSelected);
            if (null != cache) return;
            View.AddElement(selectedProfile);
            EditorUtility.SetDirty(selectedActor);
            AssetDatabase.SaveAssets();
        }
        private void OnGUI()
        {
            selectedProfile = Selection.activeObject as SkillProfile;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, skillListOptions);
            bool foldout = true;
            skillListScrollView = EditorGUILayout.BeginScrollView(skillListScrollView);
            if (SkillProfileSet.CountOfElements() != 0)
            {
                SkillProfileSet.Instance.elements = CGUI.VObjectDropdown("Skills", SkillProfileSet.Instance.elements, ref foldout, null, skillListOptions);
            }
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            View.Elements = CGUI.ObjectGridField(0, 3, View.Elements, DrawViewElement, null);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        private void DrawViewElement(SkillProfile profile, int idx = -1)
        {
            if (null == profile) return;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, gridElementOptions);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            CGUI.SpriteField("Icon", profile.skillIcon, skillIconOptions);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(profile.skillName);
            EditorGUILayout.LabelField(profile.skillType.ToString());
            GUILayout.FlexibleSpace();
            EditorGUILayout.ObjectField(profile, typeof(ItemProfile));
            EditorGUILayout.LabelField("Description");
            if (profile.description == "")
                EditorGUILayout.LabelField("None Description");
            else
                EditorGUILayout.LabelField(profile.description, skillDescOptions);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        public SkillView View
        {
            get { return selectedActor.skills; }
            set { selectedActor.skills = value; }
        }
    }
}
