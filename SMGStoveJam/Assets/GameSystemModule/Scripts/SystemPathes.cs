using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemPathes : SingletonScriptableObject<SystemPathes>
{
#if UNITY_EDITOR
    //  Define Default Pathes
    public static string pathOfPathes = "Assets/Resources/JSModule/Pathes/SystemPathes.asset";
    public static string pathOfCoreDataSet = "Assets/Resources/JSModule/CoreData/CoreDataSet.asset";
    public static string pathOfProfiles = "Assets/Resources/JSModule/CoreData/Profiles/ActorProfileSet.asset";
    public static string pathOfEditorResourceSet = "Assets/Resources/JSModule/EditorResources/EditorResourceSet.asset";
    public static string pathOfSkillDataSet = "Assets/Resources/JSModule/CoreData/Skills/SkillDataSet.asset";
    public static string pathOfStatusEffectSet = "Assets/Resources/JSModule/CoreData/StatusEffects/StatusEffectSet.asset";
    public static string pathOfItemDataSet = "Assets/Resources/JSModule/CoreData/Items/ItemDataSet.asset";
    public static string pathOfMetaStatData = "Assets/Resources/JSModule/CoreData/ScriptableVariable/MetaStatData.asset";
    public static string pathOfBlueprintSet = "Assets/Resources/JSModule/CoreData/Blueprints/BlueprintSet.asset";
    public static string pathOfSoundProfileSet = "Assets/Resources/JSModule/CoreData/Sounds/SoundProfileSet.asset";
    public static string pathOfMetaEquipPartData = "Assets/Resources/JSModule/CoreData/ScriptableVariable/MetaEquipPartData.asset";

    public enum PathType
    {
        Pathes = 0,
        CoreDataSet,
        ActorProfileSet,
        EditorResourceSet,
        SkillProfileSet,
        StatusEffectSet,
        ItemProfileSet,
        MetaStatData,
        BlueprintSet,
        SoundProfileSet,
        MetaEquipPartData
    }

    public List<string> pathes = new List<string>(EnumHelper.CountOfElement<PathType>());

    public static string GetSystemPath(PathType pathType)
    {
        return Instance.pathes[(int)pathType];
    }
    public static SystemPathes CreateInstance()
    {
        if (null != SystemPathes.Instance)
        {
            Debug.LogWarning("Already Have SystemPath Asset");
            return _instance;
        }
        PathHelper.MakeSurePath(pathOfPathes);
        PathHelper.MakeSurePath(pathOfCoreDataSet);
        PathHelper.MakeSurePath(pathOfProfiles);
        PathHelper.MakeSurePath(pathOfEditorResourceSet);
        PathHelper.MakeSurePath(pathOfSkillDataSet);
        PathHelper.MakeSurePath(pathOfStatusEffectSet);
        PathHelper.MakeSurePath(pathOfItemDataSet);
        PathHelper.MakeSurePath(pathOfMetaStatData);
        PathHelper.MakeSurePath(pathOfBlueprintSet);
        PathHelper.MakeSurePath(pathOfSoundProfileSet);
        PathHelper.MakeSurePath(pathOfMetaEquipPartData);

        _instance = CreateInstance<SystemPathes>();
        UnityEditor.AssetDatabase.CreateAsset(
            _instance,
            pathOfPathes
        );
        UnityEditor.EditorUtility.SetDirty(_instance);
        _instance.pathes.Add(pathOfPathes);
        _instance.pathes.Add(pathOfCoreDataSet);
        _instance.pathes.Add(pathOfProfiles);
        _instance.pathes.Add(pathOfEditorResourceSet);
        _instance.pathes.Add(pathOfSkillDataSet);
        _instance.pathes.Add(pathOfStatusEffectSet);
        _instance.pathes.Add(pathOfItemDataSet);
        _instance.pathes.Add(pathOfMetaStatData);
        _instance.pathes.Add(pathOfBlueprintSet);
        _instance.pathes.Add(pathOfSoundProfileSet);
        _instance.pathes.Add(pathOfMetaEquipPartData);
        UnityEditor.AssetDatabase.Refresh();

        return Instance;
    }
#endif
}
