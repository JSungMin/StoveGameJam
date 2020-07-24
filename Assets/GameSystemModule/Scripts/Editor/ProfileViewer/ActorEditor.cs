using UnityEditor;
using CoreSystem.ProfileComponents;
using UnityEngine;

namespace CoreSystem.Editor.ProfileViewer
{
    public partial class ActorEditor : EditorWindow
    {
        public static ActorEditor               window;
        public static CoreDataSet               coreDataSet;
        public static ActorProfile              selectedActor = null;
        public static SkillProfile              selectedSkill;
        public static StatusEffectProfile       selectedStatus;
        public static ItemProfile               selectedItem;

        //public static GUISkin curSkin = null;
        public static int viewIndex = 0;
        public static Vector2 windowPoint = new Vector2(300,300);
        public static Vector2 windowSize = new Vector2(450,650);

        public static Vector2 viewSelectScrollPos;
        public static Vector2 vSpriteScrollPos;
        public static Vector2 contentScrollPos;
        public static Vector2 hSpriteScrollPos;
        

        public static bool flagStatusEffectFoldout = true;
        public static bool flagShowDefaultSkills = true;
        public static bool flagShowPortraitList = true;
        public static GUILayoutOption[] viewSelectScrollOption;
        public static GUILayoutOption[] spriteScrollViewOption;
        public static GUILayoutOption[] spriteBoxOption;
        public static GUILayoutOption[] contentAreaOption;
        public static GUILayoutOption[] statFieldOption;
        public static GUILayoutOption[] statFieldAreaOption;
        public static GUILayoutOption[] infulenceCurveOption;

        #region Editor Color Define
        public static Color purple = new Color(112f / 255f, 64f / 255f, 79f / 255f);
        public static Color ivory = new Color(194f / 255f, 175f / 255f, 153f / 255f);
        public static Color darkGray = new Color(89f / 255f, 82f / 255f, 82f / 255f);
        
        #endregion
        private static readonly CDelegates.VoidFunc[] showViewsFuncs = new CDelegates.VoidFunc[4];

        [MenuItem("Window/CoreSystem/Profile")]
        public static void ShowWindow()
        {
            window = GetWindowWithRect<ActorEditor>(new Rect(windowPoint, windowSize));
            window.minSize = windowSize;
            window.titleContent.text = "ProfileEditor";
            LinkCoreDataSet();
        }

        public static void ShowWindow(ActorProfile actor, ItemProfile item, SkillProfile skill, StatusEffectProfile status)
        {
            window = GetWindowWithRect<ActorEditor>(new Rect(windowPoint, windowSize));
            window.minSize = windowSize;
            window.titleContent.text = "ProfileEditor";
            LinkCoreDataSet();
            selectedActor = actor;
            selectedItem = item;
            selectedSkill = skill;
            selectedStatus = status;
        }
        private void OnEnable()
        {
            window = GetWindowWithRect<ActorEditor>(new Rect(windowPoint, windowSize));
            window.minSize = windowSize;

            SystemPathes.Instance = AssetDatabase.LoadAssetAtPath<SystemPathes>(SystemPathes.pathOfPathes);
            
            showViewsFuncs[0] = OnActorView;
            showViewsFuncs[1] = OnSkillView;
            showViewsFuncs[2] = OnStatusEffectView;
            showViewsFuncs[3] = OnItemView;
            InitializeActorView();
            viewSelectScrollOption = new GUILayoutOption[]
            {
                GUILayout.Width(windowSize.x),
                GUILayout.Height(40)
            };
            spriteScrollViewOption = new GUILayoutOption[]
            {
                GUILayout.Width(250),  
                GUILayout.Height(250 + EditorGUIUtility.singleLineHeight + 2),  //  2 is margin top + bottom
            };
            spriteBoxOption = new GUILayoutOption[]
            {
                GUILayout.Width(225),
                GUILayout.Height(225)
            };
            statFieldOption = new GUILayoutOption[]
            {
                GUILayout.MaxWidth(60)
            };
            statFieldAreaOption = new GUILayoutOption[]
            {
                GUILayout.MaxWidth(windowSize.x),
                GUILayout.MinHeight(10)
            };
            infulenceCurveOption = new GUILayoutOption[]
            {
                GUILayout.Height(100),
                GUILayout.Width(100)
            };
        }

        private static void LinkCoreDataSet(CoreDataSet core = null)
        {
            if (core == null)
            {
                var pathes = SystemPathes.Instance;
                coreDataSet = CoreDataSet.Instance =
                    AssetDatabase.LoadAssetAtPath<CoreDataSet>(
                        SystemPathes.GetSystemPath(SystemPathes.PathType.CoreDataSet));
            }
            else
                coreDataSet = CoreDataSet.Instance = core;
        }
        private void OnGUI()
        {
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
            coreDataSet = (CoreDataSet)EditorGUILayout.ObjectField(coreDataSet, typeof(CoreDataSet));
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.
            if (null == coreDataSet)
            {
                if (GUILayout.Button("Link CoreDataSet"))
                {
                    LinkCoreDataSet();
                }
                if (GUILayout.Button("Create CoreDataSet"))
                {
                    SystemPathes.CreateInstance();
                    ActorProfileSet.CreateInstance();
                    SkillProfileSet.CreateInstance();
                    StatusEffectProfileSet.CreateInstance();
                    ItemProfileSet.CreateInstance();
                    BlueprintProfileSet.CreateInstance();
                    SoundProfileSet.CreateInstance();
                    coreDataSet = CoreDataSet.CreateInstance();
                    EditorUtility.SetDirty(coreDataSet);
                    MetaStatData.CreateMetaData();
                    MetaEquipPartData.CreateMetaData();
                    coreDataSet.systemPathes = SystemPathes.Instance as SystemPathes;
                    coreDataSet.profileSet = ActorProfileSet.Instance as ActorProfileSet;
                    coreDataSet.skillSet = SkillProfileSet.Instance as SkillProfileSet;
                    coreDataSet.statusEffectSet = StatusEffectProfileSet.Instance as StatusEffectProfileSet;
                    coreDataSet.itemSet = ItemProfileSet.Instance;
                    coreDataSet.blueprintProfileSet = BlueprintProfileSet.Instance;
                    coreDataSet.soundProfileSet = SoundProfileSet.Instance;
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
                return;
            }
            
            var bgRect = EditorGUILayout.BeginVertical();
            //  TODO : Make Profile, Skill, Item View Page
            #region View Select Area
            viewSelectScrollPos = EditorGUILayout.BeginScrollView(viewSelectScrollPos, viewSelectScrollOption);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Actor", GUILayout.Width(150)))
            {
                //EditorResourceSet.ApplyCSkin("ProfileView", bgRect);
                viewIndex = 0;
            }
            if (GUILayout.Button("Skill", GUILayout.Width(150)))
            {
                viewIndex = 1;
                //EditorResourceSet.ApplyCSkin("SkillView", bgRect);
            }
            if (GUILayout.Button("Status", GUILayout.Width(150)))
            {
                viewIndex = 2;

            }
            if (GUILayout.Button("Item", GUILayout.Width(150)))
            {
                viewIndex = 3;

            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            #endregion

            #region View Rect
            var viewRect = EditorGUILayout.BeginVertical();
            showViewsFuncs[viewIndex]?.Invoke();
            EditorGUILayout.EndVertical();
            #endregion
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }
    }
}
