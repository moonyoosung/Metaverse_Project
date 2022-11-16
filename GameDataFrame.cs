using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Game
{
    [System.Serializable]
    public class GameDataFrame
    {
        [Header("GameDataFrame")]
        public int index;
        public int id;
        public string userId;
        public string nickName;
        public bool isCrashed;

        public GameDataFrame()
        {
            index = 0;
            id = 0;
            userId = "";
            nickName = "";
        }
        /*
        public GameDataFrame(int id)
        {
            this.id = id;
            Debug.Log("Test : GameDataFrame id : " + id);
        }
        public GameDataFrame(int id, int index) : this(id)
        {
            this.index = index;
            Debug.Log("Test : GameDataFrame id : " + id + " index : " + index);
        }
        public GameDataFrame(int id, int index, string nickName) : this(id, index)
        {
            this.nickName = nickName;
            Debug.Log("Test : GameDataFrame id : " + id + " index : " + index + " nickName : " + nickName);
        }
        */
    }
}