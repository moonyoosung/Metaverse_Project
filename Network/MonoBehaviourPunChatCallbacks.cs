using ExitGames.Client.Photon;
using Photon.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.A_MindPlus.Scripts.Network
{
    public class MonoBehaviourPunChatCallbacks : MonoBehaviour, IChatClientListener
    {
        public virtual void DebugReturn(DebugLevel level, string message)
        {
        }

        public virtual void OnChatStateChange(ChatState state)
        {
        }

        public virtual void OnConnected()
        {
        }

        public virtual void OnDisconnected()
        {
        }

        public virtual void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
        }

        public virtual void OnPrivateMessage(string sender, object message, string channelName)
        {
        }

        public virtual void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
        }

        public virtual void OnSubscribed(string[] channels, bool[] results)
        {
        }

        public virtual void OnUnsubscribed(string[] channels)
        {
        }

        public virtual void OnUserSubscribed(string channel, string user)
        {
        }

        public virtual void OnUserUnsubscribed(string channel, string user)
        {
        }
    }
}
