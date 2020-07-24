using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CoreSystem.Game.Dialogue
{
    public enum TrackType
    {
        Game,
        CutScene
    }
    public class Track : ScriptableObject
    {
        public string trackName;
        public TrackType trackType;
        public SoundProfile typingSoundProfile;
        public DialogueActionVM actionVm;
        public List<VMScript> actionScripts = new List<VMScript>();

        public List<BaseScript> scripts = new List<BaseScript>();
        
        public static Track GetClone(Track origin)
        {
            var clone = Instantiate(origin);
            Type cloneType = origin.GetType();
            FieldInfo[] fields = cloneType.GetFields();
            for (var i = 0; i < fields.Length; i++)
            {
                var fieldType = fields[i].FieldType;
                var fieldVal = fields[i].GetValue(clone);
                bool isUO = TypeHelper.HaveBaseType(fieldType, typeof(UnityEngine.Object));
                if (isUO)
                {
                    if (fieldVal == null)
                    {
                        var cloneField = CreateInstance(fieldType);
                        fields[i].SetValue(clone, cloneField);
                    }
                    else
                    {
                        var cloneField = Instantiate((UnityEngine.Object)fieldVal);
                        fields[i].SetValue(clone, cloneField);
                    }
                }
            }
            return clone;
        }

        public int ScriptCount => scripts.Count;
        
#if UNITY_EDITOR
        public static Track CreateInstance(string path)
        {
            var scriptData = CreateInstance<Track>();
            UnityEditor.AssetDatabase.CreateAsset(
                scriptData,
                path
            );
            UnityEditor.EditorUtility.SetDirty(scriptData);

            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.SaveAssets();
            return scriptData;
        }
#endif
    }
}
