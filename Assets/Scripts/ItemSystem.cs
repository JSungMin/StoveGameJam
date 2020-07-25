
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Item // 아이템의 타입.
{
    ink,
    hammer
}

public class ItemSystem : MonoBehaviour
{
    [System.Serializable]
    public class HaveItem
    {
        public string name; // 무슨 값을 설정해도 itemtype의 string값으로 초기화함.
        public Item itemType;
        public int count;
        public Sprite itemImg;
    }

    public List<HaveItem> itemList;

    public GameObject itemUILayout;
    public GameObject itemUIObj;
    public List<GameObject> itemUIObjList;

    public void Awake()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].name = itemList[i].itemType.ToString();
        }
        ItemUIObj_Reflash();
    }

    public HaveItem ItemFind(Item _type) // 아이템을 리스트에서 찾아서 리턴
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

    public void ItemReset(int _ct = 0) // 아이템 개수를 모두 초기화 합니다.
    {
        for(int i = 0; i < itemList.Count; i++)
        {
            itemList[i].count = _ct;
        }
        ItemUIObj_Reflash();
    }

    public void ItemGet(Item _type, int _ct = 1) // 아이템을 얻습니다.
    {
        ItemFind(_type).count += _ct;
        ItemUIObj_Reflash();
    }

    public bool ItemUse(Item _type, int _ct = 1) // 아이템을 사용합니다. (기본값 : 1개 사용)
    {
        var target = ItemFind(_type);
        if (target.count > _ct) return false;
        ItemFind(_type).count -= _ct;
        ItemUIObj_Reflash();
        return true;
    }


    /********** 아이템 UI ***********/
    public void ItemUIObj_Show(bool _show = true) // 아이템 UI를 보여줍니다.
    {
        itemUILayout.SetActive(_show);
    }

    public void ItemUIObj_Reflash() // 아이템 UI를 새로고침합니다. (아이템 사용 시 바로 적용될 수 있도록)
    {
        for(int i = 0; i < itemList.Count; i++)
        {
            var target = itemList[i];
            if (target.count <= 0) // 아이템의 개수가 0일 경우
            {
                for(int j = 0; j < itemUIObjList.Count; j++)
                {
                    if (itemUIObjList[j].name == target.name)
                        itemUIObjList[j].SetActive(false);
                }
            }
            else // 아이템의 개수가 1개 이상일 경우
            {
                GameObject obj = null;
                for (int j = 0; j < itemUIObjList.Count; j++) // 이미 UI에 객체가 있으면 
                {
                    if (itemUIObjList[j].name == target.name)
                    {
                        obj = itemUIObjList[j];
                        obj.SetActive(true);
                    }
                }
                if(obj == null)
                {
                    obj = Instantiate(itemUIObj, Vector3.zero, Quaternion.identity);
                    obj.name = target.name;
                    obj.transform.SetParent(itemUILayout.transform);
                    itemUIObjList.Add(obj);
                }
                obj.transform.Find("ItemText").GetComponent<Text>().text = " × " + target.count;
                obj.transform.Find("ItemImage").GetComponent<Image>().sprite = target.itemImg;
            }
            
        }

    }

}
