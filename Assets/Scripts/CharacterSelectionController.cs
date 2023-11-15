using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class CharacterSelectionController : MonoBehaviour
{
    //public GameObject knight;
    //public GameObject assassin;
    public GameObject[] skins;
    GameObject tempchar;
    int selectedCharacter = 0;
    List<GameObject> activeCharacters = new List<GameObject>();

    public Button nextButton;
    public Button previousButton;
    void Start()
    {
        nextButton.onClick.AddListener(ChangeNext);
        previousButton.onClick.AddListener(ChangePrevious);
    }
    void Awake()
    {
        skins = new GameObject[]{
            GameObject.Find("Knight"),
            GameObject.Find("Assassin"),
        };
        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i]!=null && skins[i].activeInHierarchy)
            {
                activeCharacters.Add(skins[i]);
                
            }
        }
        //selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 1);
        //Debug.Log(selectedCharacter);
        //foreach(GameObject player in skins)
        //    player.SetActive(false);

        ShowCharacter(selectedCharacter);
    }

    void ShowCharacter(int index)
    {
        for (int i = 0; i < activeCharacters.Count; i++)
        {
            if(i == index) 
                activeCharacters[i].SetActive(true);
            else
                activeCharacters[i].SetActive(false);
        }
        if (skins[index] != null)
        {
            skins[index].SetActive(true);
        }
        else
        {
            Debug.LogError("Element at index " + index + " in skins array is null");
        }
    }
    /*
    void ShowCharacter(int index)
    {
        // hide all characters
        for (int i = 0; i < skins.Length; i++)
        {
            Debug.Log("i =");
            Debug.Log(i);
            
            Debug.Log("skins length = ");
            Debug.Log(skins.Length);

            Debug.Log("skins i= ");
            Debug.Log(skins[i]);
            if(i == index){ 
                activeCharacters[i].SetActive(true);
            }    
            else{
                activeCharacters[i].SetActive(false);
            }
        };
        
        // show selected character
        
        skins[index].SetActive(true);
    }
   
    public void ChangeNext()
    {
        //skins[selectedCharacter].SetActive(false);
        if(selectedCharacter > skins.Length - 1)
        {            
            selectedCharacter = 0;
        }
        else
        {
            selectedCharacter++;
        }
        // show next Character
        ShowCharacter(selectedCharacter);       
        
        //skins[selectedCharacter].SetActive(true);
        //PlayerPrefs.SetInt("SelectedCharacter", selectedCharacter);
    }

    public void ChangePrevious()
    {
        //skins[selectedCharacter].SetActive(false);
        
        if(selectedCharacter < 0)
        {
            selectedCharacter = skins.Length - 1;
        }
        else{
            selectedCharacter--;
        }

        // show previous character
        ShowCharacter(selectedCharacter);  
        //skins[selectedCharacter].SetActive(true);
        //PlayerPrefs.SetInt("SelectedCharacter", selectedCharacter);
    }
    */
    public void ChangeNext()
    {
        selectedCharacter++;
        if(selectedCharacter >= skins.Length)
        {            
            selectedCharacter = 0;
        }
        ShowCharacter(selectedCharacter);       
    }

    public void ChangePrevious()
    {
        selectedCharacter--;
        if(selectedCharacter < 0)
        {
            selectedCharacter = skins.Length - 1;
        }
        ShowCharacter(selectedCharacter);  
    }
}
