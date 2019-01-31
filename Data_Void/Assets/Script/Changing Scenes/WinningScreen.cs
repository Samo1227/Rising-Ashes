using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinningScreen : MonoBehaviour
{
    public Button ReturnToMapButton;
    // Start is called before the first frame update
    void Start()
    {
        ReturnToMapButton.onClick.AddListener(ReturnToMainMap);
    }

    // Update is called once per frame
    public void ReturnToMainMap()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
