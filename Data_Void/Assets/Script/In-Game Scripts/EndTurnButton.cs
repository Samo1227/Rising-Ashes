using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    public Image self;
    public Color mainCol;
    public GameObject confirmationPanel;

    void Start()
    {
        self = gameObject.GetComponent<Image>();
        mainCol = self.color;
        confirmationPanel = gameObject.transform.GetChild(1).gameObject;
    }
    
    void Update()
    {
        if (CSGameManager.gameManager.bl_Player_Turn&&CSGameManager.gameManager.bl_MissionStarted)
        {
            self.color = mainCol;
            gameObject.GetComponent<Button>().interactable = true;
        }
        else
        {
            self.color = Color.grey;
            gameObject.GetComponent<Button>().interactable = false;
        }
    }

    public void OpenPanel()
    {
        confirmationPanel.SetActive(true);
        if (CSGameManager.gameManager.pr_currentRobot != null)
        {
            CSGameManager.gameManager.pr_currentRobot.Clear_Selection();
            CSGameManager.gameManager.pr_currentRobot = null;
        }
    }
    public void ClosePanel()
    {
        confirmationPanel.SetActive(false);
    }
    public void EndTurn()
    {
        CSGameManager.gameManager.EndPlayerTurn();
        confirmationPanel.SetActive(false);
    }
}
