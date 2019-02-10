using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHelper : MonoBehaviour
{   //---------------------------------------------------
    #region Vars
    public GameObject ui_PR_UI;
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
        }
        //-----------
        if (CSGameManager.gameManager.pr_currentRobot.bl_Has_Acted)
            ui_AttackButton.color = Color.grey;
        //-----------
        if (CSGameManager.gameManager.pr_currentRobot.bl_Has_Moved)
            ui_MoveButton.color = Color.grey;
        //-----------
        if (CSGameManager.gameManager.pr_currentRobot.bl_Has_Acted == false)
            ui_AttackButton.color = Color.white;
        //-----------
        if (CSGameManager.gameManager.pr_currentRobot.bl_Has_Moved == false)
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
            return;
        //-----------
        CSGameManager.gameManager.pr_currentRobot.int_Robot_State = 1;
        CSGameManager.gameManager.pr_currentRobot.Clear_Selection();
        CSGameManager.gameManager.pr_currentRobot.Find_Attack_Tile_Range();
    }
    //---------------------------------------------------
    public void EndPRTurn()
    {
        CSGameManager.gameManager.EndTurnButton();
    } 
    #endregion
    //---------------------------------------------------
}//=======================================================================================
