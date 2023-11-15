using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq; // Include this directive at the beginning of your file
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;
public class PlayerItem : MonoBehaviourPunCallbacks
{
    //public static PlayerItem playerItem;
    public Player Player { get; set; } // ensuring 'Player' is defined here
    public Text playerName;
    private int selectedCharacterIndex = 0;
    //Image backgroundImage; need to change sprite color not background color.
    //public Color highlightColor;
    public Button readyButton;
    public Sprite selectedSprite; // Drag your selected sprite here in the inspector
    public Sprite unselectedSprite; // Drag your unselected sprite here in the inspector
    public Text readyText;
    public GameObject leftArrowButton;
    public GameObject rightArrowButton; 
    //Image backgroundImage;
    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable(); //photon hash table (array) refering to items with a name
    public GameObject playerAvatar;
    public GameObject[] avatars;
    private bool isReady;
    Player player;
    [SerializeField]
    private PhotonView view;
    private Player playerInfo; // Assuming playerInfo is of type Player
    //problem, if no character selected then nothing shows up in game. need to set default character.
    private void Start() //might need to be the awake function. changed to start for now.
    { 
        view = GetComponent<PhotonView>();
        playerProperties["playerAvatar"] = 0;
        playerAvatar = avatars[0];
        avatars[0].SetActive(true);
        avatars[1].SetActive(false);

         // Set the default character index to 0
        selectedCharacterIndex = 0;
    }

    public void SetPlayerInfo(Player _player)
    {
        if(_player == null)
        {
            Debug.LogError("Player object is null.");
            return;
        }

        if(string.IsNullOrEmpty(_player.NickName))
        {
            Debug.LogError($"Player NickName is null or empty. ActorNumber: {_player.ActorNumber}");
            return;
        }
        Debug.Log($"Setting info for player: {_player.NickName}, ActorNumber: {_player.ActorNumber}");
        player = _player;
        this.playerInfo = player;
        playerName.text = _player.NickName;
        UpdatePlayerItem(player);
        // Set button interactability based on whether this is the local player or not
        readyButton.interactable = player.IsLocal;
        leftArrowButton.SetActive(player.IsLocal);
        rightArrowButton.SetActive(player.IsLocal);
        
    }
    public void UpdateReadyStatus(bool isReady, int actorNumber)
    {
        if (actorNumber == player.ActorNumber)
        {
            // Update the UI based on readiness status
            UpdateReadyUI(isReady);
        }
    }
    public void ApplyLocalChanges()
    {
        Debug.Log("ApplyLocalChanges called");
        //backgroundImage.color = highlightColor;
        leftArrowButton.SetActive(true);
        rightArrowButton.SetActive(true);
    }
    public void OnReadyButtonClicked()
    {
        if (player.IsLocal)
        {
            isReady = !isReady;
            // Directly setting the player's custom properties
            ReadyUp(isReady);

            if(view.IsMine)
            {
                UpdateReadyUI(isReady);
            }
        }

    }

    public void UpdateReadyUI(bool isReady)
    {
        if (isReady)
        {
            readyText.text = "Ready";
            readyButton.image.sprite = selectedSprite; 
        }
        else
        {
            readyText.text = "Not Ready";
            readyButton.image.sprite = unselectedSprite;
        }
    }
    /*
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // Check whether the changed properties contain "IsReady" key and the targetPlayer is not local
        if (changedProps.ContainsKey("IsReady") && !view.IsMine)
        {
            bool isReady = (bool)changedProps["IsReady"];
            Debug.Log($"Player {targetPlayer.NickName} IsReady status changed to: {isReady}");
            
            // Call your method to update the UI
            UpdateReadyUI(isReady);
        }

        
    }
    */
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer.ActorNumber == player.ActorNumber)
        {
            if (changedProps.ContainsKey("playerAvatar"))
            {
                UpdatePlayerItem(targetPlayer);
            }
            
            if (changedProps.ContainsKey("IsReady"))
            {
                bool isReady = (bool)changedProps["IsReady"];
                UpdateReadyUI(isReady);
            }
        }
    }
    // Public property to access the isReady field
    public bool IsReady
    {
        get { return isReady; }
    }

    public void OnClickLeftArrow()
    {
        if((int)playerProperties["playerAvatar"] == 0)
        {
            playerProperties["playerAvatar"] = avatars.Length - 1;
        }
        else
        {
            playerProperties["playerAvatar"] = (int)playerProperties["playerAvatar"] - 1;
        }
        // Update the selected character index
        selectedCharacterIndex = (int)playerProperties["playerAvatar"];
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void OnClickRightArrow()
    {
        if((int)playerProperties["playerAvatar"] == avatars.Length - 1)
        {
            playerProperties["playerAvatar"] = 0;
        }
        else
        {
            playerProperties["playerAvatar"] = (int)playerProperties["playerAvatar"] + 1;
        }
        // Update the selected character index
        selectedCharacterIndex = (int)playerProperties["playerAvatar"];
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }


    void UpdatePlayerItem(Player player)
    {
        
        if(player.CustomProperties.ContainsKey("playerAvatar"))
        { 
            playerAvatar = avatars[(int)player.CustomProperties["playerAvatar"]];
            playerProperties["playerAvatar"] = (int)player.CustomProperties["playerAvatar"];
            if((int)player.CustomProperties["playerAvatar"] == 0)
            {
                avatars[0].SetActive(true);
                avatars[1].SetActive(false);
            }
            else
            {
                avatars[0].SetActive(false);
                avatars[1].SetActive(true);
            }
        }
        else
        {
            playerProperties["playerAvatar"] = 0;
        }
        //Debug.Log(playerProperties["playerAvatar"]);
    }

    public void ReadyUp(bool isReady)
    {
        // Assuming you have reference to the PhotonPlayer
        Hashtable readyupProperty = new Hashtable();
        readyupProperty["IsReady"] = isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(readyupProperty);
        Debug.Log($"IsReady Property Set: {PhotonNetwork.LocalPlayer.CustomProperties["IsReady"]}");
    }

}
