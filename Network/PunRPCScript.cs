using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PunRPCScript : MonoBehaviourPun
{
    private PhotonView pv;
    private LookAtCanvas lookAtCanvas;
    private Customization customization;

    public void Initalize(MindPlusPlayer mindPlusPlayer)
    {
        this.pv = mindPlusPlayer.pv;
        this.lookAtCanvas = mindPlusPlayer.GetPart<LookAtCanvas>();
        this.customization = mindPlusPlayer.GetPart<PlayerRig>().customization;
        pv.FindObservables();
    }

    public void SendRPCAvatar(string bags, string belts, string bottom, string earrings, string glasses, string hair, string hand, string hats, string head, string necklaces, string top)
    {
        pv.RPC(nameof(SetAvatarRPC), RpcTarget.AllBuffered, bags, belts, bottom, earrings, glasses, hair, hand, hats, head, necklaces, top);
    }

    [PunRPC]
    public void SetAvatarRPC(string bags, string belts, string bottom, string earrings, string glasses, string hair, string hand, string hats, string head, string necklaces, string top)
    {
        customization.ReceiveRPCAvatar(bags, belts, bottom, earrings, glasses, hair, hand, hats, head, necklaces, top);
    }


    public void SendRPCMyNick()
    {
        pv.RPC(nameof(SetMyNickRPC), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
    }

    public void SendRPCBubbleMessage(bool isEnable, string msg)
    {
        pv.RPC(nameof(SetMyMessage), RpcTarget.All, isEnable, msg);
    }

    public void SendSpeakerSprite(bool isEnable)
    {
        pv.RPC(nameof(SetMySpeakerSprite), RpcTarget.All, isEnable);
    }

    [PunRPC]
    public void SetMyNickRPC(string nick)
    {
        lookAtCanvas.setPlayerNick.ReceiveRPCMyNick(nick);
    }
    [PunRPC]
    public void SetMyMessage(bool isEnable, string msg)
    {
        lookAtCanvas.speechBalloon.ReceiveRPCMyMessage(isEnable, msg);
    }
    [PunRPC]
    public void SetMySpeakerSprite(bool isEnable)
    {
        lookAtCanvas.playerSpeaker.ReceiveRPCMySpeakerSprite(isEnable);
    }
}
