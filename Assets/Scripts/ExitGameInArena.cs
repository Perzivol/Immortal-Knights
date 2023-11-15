using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGameInArena : MonoBehaviourPunCallbacks
{
    public GameObject aloneMessage;
    public void button_exit()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Application.Quit();
        }
    }

    public override void OnLeftRoom()
    {
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            aloneMessage.SetActive(true);
        }
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Application.Quit();
    }
}