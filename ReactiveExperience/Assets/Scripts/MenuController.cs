using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [SerializeField] private List<GameObject> menuPages; //Contains every page of your main menu

    void Start()
    {
        OpenPage(0); //Makes sure that only the main page is open on startup
    }

    public void StartGame(int level)
    {
        SceneManager.LoadScene(level); // In the button OnClick() section in the inspector you can specify a scene here, 
                                       // the demo scene defaults to to scene 1 (not to be confused with scene 0 which your main menu scene should be)
                                       // You can edit the scene order in File > Build Settings and just drag in the scene assets.
    }

    public void OpenPage(int pageNumber)
    {
        foreach (GameObject page in menuPages) // Disables all the pages so that there is no overlap
        {
            page.SetActive(false);
        }
        menuPages[pageNumber].SetActive(true); // Activates the specified page (as set in the button inspector in the OnClick() event)
    }

    public void QuitGame()
    {
        //Quits the game. The hashtags make it work in editor and in the build. 

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
