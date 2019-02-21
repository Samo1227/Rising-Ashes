﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerRobot : CharacterBase
{//extends characterbase for convienience, used to control player characters
    //---PR = Player Robot---
    //------------------------------------------
    #region Variables
    public bool bl_Turn_Available = true;//PR can move
    public bool bl_Has_Moved = false;
    public bool bl_Has_Acted = false;
    public int int_Actions = 2;
    public GameObject go_move_ui;//these are the two yellow rectangles that show if the PR has moved or acted
    public GameObject go_other_action;
    #endregion
    //------------------------------------------
    #region BotSetUP
    public int[] int_arr_parts;
    public Transform[] tr_arr_body;
    public string[] st_arr_resources;
    #endregion

    public int[] int_heat_range;
    public bool bl_overheat;
    public GameObject go_heat_bar;
    public int[] int_damage_bracket;
    public int[] int_overheat_damage_bracket;
    public bool bl_Has_Cooldown;

    public int int_head_effect;
    public int int_body_effect;
    public int int_leg_effect;
    public int int_cooldown;

    public PlayerRobot Held_robot;
    
    //------------------------------------------
    #region Start & Update
    private void Start()
    {

        for (int i = 0; i < 4; i++)
        {
            Instantiate(Resources.Load<GameObject>(st_arr_resources[i] + int_arr_parts[i]), new Vector3(tr_arr_body[i].position.x, tr_arr_body[i].position.y, tr_arr_body[i].position.z), Quaternion.identity, tr_arr_body[i]);
        }

        if (int_Weight_Max > int_Weight_Current)
        {
            int_Move_Range = int_Move_Max;
        }
        else
        {
            int_Move_Range = int_Move_Min;
        }

        if(int_body_effect == 2)//freezer
        {
            int_cooldown += 2;
        }
        if (int_body_effect == 3)
        {
            bl_Shield = true;
        }

        CSGameManager.gameManager.ls_Player_Robots_In_Level.Add(this);//adds this PR to the game managers list of alive PRs in the level, this is used by the AIs
        tl_Current_Tile = CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();//keeps a reference of the Tile the PR is on
        tl_Current_Tile.bl_Occupied_By_PC = true;//sets that Tile to be occupied
        CSGameManager.gameManager.PreparePlayerTurn();//sets up the players to take their turn, will need to be reworked if there is an exception where enemies take first turn
        int_Health = int_Health_max;//sets current health to max health
        SetDamage(int_damage); //sets up how much damage that PR can do, really only for testing purposes. Will have to be removed once damage is properly worked out


    }
    //------------------------------------------
    void Update()
    {
        go_health_bar.transform.localPosition = new Vector3(((float)int_Health - (float)int_Health_max) * (0.5f / int_Health_max), 0, 0);
        go_health_bar.transform.localScale = new Vector3((1f / int_Health_max) * int_Health, 0.2f, 1);

        if (CSGameManager.gameManager.pr_currentRobot == this)
        {
            tl_Current_Tile.bl_Current_Tile = true;
        }
        else
            tl_Current_Tile.bl_Current_Tile = false;

       // go_move_ui.SetActive(!bl_Has_Moved);//turns yellow rectangles on and off
       // go_other_action.SetActive(!bl_Has_Acted);
     
        //---------
        if (int_Health <= 0)
        {
            PlayerRobotDeath();
        }
        //---------
        if (bl_Moving)
        {
            MoveToTarget();//makes PR move
        }
        //---------
        if (bl_Turn_Available)
        {
            BehaviourHandle();//whilst this PR has a turn available it can be controlled
        }
        //---------
        if(bl_Has_Acted)
        {
            StartCoroutine(LaserOff());
        }

        ActionSwitch();
        OverheatCheck();
        if (Held_robot != null)
        {
            Held_robot.int_Actions = 0;
        }

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
        if (bl_Turn_Available )//if PR has a turn
        {
            if(CSGameManager.gameManager.pr_currentRobot == null|| CSGameManager.gameManager.pr_currentRobot.int_effect != 4)
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
                        tl_Current_Tile.bl_Current_Tile = false;
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
            }
            //---------
        }
        //---------
    }

    private void OnMouseOver()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (CSGameManager.gameManager.pr_currentRobot != null && CSGameManager.gameManager.pr_currentRobot.int_effect == 4 && Held_robot == null)
        {
            PlayerRobot rob = CSGameManager.gameManager.pr_currentRobot;//gets a reference to the currently selected player robot

            RaycastHit[] hits = new RaycastHit[2];

            Vector3 dir = new Vector3(transform.position.x, 1, transform.position.z) - rob.transform.position;
            dir = dir.normalized;
            float fl_Ray_Range;
            Ray ray_cast;

            fl_Ray_Range = 1;
            ray_cast = new Ray(rob.transform.position, dir * fl_Ray_Range);//draws a ray between target and selected robot
            Debug.DrawRay(rob.transform.position, dir * fl_Ray_Range, Color.green, 0.1f);//visual representation of ray for editor

            if (Input.GetMouseButtonUp(0))//when left mouse button is clicked
            {
                if (Physics.RaycastNonAlloc(ray_cast, hits, fl_Ray_Range) > 0)//if the raycast has hit something
                {
                    if (hits[0].collider.gameObject.GetComponent<PlayerRobot>())//the collider hit is a tile
                    {
                        if (rob.int_effect == 4)
                        {
                            rob.Held_robot = hits[0].collider.gameObject.GetComponent<PlayerRobot>();
                            rob.RandomDamage();
                            rob.bl_Has_Acted = true;//robot has done it's action
                            rob.int_Actions--;
                            rob.int_heat_current += 1;
                            int_Actions = 0;
                            rob.Clear_Selection();//clear tile highlighting 
                            hits[0].collider.gameObject.transform.parent = rob.transform;
                            tl_Current_Tile.bl_Occupied_By_PC = false;
                            GetComponent<Collider>().enabled = false;
                            transform.position = new Vector3(transform.position.x,1.5f,transform.position.z);
                        }
                    }
                }
            }
        }
    }
    #endregion
    //------------------------------------------
    #region Referesh PR
    public void RefreshPCs()//makes it so PR can take it's turn again
    {
        int_Actions = 2;
        bl_Has_Acted = false;
        bl_Has_Moved = false;
        bl_Has_Cooldown = false;
        bl_Turn_Available = true;
        bl_Is_Active = false;
        int_Robot_State = 0;//defaults to finding movement
        if (int_body_effect == 3)
        {
            bl_Shield = true;
        }
    }
    #endregion
    //------------------------------------------
    #region Overheating
    void OverheatCheck()
    {
        //---------
        if (int_heat_current < int_heat_range[0])
        {
            bl_overheat = false;
        }
        //---------
        if (int_heat_current >= int_heat_range[0])
        {
            int temp;
            temp = Random.Range(0,2);
            //---------
            if (temp == 0)
            {
                bl_overheat = false;
            }
            //---------
            else
            {
                bl_overheat = true;
            }
            //---------
        }
        //---------
        if (int_heat_current >= int_heat_range[1])
        {
            bl_overheat = true;
        }
        //---------
        if (int_heat_current > int_heat_total)
        {
            int_heat_current = int_heat_total;
        }
        //---------
        if(int_heat_current < 0)
        {
            int_heat_current = 0;
        }

    }
    #endregion
    //------------------------------------------
    #region Destruction
    public void PlayerRobotDeath()
    {
        CSGameManager.gameManager.ls_Player_Robots_With_Turns_Left.Remove(this);
        CSGameManager.gameManager.ls_Player_Robots_In_Level.Remove(this);//is not an active PR anymore, otherwise AI will break
        CSGameManager.gameManager.CheckLossOrWin();
        tl_Current_Tile.bl_Occupied_By_PC = false;//Tile PR was on is now empty
                                                  //these two could probably be put in an OnDisable method...?
        Destroy(this.gameObject);//PR is destroyed
    }
    #endregion
    //------------------------------------------
    #region ActionPointSwitch
    public void ActionSwitch()
    {
        switch (int_Actions)
        {
            case 2:
                go_move_ui.SetActive(true);
                go_other_action.SetActive(true);
                break;
            case 1:
                go_other_action.SetActive(false);
                break;
            case 0:
                go_move_ui.SetActive(false);
                bl_Turn_Available = false;//ends turn
                //---------
                if (CSGameManager.gameManager.bl_Player_Turn)
                {
                    CSGameManager.gameManager.EndPlayerTurn(this);//updates the GameManager
                }
                //---------
                break;
        }
    }
    #endregion
    //------------------------------------------
    public void RandomDamage()
    {
        if (bl_overheat == false)
        {
            int_damage = Random.Range(int_damage_bracket[0], int_damage_bracket[1]);
        }
        else
        {
            int_damage = Random.Range(int_overheat_damage_bracket[0], int_overheat_damage_bracket[1]);
        }
    }
 
}//=======================================================================================