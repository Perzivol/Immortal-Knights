using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;


public class RespawnButton : MonoBehaviourPunCallbacks
{
    
    public Button button;

    public int playerNumber;
    
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        respawn_button();
        button.interactable = false;

        // Enable the button again after a certain time
        float delayTime = 2.0f; // Time in seconds before button is enabled again
        StartCoroutine(EnableButton(delayTime));
    }

    public void respawn_button()
    {
        playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        PhotonPlayer.player.myAvatar = PhotonNetwork.Instantiate(PhotonPlayer.player.selectedAvatar.name, PlayerSpawner.playerSpawner.spawnPoint.position, Quaternion.identity);
       
        
        //PhotonNetwork.RaiseEvent(1, playerNumber, RaiseEventOptions.Default, SendOptions.SendReliable)
    }
    
    private IEnumerator EnableButton(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        button.interactable = true;
    }
    
}