using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StartScreenManager : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("ConnectToServer"); 
    }
}
