using System.Linq;

namespace CoreSystem
{
    public class ItemProfileSet : SingletonManagerObject<ItemProfileSet,ItemProfile> {

    #if UNITY_EDITOR
            public static ItemProfileSet CreateInstance()
            {
                var dataSet = CreateInstance<ItemProfileSet>();
                UnityEditor.AssetDatabase.CreateAsset(dataSet, SystemPathes.GetSystemPath(SystemPathes.PathType.ItemProfileSet));
                UnityEditor.EditorUtility.SetDirty(dataSet);
                var mItems = PathHelper.FindAssets<ItemProfile>();
                dataSet.elements = mItems.Select(data => data).ToList();
                UnityEditor.AssetDatabase.Refresh();
                return dataSet;
            }
            public static void ReloadItems()
            {
                var mItems = PathHelper.FindAssets<ItemProfile>();
                Instance.elements = mItems.Select(data => data).ToList();
            }
    #endif
    }
}
