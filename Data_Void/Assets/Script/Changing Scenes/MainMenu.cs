using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button StartGame;
    public Button QuitGame;
    // Start is called before the first frame update
    void Start()
    {
        StartGame.onClick.AddListener(LoadMainMap);
        QuitGame.onClick.AddListener(QuitApplication);
    }

    // Update is called once per frame
    public void LoadMainMap()
    {
        SceneManager.LoadScene("TileTester");
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
