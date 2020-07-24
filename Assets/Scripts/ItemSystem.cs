
using System.Collections.Generic;
using UnityEngine;

public enum Item
{
    ink,
    hammer,
    NumberOfTypes // 사용하지 않는 enum입니다.
}

public class ItemSystem : MonoBehaviour
{
    [System.Serializable]
    public class HaveItem
    {
        public string name; // 사용하지 않지만 가독성을 위해 넣어둡니다.
        public Item itemType;
        public int count;

        #region 너무 단순한 기능
        public HaveItem(Item _itemType, int _count = 0)
        {
            name = _itemType.ToString();
            itemType = _itemType;
            count = _count;
        }

        public void ItemUse(int _ct = 1)
        {
            count -= _ct;
        }

        public void ItemGet(int _ct = 1)
        {
            count += _ct;
        }
        #endregion
    }

    public List<HaveItem> itemList;

    public GameObject itemUILayout;
    public GameObject itemUIObj;
    public List<GameObject> itemUIObjList;

    public void Awake()
    {
        for(int i = 0; i < (int)Item.NumberOfTypes; i++)
        {
            itemList.Add(new HaveItem((Item)i, 1));
        }
        ItemUIObj_Reflash();
    }

    public HaveItem ItemFind(Item _type)
    {
        HaveItem result = null;
        for(int i = 0; i < itemList.Count; i++)
        {
            if(itemList[i].itemType == _type)
            {
                result = itemList[i];
            }
        }
        return result;
    }

    public void ItemReset(int _ct = 0)
    {
        for(int i = 0; i < itemList.Count; i++)
        {
            itemList[i].count = _ct;
        }
    }

    public void ItemGet(Item _type, int _ct = 1)
    {
        ItemFind(_type).ItemGet(_ct);
    }

    public void ItemUse(Item _type, int _ct = 1)
    {
        ItemFind(_type).ItemUse(_ct);
    }

    public void ItemUIObj_Show(bool _show = true)
    {
        itemUILayout.SetActive(_show);
    }

    public void ItemUIObj_Reflash()
    {
        // int ct = 0;
        for(int i = 0; i < itemList.Count; i++)
        {
            var target = itemList[i];
            if (target.count <= 0) continue;
            GameObject obj = Instantiate(itemUIObj, Vector3.zero, Quaternion.identity);
            obj.name = target.name;
            obj.transform.parent = itemUILayout.transform;
            itemUIObjList.Add(obj);
        }

    }

}
