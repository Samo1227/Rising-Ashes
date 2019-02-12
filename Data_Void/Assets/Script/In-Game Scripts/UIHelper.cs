using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHelper : MonoBehaviour
{   //---------------------------------------------------
    #region Vars
    public GameObject ui_PR_UI;
    public GameObject go_heat_bar;
    public Image ui_MoveButton;
    public Image ui_AttackButton;

    #endregion
    //---------------------------------------------------
    #region Update
    private void Update()
    {
        //-----------
        if (CSGameManager.gameManager.pr_currentRobot == null)
        {
            ui_PR_UI.SetActive(false);
            return;
        }
        //-----------
        if (CSGameManager.gameManager.pr_currentRobot != null)
        {
            
            ui_PR_UI.SetActive(true);

            go_heat_bar.transform.localScale = new Vector3((1f / CSGameManager.gameManager.pr_currentRobot.int_heat_total) * CSGameManager.gameManager.pr_currentRobot.int_heat_current, 0.2f, 1);

            if(CSGameManager.gameManager.pr_currentRobot.int_heat_current < CSGameManager.gameManager.pr_currentRobot.int_heat_range[0])
            {
                go_heat_bar.GetComponent<Image>().color = Color.green;
            }
            if(CSGameManager.gameManager.pr_currentRobot.int_heat_current >= CSGameManager.gameManager.pr_currentRobot.int_heat_range[0])
            {
                go_heat_bar.GetComponent<Image>().color = Color.yellow;
            }
            if(CSGameManager.gameManager.pr_currentRobot.int_heat_current >= CSGameManager.gameManager.pr_currentRobot.int_heat_range[1])
            {
                go_heat_bar.GetComponent<Image>().color = Color.red;
            }
            
        }
        //-----------
        if (CSGameManager.gameManager.pr_currentRobot.bl_Has_Acted)
            ui_AttackButton.color = Color.grey;
        //-----------
        if (CSGameManager.gameManager.pr_currentRobot.bl_Has_Moved)
            ui_MoveButton.color = Color.grey;
        //-----------
        if (CSGameManager.gameManager.pr_currentRobot.bl_Has_Cooldown)
            ui_MoveButton.color = Color.grey;
        //-----------
        if (CSGameManager.gameManager.pr_currentRobot.bl_Has_Acted == false)
            ui_AttackButton.color = Color.white;
        //-----------
        if (CSGameManager.gameManager.pr_currentRobot.bl_Has_Moved == false)
            ui_MoveButton.color = Color.white;
        //-----------
        if (CSGameManager.gameManager.pr_currentRobot.bl_Has_Cooldown == false)
            ui_MoveButton.color = Color.white;
        //-----------
    }
    #endregion
    //---------------------------------------------------
    #region Buttons
    public void MoveButton()
    {
        //-----------
        if (CSGameManager.gameManager.pr_currentRobot.bl_Has_Moved)
            return;
        //-----------
        CSGameManager.gameManager.pr_currentRobot.int_Robot_State = 0;
        CSGameManager.gameManager.pr_currentRobot.Clear_Selection();
        CSGameManager.gameManager.pr_currentRobot.FindMoveTiles();
    }
    //---------------------------------------------------
    public void AttackButton()
    {
        //-----------
        if (CSGameManager.gameManager.pr_currentRobot.bl_Has_Acted)
        {
            return;
        }
        //-----------
        CSGameManager.gameManager.pr_currentRobot.int_Robot_State = 1;
        CSGameManager.gameManager.pr_currentRobot.Clear_Selection();
        CSGameManager.gameManager.pr_currentRobot.Find_Attack_Tile_Range();
    }
    //---------------------------------------------------
    public void CooldownButton()
    {
        if (CSGameManager.gameManager.pr_currentRobot.bl_Has_Cooldown)
            return;
        if(CSGameManager.gameManager.pr_currentRobot.int_heat_current > 0)
        {
            CSGameManager.gameManager.pr_currentRobot.int_heat_current -= CSGameManager.gameManager.pr_currentRobot.int_cooldown;
            CSGameManager.gameManager.pr_currentRobot.bl_Has_Cooldown = true;
            CSGameManager.gameManager.pr_currentRobot.int_Actions--;
        }
        else
        {
            return;
        }


    }

    public void EndPRTurn()
    {
        CSGameManager.gameManager.pr_currentRobot.int_Actions = 0;
        CSGameManager.gameManager.EndTurnButton();
    } 
    #endregion
    //---------------------------------------------------
}//=======================================================================================
