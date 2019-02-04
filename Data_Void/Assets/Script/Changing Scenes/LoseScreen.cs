using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoseScreen : MonoBehaviour
{
    public Button RestartButton;
    public Button ReturnToMapButton;
    // Start is called before the first frame update
    void Start()
    {
        RestartButton.onClick.AddListener(RestartLevel);
        ReturnToMapButton.onClick.AddListener(ReturnToMainMap);
    }

    // Update is called once per frame
    public void RestartLevel()
    {
        SceneManager.LoadScene("TileTester");
    }

    public void ReturnToMainMap()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
