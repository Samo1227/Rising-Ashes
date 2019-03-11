using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button StartGame;
    public Button QuitGame;

    void Start()
    {
        StartGame.onClick.AddListener(LoadMainMap);
        QuitGame.onClick.AddListener(QuitApplication);
    }
    
    public void LoadMainMap()
    {
        SceneManager.LoadScene("FungusIntro");
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
