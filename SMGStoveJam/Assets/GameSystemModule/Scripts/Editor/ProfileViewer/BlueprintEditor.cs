using CoreSystem.ProfileComponents;
using UnityEngine;
using UnityEditor;

namespace CoreSystem.Editor.ProfileViewer
{
    public class BlueprintEditor : EditorWindow
    {
        public static BlueprintEditor window;

        public static Vector2 windowPoint = new Vector2(300, 300);
        public static Vector2 windowSize = new Vector2(450, 650);

        public static Vector2 elementScrollView;

        public static bool haveCoreData = false;
        public static BlueprintProfile selectedBlueprint;
        public static bool flagFoldout = true;

        [MenuItem("Window/CoreSystem/Blueprint")]
        public static void ShowWindow()
        {
            window = EditorWindow.GetWindowWithRect<BlueprintEditor>(new Rect(windowPoint, windowSize));
            window.minSize = windowSize;
            window.titleContent.text = "BlueprintEditor";
        }
        private void OnEnable()
        {
            haveCoreData = (CoreDataSet.Instance == null) ? false : true;
            if (null == selectedBlueprint && haveCoreData)
            {
                selectedBlueprint = BlueprintProfileSet.GetElement(0);
            }
        }
        private void OnGUI()
        {
            if (!haveCoreData)
            {
                EditorGUILayout.LabelField("Please Create CoreData");
                return;
            }
          
            #region SCD Area
            EditorGUILayout.BeginHorizontal();

            selectedBlueprint = (BlueprintProfile)EditorGUILayout.ObjectField(selectedBlueprint, typeof(BlueprintProfile));
            if (GUILayout.Button("Create Blueprint"))
            {
                selectedBlueprint = BlueprintProfile.CreateInstance();
                BlueprintProfileSet.AddElement(selectedBlueprint);
                //var newBlueprintItem = 
            }
            if (GUILayout.Button("Remove Blueprint") && selectedBlueprint)
            {
                BlueprintProfileSet.RemoveElement(selectedBlueprint);
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            if (null == selectedBlueprint)
                return;
            EditorGUILayout.LabelField("Target Item");
            selectedBlueprint.targetItem = (ItemProfile)EditorGUILayout.ObjectField(selectedBlueprint.targetItem, typeof(ItemProfile));
            elementScrollView = EditorGUILayout.BeginScrollView(elementScrollView);
            CGUI.VObjectDropdown<ItemAmountWrapper>(
                "Stuffes", 
                selectedBlueprint.stuffList,
                ref flagFoldout
            );
            EditorGUILayout.EndScrollView();
        }
    }
}
