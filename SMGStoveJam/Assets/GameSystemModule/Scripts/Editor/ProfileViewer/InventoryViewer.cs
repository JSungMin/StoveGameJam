using System.Linq;
using System.Collections;
using System.Collections.Generic;
using CoreSystem.ProfileComponents;
using UnityEngine;
using UnityEditor;

namespace CoreSystem.Editor.ProfileViewer
{
    public class InventoryViewer : EditorWindow
    {
        public static InventoryViewer window;
        public static List<List<ItemProfile>> sortByTypeItems = new List<List<ItemProfile>>();
        public static ItemType selectedType;
        public static ActorProfile selectedActor;
        public ItemProfile selectedProfile;

        public Vector2 itemListScrollView;
        public Vector2 itemGridScrollView;

        public GUILayoutOption[] itemListOptions;
        public GUILayoutOption[] gridElementOptions;
        public GUILayoutOption[] itemIconOptions;

        public static void ShowWindow(ActorProfile actor)
        {
            selectedActor = actor;
            window = GetWindow<InventoryViewer>();
            window.titleContent.text = actor.actorName + " Inventory";
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
            itemIconOptions = new GUILayoutOption[]
            {
                GUILayout.Width(50),
                GUILayout.Height(50)
            };
        }
        private void OnSelectionChange()
        {
            // Replace Equipment
            var tmpSelected = Selection.activeObject as ItemProfile;
            if (sortByTypeItems[(int)selectedType].Contains(tmpSelected))
                selectedProfile = tmpSelected;
            View.RootItem(selectedProfile, 1);
            AssetDatabase.SaveAssets();
        }
        private void OnGUI()
        {
            selectedProfile = Selection.activeObject as ItemProfile;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, itemListOptions);
            bool foldout = true;
            selectedType = (ItemType)EditorGUILayout.EnumPopup(selectedType);
            itemListScrollView = EditorGUILayout.BeginScrollView(itemListScrollView);
            if (sortByTypeItems.Count > 0)
            {
                sortByTypeItems[(int)selectedType] = CGUI.VObjectDropdown("Items", sortByTypeItems[(int)selectedType], ref foldout, null, itemListOptions);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            View.money = EditorGUILayout.DelayedFloatField("Money", View.money);
            itemGridScrollView = EditorGUILayout.BeginScrollView(itemGridScrollView);
            View.Elements = CGUI.ObjectGridField(0, 3, View.Elements, DrawViewElement, null);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        private void DrawViewElement(InventoryElement element, int idx = -1)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, gridElementOptions);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            CGUI.SpriteField("Icon", element.Profile.itemIcon, itemIconOptions);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(element.Profile.itemName);
            EditorGUILayout.LabelField(element.Profile.itemType.ToString());
            GUILayout.FlexibleSpace();
            element.items.profile = EditorGUILayout.ObjectField(element.Profile, typeof(ItemProfile)) as ItemProfile;
            element.Amount = EditorGUILayout.IntField("Amount", element.Amount);
            if (element.Amount <= 0)
                View.DropItem(element.items.profile, 0);
            EditorGUI.BeginDisabledGroup(true);
            element.isEquipped = EditorGUILayout.Toggle("IsEquipped", element.isEquipped);
            EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
       
        private static void InitializeItemList()
        {
            if (sortByTypeItems.Count != ItemProfileSet.CountOfElements())
            {
                sortByTypeItems = new List<List<ItemProfile>>();
                for (int i = 0; i < EnumHelper.CountOfElement<ItemType>(); i++)
                {
                    var sortedItems = ItemProfileSet.Instance.elements.Where(x =>
                    {
                        if (x.itemType == (ItemType)i)
                            return true;
                        return false;
                    }).ToList();
                    sortByTypeItems.Add(sortedItems);
                }
            }
            else
            {
                for (int i = 0; i < EnumHelper.CountOfElement<ItemType>(); i++)
                {
                    var sortedItems = ItemProfileSet.Instance.elements.Where(x =>
                    {
                        if (x.itemType == (ItemType)i)
                            return true;
                        return false;
                    }).ToList();
                    sortByTypeItems[i] = sortedItems;
                }
            }
        }
        public InventoryView View
        {
            get { return selectedActor?.inventory; }
            set { selectedActor.inventory = value; }
        }
    }
}
