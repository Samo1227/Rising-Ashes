using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobot : CharacterBase {
    public bool bl_Turn_Available = true;
    public bool bl_Has_Moved = false;
    public bool bl_Has_Acted = false;

    public GameObject go_move_ui;
    public GameObject go_other_action;
    private void Start()
    {
        CSGameManager.gameManager.ls_Player_Robots_In_Level.Add(this);
        tl_Current_Tile= CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();
        tl_Current_Tile.bl_Occupied_By_PC = true;
        CSGameManager.gameManager.PreparePlayerTurn();
        int_Health = int_Health_max;
        SetDamage(2);

    }
    void Update()
    {
        go_health_bar.transform.localPosition = new Vector3(((float)int_Health - (float)int_Health_max) * (0.5f / int_Health_max), 0,0);
        go_health_bar.transform.localScale = new Vector3((1f / int_Health_max) * int_Health,0.2f,1);

        go_move_ui.SetActive(!bl_Has_Moved);
        go_other_action.SetActive(!bl_Has_Acted);

        if (int_Health <= 0)
        {
            CSGameManager.gameManager.ls_Player_Robots_In_Level.Remove(this);
            tl_Current_Tile.bl_Occupied_By_PC = false;
            Destroy(this.gameObject);
        }
        if (bl_Moving)
        {

            MoveToTarget();
        }
        if(bl_Has_Acted && bl_Has_Moved)
        {
            //end turn
           // int_Robot_State = 2;//short term turn solution
            bl_Turn_Available = false;
            if (CSGameManager.gameManager.bl_Player_Turn)
            {
                CSGameManager.gameManager.EndPlayerTurn(this);
            }
        }
        if (bl_Turn_Available)
        {
            BehaviourHandle();
        }
    }
    //-----------------------------------
    private void BehaviourHandle()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))//movement
        {
            int_Robot_State = 0;
            Clear_Selection();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))//action
        {
            int_Robot_State = 1;
            Clear_Selection();
        }
        if (bl_Has_Acted)//once acted, only move
        {
            int_Robot_State = 0;
        }
        if (bl_Has_Moved)//once moved, only act. can be changed to give different options, this is just a quick thing
        {
            int_Robot_State = 1;
        }
    }
    //----------------------------------------------
    private void OnMouseUp()
    {
        if (bl_Turn_Available)
        {
            if (!bl_Moving)
            {
                if (int_Robot_State == 0)
                {
                    // Debug.Log("MovePlz");
                    if (CSGameManager.gameManager.pr_currentRobot != null)
                    {
                        CSGameManager.gameManager.pr_currentRobot.Clear_Selection();
                    }
                    Clear_Selection();
                    CSGameManager.gameManager.SetCurrentRobot(this);
                    FindMoveTiles();


                }
                if (int_Robot_State == 1)
                {
                    CSGameManager.gameManager.pr_currentRobot.Clear_Selection();
                    Clear_Selection();
                    CSGameManager.gameManager.SetCurrentRobot(this);
                    Find_Attack_Tile_Range();

                }
            }
        }
    }

    //----------------------------------------------
    public void RefreshPCs()
    {
        bl_Has_Acted = false;
        bl_Has_Moved = false;
        bl_Turn_Available = true;
        bl_Is_Active = false;
        int_Robot_State = 0;
    }
}
