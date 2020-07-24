using System;
using CoreSystem;

namespace CoreSystem.ProfileComponents
{
    [System.Serializable]
    public class ItemAmountWrapper {
        public ItemProfile profile;
        public int amount;
        public ItemAmountWrapper()
        {
            profile = null;
            amount = 0;
        }
        public ItemAmountWrapper(ItemProfile profile, int amount)
        {
            this.profile = profile;
            this.amount = amount;
        }
    }
    [Serializable]
    public class InventoryElement
    {
        public bool isEquipped = false;
        public ItemAmountWrapper items;

        public InventoryElement()
        {
            isEquipped = false;
            items = new ItemAmountWrapper();
        }
        public InventoryElement(ItemProfile profile, int amount = 1, bool isEquipped = false)
        {
            this.items = new ItemAmountWrapper(profile, amount);
            this.isEquipped = isEquipped;
        }

        public ItemProfile Profile => items.profile;
        public int EquipPartID => items.profile.equipPartID;
        public int Amount
        {
            get
            {
                return items.amount;
            }
            set
            {
                items.amount = value;
            }
        }
    }
}