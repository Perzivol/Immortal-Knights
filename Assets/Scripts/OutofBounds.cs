using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OutofBounds : MonoBehaviour
{
    public UIManager uiManager;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the PhotonView component of the colliding object
        PhotonView photonView = collision.gameObject.GetComponent<PhotonView>();
        
        // If the colliding object has a PhotonView, get the player number
        if(collision.gameObject.CompareTag("Player"))
        {
            if (photonView != null)
            {
                int playerNumber = photonView.Owner.ActorNumber;
                Debug.Log("Player " + playerNumber + " has gone out of bounds");
                //PhotonNetwork.Destroy(photonView);
            }
                
                
        }
        
    }

}
