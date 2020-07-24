using UnityEditor;
using System.Collections.Generic;
using CoreSystem.ProfileComponents;
using UnityEngine;

namespace CoreSystem.Editor.ProfileViewer
{
    public class StatMetaDataEditor : EditorWindow
    {
        public static StatMetaDataEditor window;
        public static VariableFloatArray statElements;
        public static Vector2 windowPoint = new Vector2(300, 300);
        public static Vector2 windowSize = new Vector2(450, 650);

        public static Vector2 scrollElements;

        public static bool flagElementsFoldOut = true;

        [MenuItem("Window/CoreSystem/StatMetaData")]
        public static void ShowWindow()
        {
            window = EditorWindow.GetWindowWithRect<StatMetaDataEditor>(new Rect(windowPoint, windowSize));
            window.minSize = windowSize;
            window.titleContent.text = "StatMetaData";
        }
        private void OnEnable()
        {
            statElements = CoreDataSet.Instance?.metaStatData;
        }
        private void OnGUI()
        {
            if (statElements == null)
            {
                EditorGUILayout.LabelField("Please Create CoreDataSet From ActorEditor");
                if (GUILayout.Button("Link CoreDataSet"))
                {
                    var pathes = SystemPathes.Instance;
                    CoreDataSet.Instance = AssetDatabase.LoadAssetAtPath<CoreDataSet>(SystemPathes.GetSystemPath(SystemPathes.PathType.CoreDataSet));
                }
                return;
            }
            var bgRect = EditorGUILayout.BeginVertical();
            EditorResourceSet.ApplyCSkin("MetaStatView", bgRect);
            scrollElements = EditorGUILayout.BeginScrollView(scrollElements);
            var prevCount = statElements.initialValue.Count;
            statElements.initialValue = CGUI.VObjectDropdown(
                "Stat Meta Data",
                statElements.initialValue,
                ref flagElementsFoldOut
            );
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            var curCount = statElements.initialValue.Count;
            if (prevCount != curCount)
            {
                foreach (var s in GetAllStats)
                {
                    var delta = curCount - s.Count;
                    if (delta < 0)
                    {
                        s.elements.RemoveRange(curCount - 1, -delta);
                    }
                    else if (delta > 0)
                    {
                        for (int c = 0; c < delta; c++)
                            s.elements.Add(0f);
                    }
                }
                EditorUtility.SetDirty(CoreDataSet.Instance.metaStatData);
                AssetDatabase.SaveAssets();
            }
        }
        private void UpdateAllStatData()
        {
            var profiles = CoreDataSet.Instance.profileSet.elements;
        }
        public Stat[] GetAllStats
        {
            get
            {
                var results = new List<Stat>();
                results.AddRange(ActorBasicStats);
                results.AddRange(ActorAdditionalStats);
                results.AddRange(ItemInfluenceStats);
                results.AddRange(StatusEffectInfluenceStats);
                return results.ToArray();
            }
        }
        private Stat[] ActorBasicStats{
            get
            {
                var basicStats = new List<Stat>();
                for (int i = 0; i < CoreDataSet.Instance.profileSet.elements.Count; i++)
                {
                    var x = CoreDataSet.Instance.profileSet.elements[i];
                    basicStats.Add(x.basicStat);
                }
                return basicStats.ToArray();
            }
        }
        private Stat[] ActorAdditionalStats
        {
            get
            {
                var addStats = new List<Stat>();
                for (int i = 0; i < CoreDataSet.Instance.profileSet.elements.Count; i++)
                {
                    var x = CoreDataSet.Instance.profileSet.elements[i];
                    addStats.Add(x.additionalStat);
                }
                return addStats.ToArray();
            }
        }
        private Stat[] ItemInfluenceStats
        {
            get
            {
                var basicStats = new List<Stat>();
                for (int i = 0; i < CoreDataSet.Instance.itemSet.elements.Count; i++)
                {
                    var x = CoreDataSet.Instance.itemSet.elements[i];
                    basicStats.Add(x.influence);
                }
                return basicStats.ToArray();
            }
        }
        private Stat[] StatusEffectInfluenceStats
        {
            get
            {
                var basicStats = new List<Stat>();
                for (int i = 0; i < CoreDataSet.Instance.statusEffectSet.elements.Count; i++)
                {
                    var x = CoreDataSet.Instance.statusEffectSet.elements[i];
                    basicStats.Add(x.influence);
                }
                return basicStats.ToArray();
            }
        }
    }
}