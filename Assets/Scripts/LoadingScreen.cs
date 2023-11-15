using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;



public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject spinner;

    [SerializeField]
    private GameObject scrim;

    [SerializeField]
    private GameObject text;

    [SerializeField]
    public GameObject LoadingCanvas;

    [SerializeField]
    private RectTransform rectComponent;

    private float turnSpeed = 100f;
    

    //public Image LoadingBarFill;

    void Update()
    {
        if(rectComponent != null)
        {
            rectComponent.Rotate(0, 0, -turnSpeed * Time.fixedDeltaTime);
        }

    }

    public void ShowSpinner()
    {
        spinner.SetActive(true);
        scrim.SetActive(true);
        text.SetActive(true);
    }

    public void HideSpinner()
    {
        spinner.SetActive(false);
        scrim.SetActive(false);
        text.SetActive(false);
    }
    

    public void LoadScene(int sceneID)
    {
        StartCoroutine(LoadSceneAsync(sceneID));
    }

    IEnumerator LoadSceneAsync(int sceneID)
    {
        //triggers load scene first
        LoadingCanvas.SetActive(true);
        Debug.Log("test");
        ShowSpinner(); 
        //delays several seconds
        Debug.Log("Started Coroutine at timestamp : " + Time.time);
        yield return new WaitForSeconds(2);
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        //loads next scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        while (!operation.isDone)
        {                         
            yield return null;        
        }
       
        
        HideSpinner();
        
    }
}
