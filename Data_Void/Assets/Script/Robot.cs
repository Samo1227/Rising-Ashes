﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour {//this is an unused prototype class

    public int int_x;
    public int int_z;

    public int int_action_points;
    public int int_action_points_max;

    //Head Stats
    int int_Veiw_Distance;
    int int_Veiw_Type;
    //Body Stats
    public int int_Health;
    public int int_Health_max;

    //Equiptment Stats
    int int_Attack_Range = 3;
    int int_damage;
    int int_heat_current;
    int int_heat_total;
    int int_heat_fail_chance;
    int int_effect;


    //Leg Stats
    public int int_Move_Range = 5;
    int int_Move_Max;
    int int_Move_Min;
    int int_Weight_Current;
    int int_Weight_Max;

    public GameObject go_health_bar;
    public int int_Robot_State = 0;

    public List<Tile> selectableTiles = new List<Tile>();
   
	void Start ()
    {
        int_Health = int_Health_max;


    }
	
	void Update ()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            int_Robot_State = 0;
            Clear_Selection();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            int_Robot_State = 1;
            Clear_Selection();
        }
    }
    //----------------------------------------------
    private void OnMouseUp()
    {
        if (int_Robot_State == 0)
        {
            Clear_Selection();
            FindMoveTiles();
           // CSGameManager.gameManager.SetCurrentRobot(this);


        }
        if (int_Robot_State == 1)
        {
            Clear_Selection();
            Find_Attack_Tile_Range();
           // CSGameManager.gameManager.SetCurrentRobot(this);

        }
    }

    public void FindMoveTiles()
    {
        
        Tile currentTile = CSGameManager.gameManager.map[this.int_x, this.int_z];
        Queue<Tile> process = new Queue<Tile>();
        process.Enqueue(currentTile);
        while (process.Count > 0)
        {
            Tile tempTile = process.Dequeue();
            if (tempTile.bl_Is_Walkable)
            {
                selectableTiles.Add(tempTile);
                tempTile.bl_Walking_Selection = true;
                if (tempTile.int_Distance_From_Start < int_Move_Range)
                {
                    foreach (Tile neighbourTile in tempTile.ls_Tile_Neighbours)
                    {
                        neighbourTile.int_Distance_From_Start = neighbourTile.int_Move_Cost + tempTile.int_Distance_From_Start;
                        if (neighbourTile.int_Distance_From_Start <= int_Move_Range)
                        {
                            process.Enqueue(neighbourTile);
                        }
                    }
                }
            }
        }

    }
    public void Find_Attack_Tile_Range()
    {
        Tile currentTile = CSGameManager.gameManager.map[this.int_x, this.int_z];
        Queue<Tile> process = new Queue<Tile>();
        process.Enqueue(currentTile);
        while (process.Count > 0)
        {
            Tile tempTile = process.Dequeue();
            
                selectableTiles.Add(tempTile);
                tempTile.bl_Attack_Selection = true;
                if (tempTile.int_Distance_From_Start < int_Attack_Range)
                {
                    foreach (Tile neighbourTile in tempTile.ls_Tile_Neighbours)
                    {
                        neighbourTile.int_Distance_From_Start = neighbourTile.int_Move_Cost + tempTile.int_Distance_From_Start;
                        if (neighbourTile.int_Distance_From_Start <= int_Attack_Range)
                        {
                            process.Enqueue(neighbourTile);
                        }
                    }
                }
            
        }
    }

    //--------------------------------------------
    public void Clear_Selection()
    {
        foreach (Tile tempTile in selectableTiles)
        {
            tempTile.bl_Walking_Selection = false;
            tempTile.bl_Attack_Selection = false;
            tempTile.int_Distance_From_Start = 0;
        }
        selectableTiles.Clear();
    }
    //---------------------------------------
}