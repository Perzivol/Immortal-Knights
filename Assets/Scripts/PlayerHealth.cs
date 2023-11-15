using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class PlayerHealth : MonoBehaviour
{
    public PhotonView view;
    [SerializeField]
    private Animator animator;

    public UIManager uiManager;
    
    [SerializeField]
    private int currentHealth, maxHealth;

    public UnityEvent<GameObject> OnHitWithReference, OnDeathWithReference;

    private int playerId;
    [SerializeField]
    private bool isDead = false;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        playerId = view.Owner.ActorNumber; // Get the ActorNumber/PlayerID
        Debug.Log("PlayerID: " + playerId);
    }
    public void InitializeHealth(int healthValue)
    {
        currentHealth = healthValue;
        maxHealth = healthValue;
        isDead = false;
    }

    public void GetHit(int amount, GameObject sender)
    {
        Debug.Log("GetHit");
        Debug.Log("Current Health Before Hit: " + currentHealth); // Debug Statement 1

        if(view.IsMine)
        {
            view.RPC("GetHit_RPC", RpcTarget.All);
        }

        currentHealth -= amount;
        Debug.Log("Current Health After Hit: " + currentHealth); // Debug Statement 2

        if(currentHealth > 0)
        {
            OnHitWithReference?.Invoke(sender);
        }
        else
        {
            Debug.Log("Calling CheckDeath Method"); // Debug Statement 3
            CheckDeath(sender);
        }
    }


    [PunRPC]
    void GetHit_RPC()
    {
        Debug.Log("GetHit_RPC");
        animator.SetTrigger("GetHit");
    }
    private void CheckDeath(GameObject sender)
    {
        OnDeathWithReference?.Invoke(sender);
        isDead = true;
        
        if (view.IsMine)
        {

            if (UIManager.Instance == null)
            {
                Debug.LogError("UIManager.Instance is null");
                return;
            }

            PhotonView photonView = UIManager.Instance.GetComponent<PhotonView>();

            if (photonView == null)
            {
                Debug.LogError("PhotonView is null on UIManager");
                return;
            }

            // Call the DecreaseLives method on all players' screens using an RPC
            photonView.RPC("DecreaseLives", RpcTarget.All, playerId);
            Debug.Log("Called Decrease Lives");

            view.RPC("DestroyPlayer_RPC", RpcTarget.All);
            Debug.Log("DestroyPlayer_RPC");
        
        }
        
    }
    
    [PunRPC]
    void DestroyPlayer_RPC()
    {
        
        if (view.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }

        // Find the PlayerSpawner object
        GameObject playerSpawnerObject = GameObject.Find("PlayerSpawner");

        // Get the PlayerSpawnerScript component
        PlayerSpawner playerSpawner = playerSpawnerObject.GetComponent<PlayerSpawner>();

        // Call the SpawnPlayer() function on the PlayerSpawnerScript component
        playerSpawner.Respawn_Command(view.Owner.ActorNumber);

    }

    public bool IsPlayerDead()
    {
        return isDead;
    }
}
