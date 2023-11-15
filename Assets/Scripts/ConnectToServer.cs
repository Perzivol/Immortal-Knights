using UnityEngine;
using System.Collections;
using Spine;
using Spine.Unity;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public static ConnectToServer server;
    public static string UserName { get; private set; } // Static variable to store username
    [SerializeField]
    public InputField usernameInput;

    [SerializeField]
    public Text buttonText;

    //[SerializeField]
    //public GameObject backButton;
    private void Awake()
    {
        if (server == null)
        {
            server = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void OnClickConnect()
    {
        string username = usernameInput.text.Trim();
        
        if (!string.IsNullOrEmpty(username))
        {
            UserName = username; // Set the static UserName property
            Invoke("LoadLobbyScene", 1f);
            //PhotonNetwork.NickName = username;
            //buttonText.text = "Connecting...";
            //PhotonNetwork.AutomaticallySyncScene = true;
            //PhotonNetwork.ConnectUsingSettings();
        }
        
    }
    /*
    public override void OnConnectedToMaster()
    {
        //Debug.Log("Connected to master");
        //TypedLobby customLobby = new TypedLobby("MainLobby", LobbyType.Default);
        //PhotonNetwork.JoinLobby(customLobby);
        Invoke("LoadLobbyScene", 1f);

    }
    */
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Photon Network. Cause: " + cause.ToString());
        
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

    public void OnClickBackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void LoadLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }

    void RetryConnection()
    {
        buttonText.text = "Retry";
        // Wait for a few seconds before retrying
        Invoke("ConnectToPhoton", 5f);
    }
    
    void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    
}
