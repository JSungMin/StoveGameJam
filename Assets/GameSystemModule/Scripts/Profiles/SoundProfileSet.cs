using System.Linq;

namespace CoreSystem
{
    public class SoundProfileSet : SingletonManagerObject<SoundProfileSet, SoundProfile>
    {
    #if UNITY_EDITOR
        public static SoundProfileSet CreateInstance()
        {
            var dataSet = CreateInstance<SoundProfileSet>();
            UnityEditor.AssetDatabase.CreateAsset(dataSet, SystemPathes.pathOfSoundProfileSet);
            UnityEditor.EditorUtility.SetDirty(dataSet);
            var profiles = PathHelper.FindAssets<SoundProfile>();
            dataSet.elements = profiles.Select(data => data).ToList();
            UnityEditor.AssetDatabase.Refresh();
            return dataSet;
        }
    #endif
    }
}
