using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class NextScene : MonoBehaviourPunCallbacks
{
    public void NextSceneButton()
    {
        SceneManager.LoadScene("WinScreen");
    }

    public void LeaveRoomWhenAlone()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
}
