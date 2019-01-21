using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour { 
    public int int_x;
    public int int_z;

    public int int_action_points;
    public int int_action_points_max;

    //the stats are not finalised, this is mainly just to get an idea for what is needed

    //Head Stats
    int int_Veiw_Distance;
    int int_Veiw_Type;
    //Body Stats
    public int int_Health;
    public int int_Health_max;
    //gimmie gimmie gimmie a honk after midnight ♥

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

    public GameObject go_health_bar;//used to display robots health
    public int int_Robot_State = 0;

    public bool bl_Is_Active = false; 
    public bool bl_Moving = false;          //for moving to target pos
    public float fl_Move_Speed = 5f; //how quick it moves to target square
    Vector3 v3_Velocity = new Vector3();//used for visually moving to squares
    Vector3 v3_Heading = new Vector3();//used for visually moving to squares
    public Tile tl_Target_Square;
    Stack<Tile> st_Path = new Stack<Tile>();//used to generate a path that is followed for movement
    public Tile tl_Current_Tile;
    //AI variables
    public float fl_Time_limit = 5f; //For the AI, incase it gets stuck in its turn
    protected float fl_Turn_Timer; //Timer
    public bool bl_Turn_Just_Started = false; //for the start of AI turn
    protected List<PlayerRobot> ls_PRs_In_Range = new List<PlayerRobot>(); //to store attack targets
    //---------------

    public List<Tile> selectableTiles = new List<Tile>();//a list of selectable tiles 

    //---------------------------------------------------

    void Start()
    {
        int_Health = int_Health_max;//current health = max health at start

     
    }
    
    //---------------------------------------------------

    public void FindMoveTiles()
    {

        Tile currentTile = CSGameManager.gameManager.map[this.int_x, this.int_z];
       // Debug.Log(currentTile.transform.position.x + " " + currentTile.transform.position.z);
        Queue<Tile> process = new Queue<Tile>();
        process.Enqueue(currentTile);

        while (process.Count > 0)
        {
            Tile tempTile = process.Dequeue(); //takes the tile out of the queue and processes it
            if (tempTile.bl_Is_Walkable)   //if the tile is walkable add it to the selectable process
            {
                if (!tempTile.bl_Occupied_By_PC)
                {
                    selectableTiles.Add(tempTile);
                    tempTile.bl_Walking_Selection = true;
                }

                if (tempTile.int_Distance_From_Start < int_Move_Range) // if it's within move range -1 check the neighbours
                {
                    foreach (Tile neighbourTile in tempTile.ls_Tile_Neighbours)
                    {
                        if (neighbourTile != currentTile)//stops path getting stuck looping
                        {
                            neighbourTile.tl_Start_Tile = tempTile;
                        }
                        neighbourTile.int_Distance_From_Start = neighbourTile.int_Move_Cost + tempTile.int_Distance_From_Start;
                        if (neighbourTile.int_Distance_From_Start <= int_Move_Range)//if the neighbours are within movement range add them to the process queue
                        {
                            if (!neighbourTile.bl_Occupied_By_AI)
                            {
                                process.Enqueue(neighbourTile);
                            }
                        }
                    }
                }
            }
        }

    }

    //---------------------------------------------------

    public void FindMoveTilesAI()
    {

        Tile currentTile = CSGameManager.gameManager.map[this.int_x, this.int_z];
        Queue<Tile> process = new Queue<Tile>();
        process.Enqueue(currentTile);

        while (process.Count > 0)
        {
            Tile tempTile = process.Dequeue(); //takes the tile out of the queue and processes it
            if (tempTile.bl_Is_Walkable)   //if the tile is walkable add it to the selectable process
            {
                if (!tempTile.bl_Occupied_By_AI)//this should mean it can't move to a square with another AI but it still does?????
                {
                    selectableTiles.Add(tempTile);
                    tempTile.bl_Walking_Selection = true;
                }

                if (tempTile.int_Distance_From_Start < int_Move_Range) // if it's within move range -1 check the neighbours
                {
                    foreach (Tile neighbourTile in tempTile.ls_Tile_Neighbours)
                    {
                        if (neighbourTile != currentTile)
                        {
                            neighbourTile.tl_Start_Tile = tempTile;
                        }
                        neighbourTile.int_Distance_From_Start = neighbourTile.int_Move_Cost + tempTile.int_Distance_From_Start;
                        if (neighbourTile.int_Distance_From_Start <= int_Move_Range)//if the neighbours are within movement range add them to the process queue
                        {
                            if (!neighbourTile.bl_Occupied_By_PC)
                            {
                                process.Enqueue(neighbourTile);
                            }
                        }
                    }
                }
            }
        }

    }

    //---------------------------------------------------

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

    //---------------------------------------------------
    public void Clear_Selection()
    {
       // Debug.Log("clear");
       foreach(Tile tl_Temp_Tile in CSGameManager.gameManager.map)
        {
            tl_Temp_Tile.bl_Walking_Selection = false;
            tl_Temp_Tile.bl_Attack_Selection = false;
            tl_Temp_Tile.int_Distance_From_Start = 0;
            tl_Temp_Tile.tl_Start_Tile = null;
        }
        //foreach (Tile tempTile in selectableTiles)
        //{
        //   // Debug.Log("clear " + tempTile.transform.position.x + " " + tempTile.transform.position.z);
        //    tempTile.bl_Walking_Selection = false;
        //    tempTile.bl_Attack_Selection = false;
        //    tempTile.int_Distance_From_Start = 0;
        //    tempTile.tl_Start_Tile = null;
        //}
        selectableTiles.Clear();
    }

    //---------------------------------------------------

    public void MoveToTarget()
    {
        if(st_Path.Count > 0)
        {
            Tile tl_temp = st_Path.Peek();
            Vector3 v3_Target = tl_temp.transform.position;
            v3_Target.y += 1; //so character doesn't move into the ground

            if(Vector3.Distance(transform.position,v3_Target)>= 0.05f)
            {
                CalculateHeading(v3_Target);
                SetVelocity();

                transform.right = v3_Heading;//faces the direction it's moving in
                transform.position += v3_Velocity * Time.deltaTime;
            }
            else
            {
                transform.position = v3_Target;
                st_Path.Pop();
            }
        }
        else
        {
            Clear_Selection();
            bl_Moving = false;
        }
    
    }

    //---------------------------------------------------

    void CalculateHeading(Vector3 v3_Temp_Target)
    {
        v3_Heading = v3_Temp_Target - transform.position;
        v3_Heading.Normalize();
    }

    //---------------------------------------------------

    void SetVelocity()
    {
        v3_Velocity = v3_Heading * fl_Move_Speed;
    }

    //---------------------------------------------------

    protected void PathFinding(Tile current_Tile, Tile target_Tile)//A* pathfinding journey ^_^
    {
        List<Tile> ls_Open_List = new List<Tile>();
        List<Tile> ls_Closed_List = new List<Tile>();

        ls_Open_List.Add(current_Tile);
            if (Vector3.Distance(current_Tile.transform.position, target_Tile.transform.position) != 0) //if target is not this tile, so AI doesn't move onto the PC square
            {
            current_Tile.h = (Vector3.Distance(current_Tile.transform.position, target_Tile.transform.position));//doing this through the array would be better probably
                current_Tile.f = current_Tile.h;
                while (ls_Open_List.Count > 0)
                {
                Tile tile_Check = FindTileWithLowestFCost(ls_Open_List);
                    ls_Closed_List.Add(tile_Check);

                    if (tile_Check == target_Tile || tile_Check == null) //arrived at target tile or no path
                    {
                        tl_Target_Square = FindEndingTile(tile_Check);
                    if (tl_Target_Square.bl_Occupied_By_AI) //prevents AI from moving into an ocupied tile
                    {
                        tl_Target_Square = tl_Target_Square.tl_Start_Tile;
                    }
                        MoveToTargetSquare(tl_Target_Square);
                        return;
                    }
                    foreach (Tile tl_Tile in tile_Check.ls_Tile_Neighbours) //for each neighboring tile
                    {
                    if (ls_Closed_List.Contains(tl_Tile))//already checked
                        {
                            //do nothing
                        }
                        else if (ls_Open_List.Contains(tl_Tile))
                        {
                        float fl_Temp_G_Cost = tile_Check.g + Vector3.Distance(tl_Tile.transform.position, tile_Check.transform.position);

                            if (fl_Temp_G_Cost < tl_Tile.g)
                            {
                                tl_Tile.tl_Start_Tile = tile_Check;
                                tl_Tile.g = fl_Temp_G_Cost;
                                tl_Tile.f = tl_Tile.g + tl_Tile.h;
                            }
                        }
                        else
                        {
                        tl_Tile.tl_Start_Tile = tile_Check;
                            tl_Tile.g = tile_Check.g + Vector3.Distance(tl_Tile.transform.position, target_Tile.transform.position); //research how to check manhattan distance, as this is a messy way of doing this
                            tl_Tile.h = Vector3.Distance(tl_Tile.transform.position, target_Tile.transform.position);
                            tl_Tile.f = tl_Tile.h + tl_Tile.g;
                            if (tl_Tile.bl_Is_Walkable)
                            {
                                //if (!tl_Tile.bl_Occupied_By_AI)//won't go into a space with a AI in it... also means it won't walk through AI though
                                {
                                    ls_Open_List.Add(tl_Tile);
                                }
                            }
                        }
                    }
                }
            }
        
    }

    //---------------------------------------------------

    protected Tile FindTileWithLowestFCost(List<Tile> list)//similar to finding closest target, but with Tiles 
    {
        Tile til_Lowest_F_Cost = list[0];

        foreach(Tile t in list)
        {
            if (t.f < til_Lowest_F_Cost.f)
            {
                til_Lowest_F_Cost = t;
            }
        }
        list.Remove(til_Lowest_F_Cost);
        return til_Lowest_F_Cost;
    }

    //---------------------------------------------------

    protected Tile FindEndingTile(Tile tile)
    {
        Stack<Tile> st_Temp_Path = new Stack<Tile>();
        Tile next = tile.tl_Start_Tile;

      
        while (next != null) //infinitely looping...or not?
        {
            st_Temp_Path.Push(next);            
            next = next.tl_Start_Tile;
        }

        if(st_Temp_Path.Count <= int_Move_Range)
        {
            return tile.tl_Start_Tile;
        }

        Tile tl_End_Tile = null;
        for(int i = 0; i <= int_Move_Range; i++)
        {
            tl_End_Tile = st_Temp_Path.Pop();
        }
        if (tl_End_Tile.bl_Occupied_By_AI)
        {
            tl_End_Tile = tl_End_Tile.tl_Start_Tile;
        }
        return tl_End_Tile;
    }

    //---------------------------------------------------

    public void MoveToTargetSquare(Tile tl_Target)//builds a path into a stack for the AI to follow
    {
        st_Path.Clear();
        if (bl_Is_Active)
        {
            bl_Moving = true;
        }

        Tile next = tl_Target;
        while(next != null)
        {
            st_Path.Push(next);
            next = next.tl_Start_Tile;
        }
    }

    //---------------------------------------------------

    protected void SetAttackRange(int int_At_Range)
    {
        int_Attack_Range = int_At_Range;
    }

    //---------------------------------------------------

    protected void FindPRsInRange()
    {
        ls_PRs_In_Range.Clear();
        foreach (PlayerRobot pr_Targets in CSGameManager.gameManager.ls_Player_Robots_In_Level)
        {
            Tile tl_Temp = CSGameManager.gameManager.map[pr_Targets.int_x, pr_Targets.int_z].gameObject.GetComponent<Tile>();
            if (tl_Temp.bl_Attack_Selection)
            {
                ls_PRs_In_Range.Add(pr_Targets);
            }
        }
        if (ls_PRs_In_Range.Count <= 0)
        {
            return; //prevents AIs from infinitly moving until they are in range to attack if there's no PCs in range
        }
        else
        {
           // print("Attacking");
            FindAttackTarget();
            //attack
        }
    }

    //---------------------------------------------------

    protected void FindAttackTarget() //can define different ways to prioritise targets here
    {
        PlayerRobot pr_Final_Attack_Target = null;
        for (int i=0; i < ls_PRs_In_Range.Count; i++)//looks at all player robots in range
        {
            if (pr_Final_Attack_Target == null)
            {
                pr_Final_Attack_Target = ls_PRs_In_Range[i];
            }
            if (pr_Final_Attack_Target.int_Health > ls_PRs_In_Range[i].int_Health)//targets the player in range with lowest health, can change in a number of ways
            {                                                                     //this step could be skipped and just taken from the initial target finder from start of turn
                pr_Final_Attack_Target = ls_PRs_In_Range[i];                      //this just allows for more varience in targets.
            }
            AttackTarget(this, pr_Final_Attack_Target);
        }
    }

    //---------------------------------------------------

    protected void AttackTarget(CharacterBase cb_Attacker, CharacterBase cb_Target)
    {
        if (cb_Target == null)
        {
            return;
        }
        cb_Target.int_Health -= cb_Attacker.int_damage;
        print(cb_Target+ " damage taken "+ cb_Attacker.int_damage+" health remaining = "+ cb_Target.int_Health);

        //cb_Target.go_health_bar.transform.localPosition = new Vector3(((float)int_Health - (float)int_Health_max) * (0.5f / int_Health_max), 0, 0);
        //cb_Target.go_health_bar.transform.localScale = new Vector3((1f / int_Health_max) * int_Health, 0.2f, 1);
    }

    //---------------------------------------------------

    protected void SetDamage(int int_Damage_No)
    {
        int_damage = int_Damage_No;
    }

    //---------------------------------------------------

    public void BeginAITurn()
    {
        bl_Is_Active = true;
        bl_Turn_Just_Started = true;
        fl_Turn_Timer = 0f;
        fl_Turn_Timer += Time.deltaTime; //if AI gets stuck taking their turn this boots them out!
    }

    //---------------------------------------------------
}//=======================================================================================
