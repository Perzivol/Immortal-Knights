using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner playerSpawner;

    Player[] allPlayers;
    public GameObject myPlayer;
    //public static PlayerSpawner instance;
    public GameObject playerPrefabs;
    public Transform[] spawnPoints;
    public Transform spawnPoint;
    public GameObject playerToSpawn;
    //public Animator spawnAnimator1;
    //public Animator spawnAnimator2;
    //public Animator spawnAnimator;
    public bool spawnFacingLeft;
    public Text player1;
    public Text player2;
    public int playerNumber;
    private void OnEnable()
    {
        
    }

    private void Start()
    {
        if(PlayerSpawner.playerSpawner == null)
        {
            PlayerSpawner.playerSpawner = this;
        }

        playerNumber = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        spawnPoint = spawnPoints[playerNumber];
        allPlayers = PhotonNetwork.PlayerList;

        string result = "Players in Game: ";
        foreach (var item in allPlayers)
        {
            result += item.ToString() + ", ";
        }
        Debug.Log(result);

        player1.text = allPlayers[0].NickName;
        player2.text = allPlayers[1].NickName;

        
        
        playerToSpawn = playerPrefabs;
        
        if (playerNumber == 0) // If this is the second player, flip the player object horizontally
        {
            spawnFacingLeft = false;
            Debug.Log("Player 1 Spawned, Actor Number: " + (playerNumber+1));
        }
        else if(playerNumber == 1)
        {
            spawnFacingLeft = true;
            Debug.Log("Player 2 Spawned, Actor Number: " + (playerNumber+1));
        }
        
        myPlayer = PhotonNetwork.Instantiate(playerToSpawn.name, spawnPoint.position, Quaternion.identity); 
        //GameObject playerToSpawn = playerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        
        //playerToSpawn.transform.parent = transform; get scale from parent? maybe
       
        

        
    }

    public void Respawn_Command(int actorNumber)
    {
        // Get the local player’s actor number
        int localPlayerNumber = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        // Check if the actor number to respawn matches the local player’s actor number
        if(localPlayerNumber == actorNumber - 1)
        {
            // Get the appropriate spawn point based on the actor number
            spawnPoint = spawnPoints[localPlayerNumber];

            // Instantiate the player’s avatar at the correct spawn point
            myPlayer = PhotonNetwork.Instantiate(PhotonPlayer.player.selectedAvatar.name, spawnPoint.position, Quaternion.identity);

            // You might want to initialize or reset some of the player’s state here
        }
    }


}
