using System.Linq;

namespace CoreSystem
{
    //  This Object is made with creating CoreDataSet
    public class ActorProfileSet : SingletonManagerObject<ActorProfileSet, ActorProfile>
    {
        public static ActorProfile GetElement(string actorName)
        {
            return Instance.elements.Where(x => x.actorName == actorName).FirstOrDefault();
        }

    #if UNITY_EDITOR
        public static void ReloadElements()
        {
            var objArr = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(SystemPathes.GetSystemPath(SystemPathes.PathType.ActorProfileSet));
            for (int i = 0; i < objArr.Length;i++)
            {
                if (!Instance.elements.Contains(objArr[i]))
                {
                    if (objArr[i].GetType() == typeof(ActorProfile))
                        Instance.elements.Add((ActorProfile)objArr[i]);
                }
            }
            
        }
        public static ActorProfileSet CreateInstance()
        {
            var profileDataSet = CreateInstance<ActorProfileSet>();
            UnityEditor.AssetDatabase.CreateAsset(
                profileDataSet,
                SystemPathes.GetSystemPath(SystemPathes.PathType.ActorProfileSet)
            );
            UnityEditor.EditorUtility.SetDirty(profileDataSet);
            var localProfiles = PathHelper.FindAssets<ActorProfile>();
            profileDataSet.elements = localProfiles.Select(data => data).ToList();
            UnityEditor.AssetDatabase.Refresh();
            return Instance as ActorProfileSet;
        }
    #endif
    }
}
