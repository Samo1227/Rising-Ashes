using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public GameObject im_MenuHolder;
    public GameObject im_HelpScreen;
    // Start is called before the first frame update
    void Start()
    {
        im_MenuHolder = gameObject.transform.GetChild(1).gameObject;
        im_HelpScreen = im_MenuHolder.gameObject.transform.GetChild(4).gameObject;
    }

    public void OpenMenuHolder()
    {
        im_MenuHolder.SetActive(true);
    }
    public void CloseMenuHolder()
    {
        im_MenuHolder.SetActive(false);
    }
    public void OpenHelpScreen()
    {
        im_HelpScreen.SetActive(true);
    }
    public void CloseHelpScreen()
    {
        im_HelpScreen.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Retreat()
    {
        SceneManager.LoadScene("ShipMenu");
    }
}
