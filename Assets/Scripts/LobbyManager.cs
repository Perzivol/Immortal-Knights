using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq; // Include this directive at the beginning of your file

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public Text MaxRoomsText;
    public Text playerName;
    [SerializeField]
    public InputField roomInputField;

    [SerializeField]
    public GameObject lobbyPanel;

    [SerializeField]
    public GameObject roomPanel;

    [SerializeField]
    public Text roomName;

    [SerializeField]
    public RoomItem roomItemPrefab;

    [SerializeField]
    public GameObject playButton;
    [SerializeField]
    public Text connectfailed;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;

    public float timeBetweenUpdates = 1f;
    float nextUpdateTime;
    private int maxPlayers = 2;
    public List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab; 
    public Transform playerItemParent;
    public int maxRooms = 10; //maximum number of rooms that can be created
    //private TypedLobby mainLobby = new TypedLobby("MainLobby", LobbyType.Default);
    private TypedLobby mainLobby = new TypedLobby(null, LobbyType.Default);
    string gameVersion = "1";

    protected new void OnEnable()
    {
        base.OnEnable(); // Call the base class OnEnable
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected new void OnDisable()
    {
        base.OnDisable(); // Call the base class OnDisable
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Lobby") // Check if the scene is the Lobby scene 
        {
            if (!PhotonNetwork.IsConnected)
            {
                ConnectToPhoton();
            }
        }
    }

    private void ConnectToPhoton()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "us";
        PhotonNetwork.NickName = ConnectToServer.UserName; // Make sure this is set appropriately
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
        roomInputField.interactable = false; // Disabling input field until connected to the server
    }
    private void Awake()
    {
        Debug.Log("Awake called on: " + this.gameObject.name, this.gameObject);
        //roomInputField.interactable = false; // Enabling input field after connecting to the server 
        playerName.text = "Welcome: " + PhotonNetwork.LocalPlayer.NickName;
    }
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers) 
        {
            // If all players are ready, show the play button
            if (AreAllPlayersReady()) 
            {
                playButton.SetActive(true);
            }
            else
            {
                playButton.SetActive(false);
            }
        }
        else
        {
            playButton.SetActive(false);
        }


        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Not connected");
        }

    }
    public void GoToMainMenu()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }

        SceneManager.LoadScene("MainMenu"); 

    }
    public void OnClickCreate()
    {
        if (PhotonNetwork.CountOfRooms >= maxRooms)
        {
            Debug.Log("Maximum number of rooms created. Please wait.");
            MaxRoomsText.text = "Maximum number of rooms created. Please wait.";
            return;
        }
        if(roomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions(){ MaxPlayers = 2, BroadcastPropsChangeToAll = true, IsVisible = true});
        }
    }

    public override void OnJoinedRoom()
    {
        // Debugging the nickname
        Debug.Log("OnJoinedRoom Setting nickname: " + PhotonNetwork.LocalPlayer.NickName);
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();

        // Debugging room status
        if (PhotonNetwork.CurrentRoom != null)
        {
            Debug.Log("OnJoinRoom Room IsOpen: " + PhotonNetwork.CurrentRoom.IsOpen);
            Debug.Log("OnJoinRoom Room IsVisible: " + PhotonNetwork.CurrentRoom.IsVisible);
        }
        else
        {
            Debug.Log("OnJoinRoom Not currently in a room.");
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created room successfully", this);
    }

    public void OnRefreshButtonClicked()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinLobby(); // This will trigger OnRoomListUpdate
        
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Created room failed:" + message, this);
        CreateRoom();
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        
        Debug.Log("OnRoomListUpdate called with " + roomList.Count + " rooms.");

        foreach (RoomInfo room in roomList)
        {
            Debug.Log("Room Found: " + room.Name);
        }
        Debug.Log("OnRoomListUpdate called");
        Debug.Log("roomList =" + roomList);
        string result = "List contents: ";
        foreach (var item in roomList)
        {
            result += item.ToString() + ", ";
        }
        Debug.Log(result);
        if (roomList.Count > 0)
        {
            UpdateRoomList(roomList);
        }
        else
        {
            Debug.Log("No rooms found");
        }
        nextUpdateTime = Time.time + timeBetweenUpdates;
        
    }
    void UpdateRoomList(List<RoomInfo> list)
    {
        
        Debug.Log("UpdateRoomList is called");
        foreach (RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear(); // Clearing the list is important to avoid duplicates

        string result = "List contents in UpdateRoomList: ";
        foreach (var item in list)
        {
            result += item.ToString() + ", ";
        }
        Debug.Log(result);

        foreach(RoomInfo room in list)
        {
            if (room.RemovedFromList)
            {
                // If the room is removed from the list, it should not be displayed.
                continue;
            }
            Debug.Log("room =");
            Debug.Log(room);
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            Debug.Log(newRoom);
            Debug.Log(room.Name);
            // Check if the room is full and update the name of the RoomItem accordingly
            if (room.PlayerCount == room.MaxPlayers)
            {
                newRoom.SetRoomName(room.Name + " (is full)");
            }
            else
            {
                newRoom.SetRoomName(room.Name);
            }
            roomItemsList.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        Debug.Log("inside joinroom lobby manager roomname is =" + roomName);
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftLobby()
    {
        Debug.Log("Player Left the lobby");
    }

    public override void OnLeftRoom()
    {
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);

        // Debugging room status
        if (PhotonNetwork.CurrentRoom != null)
        {
            Debug.Log("OnLeftRoom Room IsOpen: " + PhotonNetwork.CurrentRoom.IsOpen);
            Debug.Log("OnLeftRoom Room IsVisible: " + PhotonNetwork.CurrentRoom.IsVisible);
        }
        else
        {
            Debug.Log("OnLeftRoom Not currently in a room.");
        }

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby(mainLobby);
        // Debugging connection status
        Debug.Log("OnConnectedtoMaster Connection Status: " + PhotonNetwork.NetworkClientState);
        roomInputField.interactable = true; // Enabling input field after connecting to the server 
        

    }


    void UpdatePlayerList()
    {
        // Remove old PlayerItems
        foreach (Transform child in playerItemParent)
        {
            Destroy(child.gameObject);
        }

        // Create and display new PlayerItems
        if (PhotonNetwork.CurrentRoom != null)
        {
            foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
            {
                GameObject newPlayerItemObject = Instantiate(playerItemPrefab.gameObject, playerItemParent);
                PlayerItem newPlayerItem = newPlayerItemObject.GetComponent<PlayerItem>();
                newPlayerItem.SetPlayerInfo(playerInfo.Value);
            }
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player Entered Room: {newPlayer.NickName}, ActorNumber: {newPlayer.ActorNumber}");
        UpdatePlayerList();    
        if (PhotonNetwork.PlayerList.Length == maxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
        // Debugging room status
        if (PhotonNetwork.CurrentRoom != null)
        {
            Debug.Log("OnPlayerEnteredRoom Room IsOpen: " + PhotonNetwork.CurrentRoom.IsOpen);
            Debug.Log("OnPlayerEnteredRoom Room IsVisible: " + PhotonNetwork.CurrentRoom.IsVisible);
        }
        else
        {
            Debug.Log("OnPlayerEnteredRoom Not currently in a room.");
        }
        // Debugging the nickname
        Debug.Log("OnPlayerEnteredRoom Setting nickname: " + PhotonNetwork.LocalPlayer.NickName);
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined a Photon Lobby");
        
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player Left Room: {otherPlayer.NickName}, ActorNumber: {otherPlayer.ActorNumber}");
        UpdatePlayerList();
        if (!PhotonNetwork.CurrentRoom.IsOpen && !PhotonNetwork.CurrentRoom.IsVisible && PhotonNetwork.PlayerList.Length < maxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }

        // Debugging room status
        if (PhotonNetwork.CurrentRoom != null)
        {
            Debug.Log("OnPlayerLeftRoom Room IsOpen: " + PhotonNetwork.CurrentRoom.IsOpen);
            Debug.Log("OnPlayerLeftRoom Room IsVisible: " + PhotonNetwork.CurrentRoom.IsVisible);
        }
        else
        {
            Debug.Log("OnPlayerLeftRoom Not currently in a room.");
        }

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join a room but failed.");
        Debug.LogError($"Room creation failed: {message} (Return code: {returnCode})");
        // Inform the player that the room creation failed
        // For instance, you can display a UI message, play a sound, etc.
        // Example:
        MaxRoomsText.text = "Failed to create room. Please try again later.";
        // If you have a specific UI panel or element to display error messages, use it here.
        CreateRoom();
        
    }

    void CreateRoom()
    {
        if (PhotonNetwork.CountOfRooms >= maxRooms)
        {
            Debug.Log("Maximum number of rooms created. Please wait.");
            MaxRoomsText.text = "Maximum number of rooms created. Please wait.";
            return;
        }
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen = true, MaxPlayers = 2};
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
    }

    // Load the next scene once both players are in the same game room and one player pushes play
    public void PlayGameButton()
    {
        if (PhotonNetwork.IsMasterClient && AreAllPlayersReady())
        {
            // Only the master client should load the next scene
            PhotonNetwork.LoadLevel("Battlefield");
        }
    }
    private bool AreAllPlayersReady()
    {
        if (PhotonNetwork.CurrentRoom == null)
            return false;

        foreach (Player p in PhotonNetwork.CurrentRoom.Players.Values)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue("IsReady", out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                    return false; // One player is not ready, so return false
            }
            else
            {
                return false; // If a player hasn’t set the IsReady property, return false
            }
        }
        return true; // All players are ready
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Photon Network. Cause: " + cause.ToString());
        // Debugging connection status
        Debug.Log("OnDisconnected Connection Status: " + PhotonNetwork.NetworkClientState);
        // Retry connecting to the server or notify the user
        if (cause == DisconnectCause.ExceptionOnConnect)
        {
            Debug.LogError("Failed to initially connect to Photon Network.");
            // Implement user notification or a retry button
        }
        else
        {
            // You might want to retry connecting here
            RetryConnection();
        }
    }
    /*
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("IsReady"))
        {
            bool isReady = (bool)changedProps["IsReady"];
            Debug.Log($"IsReady Property Updated for Player {targetPlayer.NickName}: {targetPlayer.CustomProperties["IsReady"]}");
            int actorNumber = targetPlayer.ActorNumber;

            foreach (PlayerItem playerItem in playerItemsList)
            {
                playerItem.UpdateReadyStatus(isReady, actorNumber);
            }
        }
    }
    */
    void RetryConnection()
    {
        //buttonText.text = "Retry";
        // Wait for a few seconds before retrying
        Invoke("ConnectToPhoton", 5f);
    }
    

}
