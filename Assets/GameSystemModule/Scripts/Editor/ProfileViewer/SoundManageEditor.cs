using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

namespace CoreSystem.Editor.ProfileViewer
{
    public class SoundManageEditor : EditorWindow
    {
        public static SoundManageEditor window;
        public static Vector2 windowSize = new Vector2(880, 650);
        public static SoundProfile selectedObject;
        public GUILayoutOption[] selectScrollViewOption;
        public GUILayoutOption[] profileViewOption;
        public GUILayoutOption[] objectViewOption;
        public Vector2 selectScroll;

        [MenuItem("Window/CoreSystem/SoundManager")]
        public static void ShowWindow()
        {
            window = EditorWindow.GetWindow<SoundManageEditor>();
            window.minSize = windowSize;
            window.titleContent.text = "SoundManager";
        }
        private void OnEnable()
        {
            selectScrollViewOption = new GUILayoutOption[]
            {
                GUILayout.MinWidth(330)
            };
            profileViewOption = new GUILayoutOption[]
            {
                GUILayout.MinWidth(150),
                GUILayout.ExpandWidth(true)
            };
            objectViewOption = new GUILayoutOption[]
            {
                GUILayout.MinWidth(500)
            };
        }

        private void OnGUI()
        {
            selectedObject = Selection.activeObject as SoundProfile;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(selectScrollViewOption);
            if (selectedObject == null && SoundProfileSet.CountOfElements() != 0)
            {
                selectedObject = SoundProfileSet.GetElement(0);
            }
            
            if (GUILayout.Button("Create SoundProfile"))
            {
                selectedObject = SoundProfile.CreateInstance();
                SoundProfileSet.AddElement(selectedObject);
            }
            if (GUILayout.Button("Remove SoundProfile") && selectedObject)
            {
                SoundProfileSet.RemoveElement(selectedObject);
            }
            selectScroll = EditorGUILayout.BeginScrollView(selectScroll);
            bool foldout = true;
            CGUI.VObjectDropdown(
                "Sound Profiles",
                Profiles, 
                ref foldout,
                SoundProfileSet.RemoveElement,
                profileViewOption
            );
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, objectViewOption);
            if (null != selectedObject)
            {
                EditorGUI.BeginChangeCheck();
                selectedObject.profileName = EditorGUILayout.DelayedTextField("Name",selectedObject.profileName);
                if (EditorGUI.EndChangeCheck())
                {
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedObject), "Sound_" + selectedObject.profileName);
                }
                selectedObject.clip = EditorGUILayout.ObjectField("Clip", selectedObject.clip, typeof(AudioClip)) as AudioClip;
                selectedObject.mixerGroup = EditorGUILayout.ObjectField("Mixer", selectedObject.mixerGroup, typeof(AudioMixerGroup)) as AudioMixerGroup;
                selectedObject.volume = EditorGUILayout.DelayedFloatField("Volume", selectedObject.volume);
                selectedObject.pitch = EditorGUILayout.DelayedFloatField("Pitch", selectedObject.pitch);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

        }
        
        public List<SoundProfile> Profiles
        {
            get
            {
                return SoundProfileSet.Instance.elements;
            }
            set
            {
                SoundProfileSet.Instance.elements = value;
            }
        }
    }
}
