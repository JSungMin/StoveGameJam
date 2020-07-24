using UnityEngine;
using CoreSystem.ProfileComponents;

namespace CoreSystem
{
    public partial class ActorProfile : ScriptableObject
    {
        public string actorName;
        public Color actorNameColor;
        [SerializeField]
        public PortraitDescSet portraitSet = new PortraitDescSet();
        public DynamicGauge HP;
        public DynamicGauge MP;
        public DynamicGauge Chegan;
        //  Set By Editor
        public Stat basicStat = new Stat();
        //  Set By Inventory, Status
        public Stat additionalStat = new Stat();
        public SkillView skills = new SkillView();
        public InventoryView inventory = new InventoryView();
        public EquipmentView equipment = new EquipmentView();
        public string description;

        public PortraitDesc GetPortraitDesc(string name)
        {
            return portraitSet.GetDesc(name);
        }
        public bool TakeEquipment(InventoryElement element)
        {
            var profile = element.Profile;
            var partID = profile.equipPartID;
            element.isEquipped = true;
            if (!equipment.IsEmpty(partID))
            {
                var prevEquipment = equipment.Elements[partID];
                inventory.FindElement(prevEquipment).isEquipped = false;
            }
            equipment.ReplaceElement(partID, profile);
            equipment.ApplyInfulence(additionalStat);
            return true;
        }
        public void TakeOffEquipment(int partID)
        {
            if (!equipment.IsEmpty(partID))
            {
                var prevEquipment = equipment.Elements[partID];
                inventory.FindElement(prevEquipment).isEquipped = false;
            }
            equipment.ReplaceElement(partID, null);
            equipment.ApplyInfulence(additionalStat);
        }
        public void TakeOffEquipment(InventoryElement element)
        {
            TakeOffEquipment(element.EquipPartID);
        }
#if UNITY_EDITOR
        //  Called By CreateInstance With Editor Project
        public void Initialize()
        {
            if (CoreDataSet.Instance == null)
            {
                var pathes = SystemPathes.Instance;
                CoreDataSet.Instance = Resources.Load<CoreDataSet>(SystemPathes.pathOfCoreDataSet);
            }
            basicStat.Initialize();
            additionalStat.Initialize();
            equipment.ConfirmMetaData();
        }
        public static ActorProfile CreateInstance()
        {
            var profileData = CreateInstance<ActorProfile>();
            var pathOfProfiles = SystemPathes.GetSystemPath(SystemPathes.PathType.ActorProfileSet);
            var parentPath = PathHelper.GetParentPath(pathOfProfiles);
            var profileCount = ActorProfileSet.CountOfElements();
            UnityEditor.AssetDatabase.CreateAsset(
                profileData,
                PathHelper.Bind(parentPath, "NewProfile_"+profileCount+".asset")
            );
            UnityEditor.EditorUtility.SetDirty(profileData);
            
            //  Add Default Portrait
            profileData.portraitSet.Add(new PortraitDesc());
            
            profileData.Initialize();

            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.SaveAssets();
            return profileData;
        }
    #endif
    }
}
