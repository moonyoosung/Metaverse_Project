using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MindPlus.Game
{
    public class GamePlayerBase
    {
        public MindPlusPlayer mindPlusPlayer;
        public bool isDead = false;
        public enum GameState { NONE, INREADY, INGAMEINFOPANEL, INGAME }
        public GameState gameState = GameState.NONE;
    }
    [System.Serializable]
    public class GamePlayerFrame<T> : GamePlayerBase where T : GameDataFrame, new()
    {
        public T gameData;
        public GamePlayerFrame()
        {

        }
        public GamePlayerFrame(T gameData)
        {
            this.gameData = gameData;
        }

        public virtual void SetGameData(int id, int index, string userID, string nickName)
        {
            gameData = new();
            gameData.id = id;
            gameData.index = index;
            gameData.userId = userID;
            gameData.nickName = nickName;
        }
        public virtual void Initialize(MindPlusPlayer mindPlusPlayer)
        {
            this.mindPlusPlayer = mindPlusPlayer;
        }
    }
}