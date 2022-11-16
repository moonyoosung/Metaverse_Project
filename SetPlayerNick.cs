using MindPlus.Contexts.Player;
using Photon.Pun;
using Slash.Unity.DataBind.Core.Presentation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SetPlayerNick : MonoBehaviour, IPlayerComponent
{
    public ContextHolder contextHolder;
    [Header("UI")]
    private bool isLocalPlayer;
    public PlayerNickContext Context { private set; get; }

    public void Initialize(MindPlus.GameManager gameManager, MindPlusPlayer mindPlusPlayer, bool isOnline, bool isLocalPlayer, bool activeAudioListener)
    {
        this.Context = new PlayerNickContext();
        contextHolder.Context = Context;
        mindPlusPlayer.Context.PlayerNickContext = Context;

        this.isLocalPlayer = isLocalPlayer;

        if (mindPlusPlayer.RPC && isLocalPlayer)
        {
            Context.SetValue("NickName", gameManager.Persistent.AccountManager.PlayerData.userName);
            mindPlusPlayer.RPC.SendRPCMyNick();
            if(!ColorUtility.TryParseHtmlString("#A3FF8C", out Color color))
            {
                Debug.LogWarning("Not Parse " + "#A3FF8C");
            }
            mindPlusPlayer.ChangePlayerNickNameColor(color);
        }
    }

    public void ReceiveRPCMyNick(string nick)
    {
        if (!isLocalPlayer)
        {
            Context.SetValue("NickName", nick);
        }
    }
}
