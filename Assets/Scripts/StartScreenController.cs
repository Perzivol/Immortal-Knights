using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    [SerializeField]
    Button startButton;

    //LoadingScreen loadingScreen;

    void OnEnable()
    {
        //loadingScreen = null;
        startButton.onClick.AddListener(HandleStartButtonClicked);
        GameState.lives = 3;
        GameState.time = 0;

    }

    void OnDisable()
    {
        startButton.onClick.RemoveListener(HandleStartButtonClicked);
    }
    // later add a scene that is the loading screen different that the current greyed out loading screen
    void HandleStartButtonClicked()
    {
        // call the loading canvas delay x secs and then wait for async to load the background
        
    }
}
