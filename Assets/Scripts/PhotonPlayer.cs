using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonPlayer : MonoBehaviour
{
    // script represents the presence of the player in the game not the character
    public static PhotonPlayer player;
    public PhotonView view;
    public GameObject selectedAvatar;
    public GameObject[] playerAvatar; //when we want to select characters in the future this will be an arry of all the character prefabs
    public GameObject myAvatar;
    //public Animator spawnAnimator;
    public int playerNumber;

     private void Awake()
    {
        if(PhotonPlayer.player == null)
        {
            PhotonPlayer.player = this;
        }
    }

    void Start()
    {
        view = GetComponent<PhotonView>();

        if(view.IsMine)
        {

            int selectedCharacter = (int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"];
            Debug.Log("Selected Character in Photon Player: " + selectedCharacter);
            //spawns the actually character on to the presence of the player
            selectedAvatar = playerAvatar[selectedCharacter];
            myAvatar = PhotonNetwork.Instantiate(selectedAvatar.name , PlayerSpawner.playerSpawner.spawnPoint.position, Quaternion.identity);
             
        }
        
    }



}
