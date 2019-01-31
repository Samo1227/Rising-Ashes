using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobot : CharacterBase {//extends characterbase for convienience, used to control player characters
    //---PR = Player Robot---
    //------------------------------------------
    #region Variables
    public bool bl_Turn_Available = true;//PR can move
    public bool bl_Has_Moved = false;
    public bool bl_Has_Acted = false;

    public GameObject go_move_ui;//these are the two yellow rectangles that show if the PR has moved or acted
    public GameObject go_other_action;
    #endregion
    //------------------------------------------
    #region Start & Update
    private void Start()
    {
<<<<<<< HEAD
=======

        for (int i = 0; i < 4; i++)
        {
            Instantiate(Resources.Load<GameObject>(st_arr_resources[i] + int_arr_parts[i]), new Vector3(tr_arr_body[i].position.x, tr_arr_body[i].position.y, tr_arr_body[i].position.z), Quaternion.identity, tr_arr_body[i]);
        }

>>>>>>> parent of 0624653... Basic Weight Function Added
        CSGameManager.gameManager.ls_Player_Robots_In_Level.Add(this);//adds this PR to the game managers list of alive PRs in the level, this is used by the AIs
        tl_Current_Tile= CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();//keeps a reference of the Tile the PR is on
        tl_Current_Tile.bl_Occupied_By_PC = true;//sets that Tile to be occupied
        CSGameManager.gameManager.PreparePlayerTurn();//sets up the players to take their turn, will need to be reworked if there is an exception where enemies take first turn
        int_Health = int_Health_max;//sets current health to max health
        SetDamage(2); //sets up how much damage that PR can do, really only for testing purposes. Will have to be removed once damage is properly worked out
<<<<<<< HEAD
=======


>>>>>>> parent of 0624653... Basic Weight Function Added
    }

    //------------------------------------------

    void Update()
    {
        go_health_bar.transform.localPosition = new Vector3(((float)int_Health - (float)int_Health_max) * (0.5f / int_Health_max), 0,0);
        go_health_bar.transform.localScale = new Vector3((1f / int_Health_max) * int_Health,0.2f,1);

        go_move_ui.SetActive(!bl_Has_Moved);//turns yellow rectangles on and off
        go_other_action.SetActive(!bl_Has_Acted);
        //---------
        if (int_Health <= 0)
        {
            CSGameManager.gameManager.ls_Player_Robots_In_Level.Remove(this);//is not an active PR anymore, otherwise AI will break
            CSGameManager.gameManager.CheckLossOrWin();
            tl_Current_Tile.bl_Occupied_By_PC = false;//Tile PR was on is now empty
            //these two could probably be put in an OnDisable method...?
            Destroy(this.gameObject);//PR is destroyed
        }
        //---------
        if (bl_Moving)
        {
            MoveToTarget();//makes PR move
        }
        //---------
        if (bl_Has_Acted && bl_Has_Moved)
        {
            bl_Turn_Available = false;//ends turn
            //---------
            if (CSGameManager.gameManager.bl_Player_Turn)
            {
                CSGameManager.gameManager.EndPlayerTurn(this);//updates the GameManager
            }
            //---------
        }
        //---------
        if (bl_Turn_Available)
        {
            BehaviourHandle();//whilst this PR has a turn available it can be controlled
        }
        //---------
    }
    #endregion
    //------------------------------------------
    #region PR Behaviour
    private void BehaviourHandle()//these could probably be controlled more easily through a switch case
    {
        //---------
        if (Input.GetKeyDown(KeyCode.Alpha1))//movement
        {
            int_Robot_State = 0;
            Clear_Selection();//gets rid of tile highlighting
        }
        //---------
        if (Input.GetKeyDown(KeyCode.Alpha2))//action
        {
            int_Robot_State = 1;
            Clear_Selection();//gets rid of tile highlighting
        }
        //---------
        if (bl_Has_Acted)//once acted, only move
        {
            int_Robot_State = 0;
        }
        //---------
        if (bl_Has_Moved)//once moved, only act. can be changed to give different options, this is just a quick thing
        {
            int_Robot_State = 1;
        }
        //---------
    }
    #endregion
    //------------------------------------------
    #region Clicking
    private void OnMouseUp()
    {
        //---------
        if (bl_Turn_Available)//if PR has a turn
        {
            //---------
            if (!bl_Moving)//when it's not moving it can be clicked on
            {
                //---------
                if (int_Robot_State == 0)//movement state
                {
                    //---------
                    if (CSGameManager.gameManager.pr_currentRobot != null)//the clear is needed but will always be null the first time a PR is clicked on, thus this is neccessary
                    {
                        CSGameManager.gameManager.pr_currentRobot.Clear_Selection();//clears the selection of the previously selected robot
                    }
                    //---------
                    Clear_Selection();//clears this PR's selection just to be safe
                    CSGameManager.gameManager.SetCurrentRobot(this);//set the gamemanagers reference of currently selected PR to this
                    FindMoveTiles();//works out PR's movement bounds
                }
                //---------
                if (int_Robot_State == 1)//action state
                {
                    //---------
                    if (CSGameManager.gameManager.pr_currentRobot != null)//the clear is needed but will always be null the first time a PR is clicked on, thus this is neccessary
                    {
                        CSGameManager.gameManager.pr_currentRobot.Clear_Selection();//clears the selection of the previously selected robot
                    }
                    //---------
                    Clear_Selection();//clears this PR's selection just to be safe
                    CSGameManager.gameManager.SetCurrentRobot(this);//set the gamemanagers reference of currently selected PR to this
                    Find_Attack_Tile_Range();//works out PR's movement bounds
                }
                //---------
            }
            //---------
        }
        //---------
    }
    #endregion
    //------------------------------------------
    #region Referesh PR
    public void RefreshPCs()//makes it so PR can take it's turn again
    {
        bl_Has_Acted = false;
        bl_Has_Moved = false;
        bl_Turn_Available = true;
        bl_Is_Active = false;
        int_Robot_State = 0;//defaults to finding movement
    }
    #endregion
    //------------------------------------------
}//=======================================================================================
