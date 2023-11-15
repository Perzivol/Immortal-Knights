using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinScreenManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Button lobbyButton; // Reference to your button
    public static WinScreenManager Instance { get; private set; }
    private void Awake()
    {
        if (lobbyButton != null)
        {
            lobbyButton.onClick.AddListener(GoToLobby);
        }
        else
        {
            Debug.LogError("Lobby Button is not assigned in the inspector.");
        }

        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("Not connected to Photon, unable to remove players from the room.");
            return;
        }

        if (!PhotonNetwork.InRoom)
        {
            Debug.LogWarning("Not in a Photon room, nothing to disconnect from.");
            return;
        }

        
    }

    private void RemovePlayersFromRoom()
    {
        // Disconnect the local player from the room
        PhotonNetwork.LeaveRoom();
    }

    public void GoToLobby()
    {
        RemovePlayersFromRoom();
        // Load the lobby scene
        SceneManager.LoadScene("Lobby"); // Replace with your lobby scene name
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Successfully left the room.");
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.Log($"Disconnected from Photon: {cause}");
    }
}
