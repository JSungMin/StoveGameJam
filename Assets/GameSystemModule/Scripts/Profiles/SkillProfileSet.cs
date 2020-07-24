using System.Linq;

namespace CoreSystem
{
    public class SkillProfileSet : SingletonManagerObject<SkillProfileSet,SkillProfile> {
        #if UNITY_EDITOR
        public static SkillProfileSet CreateInstance()
        {
            var dataSet = CreateInstance<SkillProfileSet>();
            UnityEditor.AssetDatabase.CreateAsset(dataSet, SystemPathes.GetSystemPath(SystemPathes.PathType.SkillProfileSet));
            UnityEditor.EditorUtility.SetDirty(dataSet);
            var localSkills = PathHelper.FindAssets<SkillProfile>();
            dataSet.elements = localSkills.Select(data => data).ToList();
            UnityEditor.AssetDatabase.Refresh();
            return dataSet;
        }
        public static void ReloadSkills()
        {
            var localSkills = PathHelper.FindAssets<SkillProfile>();
            Instance.elements = localSkills.Select(data => data).ToList();
        }
    #endif
    }
}
