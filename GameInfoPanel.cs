using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameInfo
{
    public string title;
    public string description;
    public int number;
    public Sprite gameScreen;
    [Header("Cotrol key")]
    public bool isMove = false;
    public bool isJump = false;
    public bool isPush = false;
    public bool isTap = false;

    public GameInfo() { }

    public GameInfo(GameInfo gameInfo)
    {
        this.title = gameInfo.title;
        this.description = gameInfo.description;
        this.number = gameInfo.number;
        this.gameScreen = gameInfo.gameScreen;
        this.isMove = gameInfo.isMove;
        this.isJump = gameInfo.isJump;
        this.isPush = gameInfo.isPush;
        this.isTap = gameInfo.isTap;
    }
}

public class GameInfoPanel : MonoBehaviour
{
    public Text title;
    public Text description;
    public Text number;
    public Image gameScreen;

    [Header("Cotrol key")]
    public GameObject objectMove;
    public GameObject objectJump;
    public GameObject objectPush;
    public GameObject objectTap;

    public void SetGameInfo(GameInfo gameInfo)
    {
        title.text = gameInfo.title;
        description.text = gameInfo.description;
        number.text = gameInfo.number.ToString();
        gameScreen.sprite = gameInfo.gameScreen;

        objectMove.SetActive(gameInfo.isMove);
        objectJump.SetActive(gameInfo.isJump);
        objectPush.SetActive(gameInfo.isPush);
        objectTap.SetActive(gameInfo.isTap);
    }
}
