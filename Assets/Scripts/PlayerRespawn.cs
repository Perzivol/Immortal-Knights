using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerRespawn : MonoBehaviour
{

    Player[] allPlayers;
    //public static PlayerSpawner instance;
    public GameObject playerPrefabs;
    //public GameObject PlayerSpawner;
    public Transform RespawnPoints;
    //public GameObject fallDetector;
    private int playerNumber;
    public PlayerSpawner[] spawnPoints;
    
    //int playerNumber = 0;
    //int myNumberInRoom = 0;
    private void Start()
    {
        playerNumber = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        //RespawnPoints = Player.spawnPoints
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void respawnPlayer()
    {
        Transform spawnPoint = RespawnPoints;
        //Transform spawnPoint = RespawnPoints[playerNumber];
        //allPlayers = PhotonNetwork.PlayerList;
        this.transform.position = spawnPoint.position;
        //GameObject playerToSpawn = playerPrefabs;
        if (playerNumber == 1) 
        {
            //playerToSpawn.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f); //may need to come back to this spawning in the correct direction
            //this.gameObject.position = spawnPoint;
        }
        else
        {
            //playerToSpawn.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f);
        }
        
        //PhotonNetwork.Instantiate(playerToSpawn.name, spawnPoint.position, Quaternion.identity); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "FallDetector")
        {
            respawnPlayer();
        }
    }
}
