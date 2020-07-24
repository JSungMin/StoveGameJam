using System.Linq;
using System.Collections;
using System.Collections.Generic;
using CoreSystem.ProfileComponents;
using UnityEngine;
using UnityEditor;

namespace CoreSystem.Editor.ProfileViewer
{
    public class EquipmentViewer : EditorWindow
    {
        public static EquipmentViewer window;
        public static List<List<ItemProfile>> sortByPartIdItems = new List<List<ItemProfile>>();
        public static int selectedPartID = 0;
        public static ActorProfile selectedActor;
        public ItemProfile selectedProfile;

        public Vector2 itemListScrollView;

        public GUILayoutOption[] itemListOptions;
        public GUILayoutOption[] gridElementOptions;

        public static void ShowWindow(ActorProfile actor)
        {
            selectedActor = actor;
            window = GetWindow<EquipmentViewer>();
            window.titleContent.text = actor.actorName + " Equipment";
        }
        private void OnEnable()
        {
            InitializeItemList();
            itemListOptions = new GUILayoutOption[]
            {
                GUILayout.MaxWidth(200)
            };
            gridElementOptions = new GUILayoutOption[]
            {
                GUILayout.Width(200),
                GUILayout.Height(150)
            };
        }

        private void OnSelectionChange()
        {
            // Replace Equipment
            var tmpSelected = Selection.activeObject as ItemProfile;
            if (sortByPartIdItems[selectedPartID].Contains(tmpSelected))
                selectedProfile = tmpSelected;
            var prevElement = selectedActor.equipment.Elements[selectedPartID];
            if (prevElement == selectedProfile)
            {
                OnTakeOffEquipment(prevElement);
            }
            else
            {
                OnTakeOffEquipment(prevElement);
                OnTakeEquipment(selectedProfile);
            }
            AssetDatabase.SaveAssets();
        }
        private void OnGUI()
        {
            selectedProfile = Selection.activeObject as ItemProfile;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, itemListOptions);
            bool foldout = true;
            selectedPartID = EditorGUILayout.Popup(selectedPartID, CoreDataSet.MetaEquipPartData.Names.ToArray());
            itemListScrollView = EditorGUILayout.BeginScrollView(itemListScrollView);
            if (sortByPartIdItems.Count != 0)
            {
                sortByPartIdItems[selectedPartID] = CGUI.VObjectDropdown("Equipment", sortByPartIdItems[selectedPartID], ref foldout, null, itemListOptions);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            View.Elements = CGUI.ObjectGridField(0, 3, View.Elements, DrawViewElement, null);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        private void DrawViewElement(ItemProfile profile, int idx = -1)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, gridElementOptions);
            EditorGUILayout.LabelField(CoreDataSet.MetaEquipPartData.Names[idx]);
            EditorGUILayout.ObjectField(profile, typeof(ItemProfile));
            EditorGUILayout.EndVertical();
        }
        //  If no item in inventory then Create and Root it
        private void OnTakeEquipment(ItemProfile profile)
        {
            if (selectedActor.inventory.Contains(profile))
            {
                var element = selectedActor.inventory.FindElement(profile);
                selectedActor.TakeEquipment(element);
            }
            else
            {
                var element = selectedActor.inventory.RootItem(profile, 1);
                selectedActor.TakeEquipment(element);
            }
        }
        private void OnTakeOffEquipment(ItemProfile profile)
        {
            if (profile == null)
                return;
            selectedActor.TakeOffEquipment(profile.equipPartID);
        }
        private static void InitializeItemList()
        {
            if (sortByPartIdItems.Count != CoreDataSet.MetaEquipPartData.Count)
            {
                sortByPartIdItems = new List<List<ItemProfile>>();
                for (int i = 0; i < CoreDataSet.MetaEquipPartData.Count + 1; i++)
                {
                    var sortedItems = ItemProfileSet.Instance.elements.Where(x =>
                    {
                        if (x.itemType == ItemType.장비 &&
                            x.equipPartID == i)
                            return true;
                        return false;
                    }).ToList();
                    sortByPartIdItems.Add(sortedItems);
                }
            }
            else
            {
                for (int i = 0; i < CoreDataSet.MetaEquipPartData.Count + 1; i++)
                {
                    var sortedItems = ItemProfileSet.Instance.elements.Where(x =>
                    {
                        if (x.itemType == ItemType.장비 &&
                            x.equipPartID == i)
                            return true;
                        return false;
                    }).ToList();
                    sortByPartIdItems[i] = sortedItems;
                }
            }
        }
        public EquipmentView View
        {
            get { return selectedActor.equipment; }
            set { selectedActor.equipment = value; }
        }
    }
}
