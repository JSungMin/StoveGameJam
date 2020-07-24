using CoreSystem.ProfileComponents;

namespace CoreSystem
{
    public class CoreDataSet : SingletonScriptableObject<CoreDataSet>
    {
        public SystemPathes             systemPathes;
        public ActorProfileSet          profileSet;
        public SkillProfileSet          skillSet;
        public StatusEffectProfileSet   statusEffectSet;
        public ItemProfileSet           itemSet;
        public BlueprintProfileSet      blueprintProfileSet;
        public SoundProfileSet          soundProfileSet;
        public MetaStatData             metaStatData;
        public MetaEquipPartData        metaEquipPartData;

        public static MetaStatData MetaStatData => Instance.metaStatData;
        public static MetaEquipPartData MetaEquipPartData => Instance.metaEquipPartData;


    #if UNITY_EDITOR
        public static CoreDataSet CreateInstance()
        {
            var dataSet = CreateInstance<CoreDataSet>();
            UnityEditor.AssetDatabase.CreateAsset(
                dataSet, 
                SystemPathes.GetSystemPath(SystemPathes.PathType.CoreDataSet)
            );
            UnityEditor.AssetDatabase.Refresh();
            return Instance;
        }
    #endif
    }
}
