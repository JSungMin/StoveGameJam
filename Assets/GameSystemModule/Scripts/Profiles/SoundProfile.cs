using UnityEngine;
using UnityEngine.Audio;

namespace CoreSystem
{
    public class SoundProfile : ScriptableObject {
        public string profileName;
        public AudioClip clip;
        public float volume = 1;
        public float pitch = 4;
        public AudioMixerGroup mixerGroup;

    #if UNITY_EDITOR
        public static SoundProfile CreateInstance()
        {
            var profile = CreateInstance<SoundProfile>();
            var pathOfSounds = SystemPathes.pathOfSoundProfileSet;
            var parentPath = PathHelper.GetParentPath(pathOfSounds);
            var soundCount = SoundProfileSet.CountOfElements();
            UnityEditor.AssetDatabase.CreateAsset(
                profile,
                PathHelper.Bind(parentPath, "NewSFX_"+soundCount+".asset")
            );
            UnityEditor.EditorUtility.SetDirty(profile);
            UnityEditor.AssetDatabase.SaveAssets();
            return profile;
        }
    #endif
    }
}
