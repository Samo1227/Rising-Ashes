using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    public Image self;
    public Color mainCol;


    void Start()
    {
        self = gameObject.GetComponent<Image>();
        mainCol = self.color;
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

    public void EndTurn()
    {
        CSGameManager.gameManager.EndPlayerTurn();
    }
}
