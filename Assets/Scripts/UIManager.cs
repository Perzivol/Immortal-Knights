using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField]
    private Transform lifeParent1;
    private bool gameHasWinner = false; // New boolean flag to track if game has a winner already
    [SerializeField]
    private Transform lifeParent2;
    public GameObject ControlsScreen;
    [SerializeField]
    private GameObject lifeIconPrefab;
    [SerializeField]
    private Sprite[] lifeIconImages;
    private int maxLives = 5;
    private int player1Lives = 5;
    private int player2Lives = 5;
    public GameObject winscreen;
    private PhotonView photonView; // Declare the photonView variable as a class-level variable
    Player[] allPlayers;
    public Text winnerplayer;
    // Start is called before the first frame update
    void Start()
    {
        InitializeLifeIcons();
        // Get a reference to the PhotonView component of the UIManager object
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView not found on UIManager in Start");
        }
        else
        {
            Debug.Log("PhotonView is initialized on UIManager");
        }
    }

    // Update is called once per frame
    void Awake()
    {
        allPlayers = PhotonNetwork.PlayerList;
        Debug.Log("UIManager Awake Called");
         if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        
    }
    private void Update()
    {
        // This function should be called every frame or in response to an event that might change your UI.
        UpdateLifeIcons();
    }
    private void UpdateLifeIcons()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == 1)
            {
                UpdatePlayerLifeIcons(lifeParent2, player1Lives, player);
            }
            else if (player.ActorNumber == 2)
            {
                UpdatePlayerLifeIcons(lifeParent1, player2Lives, player);
            }
        }
    }
    private void UpdatePlayerLifeIcons(Transform parent, int lives, Player player)
    {
        // Remove all life icons under the parent
        foreach (Transform child in parent)
        {
            //Debug.Log("Removed Life Icon for Player " + player.ActorNumber);
            Destroy(child.gameObject);
        }

        // Instantiate the correct number of life icons based on the lives parameter
        for (int i = 0; i < lives; i++)
        {
            InstantiateLifeIcon(parent, player);
        }
    }
    // Instantiate life icons for both players at the start of the game
    private void InitializeLifeIcons()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            for (int i = 0; i < maxLives; i++)
            {
                if (player.ActorNumber == 1)
                {
                    InstantiateLifeIcon(lifeParent2, player);
                }
                else if (player.ActorNumber == 2)
                {
                    InstantiateLifeIcon(lifeParent1, player);
                }
            }
        }
    }

    // Instantiate a life icon and parent it to the given transform
    private void InstantiateLifeIcon(Transform parent, Player player)
    {
        if (photonView)
        {
            GameObject lifeIcon = Instantiate(lifeIconPrefab, parent);
            // Get the Image component of the life icon
            Image lifeIconImage = lifeIcon.GetComponent<Image>();

            // Set the sprite of the life icon to the appropriate image based on the selected character index
            int selectedCharacter = (int)player.CustomProperties["playerAvatar"];
            if (selectedCharacter >= 0 && selectedCharacter < lifeIconImages.Length)
            {
                lifeIconImage.sprite = lifeIconImages[selectedCharacter];
            }
        }
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(player1Lives);
            stream.SendNext(player2Lives);
        }
        else
        {
            // Network player, receive data
            this.player1Lives = (int)stream.ReceiveNext();
            this.player2Lives = (int)stream.ReceiveNext();
        }
    }
    
    // Decrease the player's lives and update the UI
    [PunRPC]
    public void DecreaseLives(int player)
    {
        Debug.Log("DecreaseLives called on client with PhotonView IsMine = " + photonView.IsMine + " for player " + player);

        if (player == 1 && player1Lives > 0)
        {
            Debug.Log("Removed Player 1 Life");
            player1Lives--;
        }
        else if (player == 2 && player2Lives > 0)
        {
            Debug.Log("Removed Player 2 Life");
            player2Lives--;
        }
        
        CheckForWinner();
        
        
    }

    private void CheckForWinner()
    {
        // If game has a winner, we don’t need to check again
        if (gameHasWinner) return;

        if (player1Lives < 1)
        {
            winnerplayer.text = "Winner: " + allPlayers[1].NickName;
            Debug.Log("Player 2 Wins");
            winscreen.SetActive(true); 
            gameHasWinner = true; // Set the flag to true when we have a winner
        }
        else if (player2Lives < 1)
        {
            winnerplayer.text =  "Winner: " + allPlayers[0].NickName;
            Debug.Log("Player 1 Wins");
            winscreen.SetActive(true); 
            gameHasWinner = true; // Set the flag to true when we have a winner
        }
    }

    // Destroy a life icon from the given transform
    private void DestroyLifeIcon(Transform parent)
    {
        if (parent.childCount > 0)
        {
            Debug.Log("Destroyed Life Icon");
            Destroy(parent.GetChild(parent.childCount - 1).gameObject);
        }
    }

    public void ControlsScreenActivate()
    {
        if(photonView.IsMine)
        {
            ControlsScreen.SetActive(true);
        }
        
    }

    public void ControlsScreenDeactivate()
    {
        if(photonView.IsMine)
        {
            ControlsScreen.SetActive(false);
        }
    }
}
