using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemData", menuName = "MindPlus/ItemData", order = 0)]
[System.Serializable]
public class UIItemListData : ScriptableObject
{
    [System.Serializable]
    public class Item
    {
        public string customId;
        public string part;
        public string meshId;
        public string textureId;
        public int coin;
        public int heart;
        public string thumbnail;
        public int owned;
        public Sprite valueIcon;

        public int GetCost()
        {
            if (coin != 0)
                return coin;
            return heart;
        }

        public bool isCoin()
        {
            if (coin > 0)
                return true;
            return false;
        }
    }

    public List<Item> Items = new List<Item>();
}
