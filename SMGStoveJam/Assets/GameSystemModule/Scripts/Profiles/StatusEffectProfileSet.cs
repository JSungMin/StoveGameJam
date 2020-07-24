using System.Linq;

namespace CoreSystem
{
    public class StatusEffectProfileSet : SingletonManagerObject<StatusEffectProfileSet, StatusEffectProfile>
    {
    #if UNITY_EDITOR
        public static void ReloadDesc()
        {
            var objArr = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(SystemPathes.GetSystemPath(SystemPathes.PathType.StatusEffectSet));
            for (int i = 0; i < objArr.Length; i++)
            {
                if (!Instance.elements.Contains(objArr[i]))
                {
                    if (objArr[i].GetType() == typeof(StatusEffectProfile))
                        Instance.elements.Add((StatusEffectProfile)objArr[i]);
                }
            }

        }
        public static StatusEffectProfileSet CreateInstance()
        {
            var descDataSet = CreateInstance<StatusEffectProfileSet>();
            UnityEditor.AssetDatabase.CreateAsset(
                descDataSet,
                SystemPathes.GetSystemPath(SystemPathes.PathType.StatusEffectSet)
            );
            UnityEditor.EditorUtility.SetDirty(descDataSet);
            var localProfiles = PathHelper.FindAssets<StatusEffectProfile>();
            descDataSet.elements = localProfiles.Select(data => data).ToList();
            UnityEditor.AssetDatabase.Refresh();
            return Instance as StatusEffectProfileSet;
        }
    #endif
    }
}
