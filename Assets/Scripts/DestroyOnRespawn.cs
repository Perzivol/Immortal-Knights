using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class DestroyOnRespawn : MonoBehaviourPunCallbacks
{
    public PhotonView view;

    void Start()
    {
     
        view = GetComponent<PhotonView>();
    }
    public void OnEnable()
    {
        // Listen for the custom event with code 1
        PhotonNetwork.NetworkingClient.EventReceived += OnCustomEventReceived;
    }

    public void OnDisable()
    {
        // Stop listening for the custom event
        PhotonNetwork.NetworkingClient.EventReceived -= OnCustomEventReceived;
    }

    private void OnCustomEventReceived(EventData eventData)
    {
        if (eventData.Code == 1)
        {
            // Get the actor number from the event data
            int actorNumber = (int)eventData.CustomData;

            // If this game object belongs to the player with the actor number, destroy it
            if (photonView.Owner.ActorNumber == actorNumber)
            {
                if(view.IsMine)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            }

            
        }
    }
}