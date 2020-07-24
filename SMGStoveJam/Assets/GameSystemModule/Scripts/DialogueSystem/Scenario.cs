using System.Collections.Generic;
using UnityEngine;

namespace CoreSystem.Game.Dialogue
{
    [CreateAssetMenu(menuName = "GameData/Scenario")]
    public class Scenario : ScriptableObject
    {
        public string scenarioName;
        public List<Track> trackList = new List<Track>();
        
#if UNITY_EDITOR
        public static Scenario CreateInstance(string path, string name)
        {
            var scriptData = CreateInstance<Scenario>();
            var pathOfData = PathHelper.Bind(path, name + ".asset");
            UnityEditor.AssetDatabase.CreateAsset(
                scriptData,
                pathOfData
            );
            UnityEditor.EditorUtility.SetDirty(scriptData);

            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.SaveAssets();
            return scriptData;
        }
#endif
    }
}
