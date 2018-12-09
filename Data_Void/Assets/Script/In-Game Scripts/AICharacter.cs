using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacter : CharacterBase {

    public PlayerRobot pr_Target; //character the AI will move towards and attack
    public Renderer rnd_Rendereer;
    //--------------------------------
    void Start()
    {
        int_Health = int_Health_max;
        SetAttackRange(1);
        SetDamage(1);
        CSGameManager.gameManager.ls_AI_Characters_In_Level.Add(this);
        rnd_Rendereer = gameObject.transform.GetComponent<Renderer>();
        tl_Current_Tile = CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();
        tl_Current_Tile.bl_Occupied_By_AI = true;
    }
    //--------------------------------
    void Update()
    {
        if (int_Health <= 0)
        {
            CSGameManager.gameManager.ls_AI_Characters_In_Level.Remove(this);
            tl_Current_Tile.bl_Occupied_By_AI = false;
            Destroy(this.gameObject);
        }

        if (fl_Turn_Timer>= fl_Time_limit)
        {
            CSGameManager.gameManager.EndAITurn();
        }
        if (bl_Is_Active)
        {
            TakeTurn();
        }

        go_health_bar.transform.localPosition = new Vector3(((float)int_Health - (float)int_Health_max) * (0.5f / int_Health_max), 0, 0);
        go_health_bar.transform.localScale = new Vector3((1f / int_Health_max) * int_Health, 0.2f, 1);

    }

    void TakeTurn()
    {
        if (bl_Turn_Just_Started)
        {
            pr_Target = null; //rework out target at the start of each turn
            bl_Turn_Just_Started = false;
        }
        if (!bl_Moving)//don't keep checking for a target when moving
        {
            tl_Current_Tile.bl_Occupied_By_AI = false;
            FindNearestPlayerRobot();
            CalculatePath();//was working fine but now gets stuck -_-
            FindMoveTilesAI();//in theory should stop them from moving through players and stopping on other AI... doesn't seem to work though
            if (tl_Target_Square)
            {
                bl_Moving = true;
            }
            else//this doesn't get called for some reason... wait of course it won't! I'm an idiot
            {   //this is what happens if there is no path to target square I guess...
                //attack and end turn
                Clear_Selection();//putting this everywhere now :S
                int_x = (int)transform.position.x;
                int_z = (int)transform.position.z;
                tl_Current_Tile = CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();
                tl_Current_Tile.bl_Occupied_By_AI = true;
                Find_Attack_Tile_Range();
                FindPRsInRange();
                Clear_Selection();
                bl_Is_Active = false;
                CSGameManager.gameManager.EndAITurn();
            }

        }
        else
        {
            MoveToTarget();
            if (!bl_Moving)
            {
                Clear_Selection();//putting this everywhere now :S
                int_x = (int)transform.position.x;
                int_z = (int)transform.position.z;
                tl_Current_Tile = CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();
                tl_Current_Tile.bl_Occupied_By_AI = true;
                Find_Attack_Tile_Range();
                FindPRsInRange();
                Clear_Selection();
                pr_Target = null;
                bl_Is_Active = false;
                CSGameManager.gameManager.EndAITurn();
            }
        }

    }

    //--------------------------------
    void FindNearestPlayerRobot()//this is a vary quick and simple way to find the nearest target, it's lacking in versatility and elegance
    {
        PlayerRobot pr_Nearest = null;
        float fl_Distance_To_Current_Target = Mathf.Infinity;
        foreach(PlayerRobot tPR in CSGameManager.gameManager.ls_Player_Robots_In_Level)
        {
            float fl_Distance_To_Target = Vector3.Distance(transform.position, tPR.transform.position); //inefficient way to check distance to all targets, also ignores obsticles

            if(fl_Distance_To_Target < fl_Distance_To_Current_Target) //if distance to the target it's looking at is lower that distance(to current target) 
            {
                fl_Distance_To_Current_Target = fl_Distance_To_Target;
                pr_Nearest = tPR; //nearest Player Robot = what it's looking at
            }
        }
        pr_Target = pr_Nearest; //once its looked at all PRs the closest one is target, long term needs to be different as there may be no path to closest target
    }

    void CalculatePath()
    {
        Tile tl_Target_Tile = CSGameManager.gameManager.map[pr_Target.int_x, pr_Target.int_z].gameObject.GetComponent<Tile>();
        Tile tl_Current_Tile = CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();
        PathFinding(tl_Current_Tile, tl_Target_Tile);
    }



    private void OnMouseOver()//this doesn't work for shooting enemies
    {
        if ( tl_Current_Tile.bl_Attack_Selection ==true)
        {
            PlayerRobot rob = CSGameManager.gameManager.pr_currentRobot;
            rnd_Rendereer.material.color = Color.yellow;

            RaycastHit hit;

            Vector3 dir = new Vector3(transform.position.x, 1, transform.position.z) - rob.transform.position;
            dir = dir.normalized;
            float fl_Ray_Range = Vector3.Distance(new Vector3(transform.position.x, 1, transform.position.z), rob.transform.position);

            Ray ray_cast = new Ray(rob.transform.position, dir * fl_Ray_Range);

            Debug.DrawRay(rob.transform.position, dir * fl_Ray_Range, Color.green, 0.1f);

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray_cast, out hit, fl_Ray_Range))
                {

                    if (hit.collider.gameObject.GetComponent<AICharacter>())
                    {
                        rob.bl_Has_Acted = true;

                        rnd_Rendereer.material.color = Color.red;
                        rob.Clear_Selection();
                        hit.collider.gameObject.GetComponent<AICharacter>().AttackTarget(rob, this);
                    }

                }
            }
            //rob.Clear_Selection();
        }
    }
    private void OnMouseExit()
    {
        if (tl_Current_Tile.bl_Attack_Selection == true)
        {
            rnd_Rendereer.material.color = Color.red;
        }


    }
}
