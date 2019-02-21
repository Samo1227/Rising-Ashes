using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour {
    //---------------------------------------------------
    #region Variables
    public int int_x;
    public int int_z;

    public int int_action_points;
    public int int_action_points_max;

    //the stats are not finalised, this is mainly just to get an idea for what is needed

    //Head Stats
    public int int_Veiw_Distance;
    public int int_Veiw_Type;
    //Body Stats
    public int int_Health;
    public int int_Health_max;
    //gimmie gimmie gimmie a honk after midnight ♥

    //Equiptment Stats
    public int int_Attack_Range = 3;
    public int int_damage;
    public int int_heat_current;
    public int int_heat_total;
    public int int_heat_fail_chance;
    public int int_effect;


    //Leg Stats
    public int int_Move_Range = 5;
    public int int_Move_Max;
    public int int_Move_Min;
    public int int_Weight_Current;
    public int int_Weight_Max;

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
    protected List<Tile> ls_Dest_Tiles_In_Range = new List<Tile>(); //destructable tile targets
    //---------------

    public List<Tile> selectableTiles = new List<Tile>();//a list of selectable tiles, useful for clearing data on each tile when the selection is done with 
    public bool bl_Shield;

    public LineRenderer lr_laser;
    #endregion
    //---------------------------------------------------
    #region Start
    void Start()
    {
        int_Health = int_Health_max;//current health = max health at start
        //-----------
        if (gameObject.GetComponent<LineRenderer>() != null)
        {
            lr_laser = gameObject.GetComponent<LineRenderer>();
        }
        //-----------
    }
    #endregion
    //---------------------------------------------------
    #region FindMoveRange Player
    public void FindMoveTiles()//player robot version
    {

        Tile currentTile = CSGameManager.gameManager.map[this.int_x, this.int_z];//this shouldn't really be neccessary as it's set outside of this, but this is just to be safe
    
        Queue<Tile> process = new Queue<Tile>();//processes all the tiles this can move to, runs till no possible tiles are left
        process.Enqueue(currentTile);//starts with the current tile this is on
        //-----------
        while (process.Count > 0)
        {
            Tile tempTile = process.Dequeue(); //takes the tile out of the queue and processes it
            //---------
            if (tempTile.bl_Is_Walkable)   //if the tile is walkable add it to the selectable process
            {
                //----------
                if (!tempTile.bl_Occupied_By_PC)//can't select a tile that has a player robot but can move through it
                {
                    selectableTiles.Add(tempTile);
                    tempTile.bl_Walking_Selection = true;
                }
                //----------
                if (tempTile.int_Distance_From_Start < int_Move_Range) // if it's within move range -1 check the neighbours
                {
                    //----------
                    foreach (Tile neighbourTile in tempTile.ls_Tile_Neighbours)//for every neihbour, check  
                    {
                        //----------
                        if (neighbourTile != currentTile)//stops path getting stuck looping
                        {
                            //----------
                            if (neighbourTile.tl_Start_Tile==null)//only do if there isn't already a start tile for this tile, this should prevent infinite looping 
                                 neighbourTile.tl_Start_Tile = tempTile;
                            //----------
                        }
                        //----------
                        neighbourTile.int_Distance_From_Start = neighbourTile.int_Move_Cost + tempTile.int_Distance_From_Start;//sets the neighbour tiles distance from the start point for move distance limits
                        //----------
                        if (neighbourTile.int_Distance_From_Start <= int_Move_Range)//if the neighbours are within movement range add them to the process queue
                        {
                            //----------
                            if (!neighbourTile.bl_Occupied_By_AI)//player robots can not walk through AI occupied spaces
                            {
                                process.Enqueue(neighbourTile);//add any walkable neighbours to the queue to process their neighbours
                            }
                            //----------
                        }
                        //----------
                    }
                    //----------
                }
                //----------
            }
            //----------
        }
        //----------
    }
    #endregion
    //---------------------------------------------------
    #region FindMoveRange AI
    public void FindMoveTilesAI()//AI robot version, could've been in one method, but it would've made it very messy
    {

        Tile currentTile = CSGameManager.gameManager.map[this.int_x, this.int_z];//this shouldn't really be neccessary as it's set outside of this, but this is just to be safe

        Queue<Tile> process = new Queue<Tile>();//processes all the tiles this can move to, runs till no possible tiles are left
        process.Enqueue(currentTile);//starts with the current tile this robot is on
        //-----------
        while (process.Count > 0)
        {
            Tile tempTile = process.Dequeue(); //takes the tile out of the queue and processes it
            //-----------
            if (tempTile.bl_Is_Walkable)   //if the tile is walkable add it to the selectable process
            {
                //-----------
                if (!tempTile.bl_Occupied_By_AI)//this should mean it can't move to a square with another AI but it still does????? I think this is fixed now
                {
                    selectableTiles.Add(tempTile);
                    tempTile.bl_Walking_Selection = true;
                }
                //-----------
                if (tempTile.int_Distance_From_Start < int_Move_Range) // if it's within move range -1 check the neighbours
                {
                    //-----------
                    foreach (Tile neighbourTile in tempTile.ls_Tile_Neighbours)
                    {
                        //-----------
                        if (neighbourTile != currentTile)
                        {
                            //-----------
                            if (neighbourTile.tl_Start_Tile == null)//only do if there isn't already a start tile for this tile, this should prevent infinite looping 
                                neighbourTile.tl_Start_Tile = tempTile;//this was working without the check, but it might have had trouble at some point, so put this in just in case
                            //-----------
                        }
                        //-----------
                        neighbourTile.int_Distance_From_Start = neighbourTile.int_Move_Cost + tempTile.int_Distance_From_Start;
                        //-----------
                        if (neighbourTile.int_Distance_From_Start <= int_Move_Range)//if the neighbours are within movement range add them to the process queue
                        {
                            //-----------
                            if (!neighbourTile.bl_Occupied_By_PC)
                            {
                                process.Enqueue(neighbourTile);
                            }
                            //-----------
                        }
                        //-----------
                    }
                    //-----------
                }
                //-----------
            }
            //-----------
        }
        //-----------
    }
    #endregion
    //---------------------------------------------------
    #region FindAttackRange
    public void Find_Attack_Tile_Range()//very similar to find move range, but it's a little simpler as doesn't have to check for obsticals which it may need to do in future
    {
        Tile currentTile = CSGameManager.gameManager.map[this.int_x, this.int_z];//starts from this robots position as always.

        Queue<Tile> process = new Queue<Tile>();//processes all the tiles this possibly attack, runs till no possible tiles are left
        process.Enqueue(currentTile);//starts with the current tile this robot is on
        //-----------
        while (process.Count > 0)
        {
            Tile tempTile = process.Dequeue();//takes the tile out of the queue and processes it

            selectableTiles.Add(tempTile);//add to the selected tile list
            tempTile.bl_Attack_Selection = true;//sets that this tile can be attacked
            //-----------
            if (tempTile.int_Distance_From_Start < int_Attack_Range)// if it's within attack range -1 check the neighbours as else a neighbour will be an extra square away
            {
                //-----------
                foreach (Tile neighbourTile in tempTile.ls_Tile_Neighbours)//checks each of the current tiles neighbours 
                {
                    //-----------
                    neighbourTile.int_Distance_From_Start = neighbourTile.int_Attack_Range_Cost + tempTile.int_Distance_From_Start; //attack range cost
                    //-----------
                    if (neighbourTile.int_Distance_From_Start <= int_Attack_Range)//if the neighbour tile is in attack range add it to the process queue
                    {
                        process.Enqueue(neighbourTile);
                    }
                    //-----------
                }
                //-----------
            }
            //-----------
        }
        //-----------
    }
    #endregion
    //---------------------------------------------------
    #region ClearSelection
    public void Clear_Selection()//resets all of the variables for each tile in the map 
    {
        //-----------
        foreach (Tile tl_Temp_Tile in CSGameManager.gameManager.map)//will be slower with begger maps, will have to test to see if this is a problem
        {
            tl_Temp_Tile.bl_Walking_Selection = false;
            tl_Temp_Tile.bl_Attack_Selection = false;
            tl_Temp_Tile.int_Distance_From_Start = 0;//don't want extra movement/attack range cost when finding movement or attack range
            tl_Temp_Tile.tl_Start_Tile = null;
        }
        //-----------
        selectableTiles.Clear();//empties out the selectable tiles list
    }
    #endregion
    //---------------------------------------------------
    #region MovementVisual
    public void MoveToTarget()//used for the visual representation of movement
    {
        //-----------
        if (st_Path.Count > 0)//as long as there is a tile left in the path
        {
            Tile tl_temp = st_Path.Peek();//looks at the path stack and takes a reference to the top of the stack
            Vector3 v3_Target = tl_temp.transform.position;//sets the movement destination to that tile 
            v3_Target.y += 1; //so character doesn't move into the ground, will need to be reworked if we decide to add elevation to the system
            //-----------
            if (Vector3.Distance(transform.position, v3_Target)>= 0.05f)//when it's not very close to the target position, has a margin for error for safety purposes
            {
                CalculateHeading(v3_Target);//work out which direction the robot is moving in
                SetVelocity();

                transform.right = v3_Heading;//faces the direction the robot is moving in
                transform.position += v3_Velocity * Time.deltaTime;//moves to the location over time, perhaps this could have been done simpler by lerping from tile to tile instead
            }
            //-----------
            else
            {
                transform.position = v3_Target;//sets robots position to the target to make sure it's in the right place
                st_Path.Pop();//gets rid of the top tile in the stack
            }
            //-----------
        }
        //-----------
        else
        {
            Clear_Selection();//when at the destination the selection can be cleared
            bl_Moving = false;//the robot is no longer moving
        }
        //-----------
    }

    //---------------------------------------------------

    void CalculateHeading(Vector3 v3_Temp_Target)
    {
        v3_Heading = v3_Temp_Target - transform.position;//works out heading based on robots current position against target position
        v3_Heading.Normalize();//normailizes v3heading to a (-1-0-1) value
    }

    //---------------------------------------------------

    void SetVelocity()
    {
        v3_Velocity = v3_Heading * fl_Move_Speed; //velocity is set by heading (-1 to 1, -1 to 1, -1 to 1) mulitplied by movement speed
    }
    #endregion
    //---------------------------------------------------
    #region PathFinding
    protected void PathFinding(Tile current_Tile, Tile target_Tile)//A* pathfinding journey ^_^
    {
        List<Tile> ls_Open_List = new List<Tile>();
        List<Tile> ls_Closed_List = new List<Tile>();

        ls_Open_List.Add(current_Tile);
        //-----------
        if (Vector3.Distance(current_Tile.transform.position, target_Tile.transform.position) != 0) //if target is not this tile, so AI doesn't move onto the PC square
            {
            current_Tile.h = (Vector3.Distance(current_Tile.transform.position, target_Tile.transform.position));//doing this through the array would be better probably
                current_Tile.f = current_Tile.h;
            //-----------
            while (ls_Open_List.Count > 0)
                {
                Tile tile_Check = FindTileWithLowestFCost(ls_Open_List);
                    ls_Closed_List.Add(tile_Check);
                //-----------
                if (tile_Check == target_Tile || tile_Check == null) //arrived at target tile or no path
                    {
                        tl_Target_Square = FindEndingTile(tile_Check);
                    //-----------
                    if (tl_Target_Square.bl_Occupied_By_AI) //prevents AI from moving into an ocupied tile
                        {
                            tl_Target_Square = tl_Target_Square.tl_Start_Tile;
                        }
                    //-----------
                    MoveToTargetSquare(tl_Target_Square);
                        return;
                    }
                //-----------
                foreach (Tile tl_Tile in tile_Check.ls_Tile_Neighbours) //for each neighboring tile
                    {
                    //-----------
                    if (ls_Closed_List.Contains(tl_Tile))//already checked
                        {
                            //do nothing
                        }
                    //-----------
                    else if (ls_Open_List.Contains(tl_Tile))
                        {
                        float fl_Temp_G_Cost = tile_Check.g + Vector3.Distance(tl_Tile.transform.position, tile_Check.transform.position);
                        //-----------
                        if (fl_Temp_G_Cost < tl_Tile.g)//if the tile being checked's G cost is lower than the currently best tile tile_Check replaces it
                            {
                                tl_Tile.tl_Start_Tile = tile_Check;
                                tl_Tile.g = fl_Temp_G_Cost;
                                tl_Tile.f = tl_Tile.g + tl_Tile.h;
                            }
                        //-----------
                    }
                    //-----------
                    else
                    {
                        tl_Tile.tl_Start_Tile = tile_Check;
                            tl_Tile.g = tile_Check.g + Vector3.Distance(tl_Tile.transform.position, target_Tile.transform.position); //research how to check manhattan distance, as this is a messy way of doing this
                            tl_Tile.h = Vector3.Distance(tl_Tile.transform.position, target_Tile.transform.position);
                            tl_Tile.f = tl_Tile.h + tl_Tile.g;
                        //-----------
                        if (tl_Tile.bl_Is_Walkable)
                            {
                                //if (!tl_Tile.bl_Occupied_By_AI)//won't go into a space with a AI in it... also means it won't walk through AI though
                                {
                                    ls_Open_List.Add(tl_Tile);
                                }
                            }
                        }
                    //-----------
                }
                //-----------
            }
            //-----------
        }
        //-----------
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
    #endregion
    //---------------------------------------------------
    #region SetAttackRange
    protected void SetAttackRange(int int_At_Range)//allows this to be changed by other functions
    {
        int_Attack_Range = int_At_Range;
    }
    #endregion
    //---------------------------------------------------
    #region AI Attacking Target
    protected void FindPRsInRange()//AI finds the Player Robots in range
    {
        ls_PRs_In_Range.Clear();//clear the lis of player robots in range (just to be safe)
        //-----------
        foreach (PlayerRobot pr_Targets in CSGameManager.gameManager.ls_Player_Robots_In_Level)//checks all of the player robots to find who is in range
        {
            Tile tl_Temp = CSGameManager.gameManager.map[pr_Targets.int_x, pr_Targets.int_z].gameObject.GetComponent<Tile>();//this checks if the player robot is within the selected AI's attack range 
            //-----------
            if (tl_Temp.bl_Attack_Selection)//if the tile under the player robot is in attack range
            {
                ls_PRs_In_Range.Add(pr_Targets);//adds PR to list of possible targets
            }
            //-----------
        }
        //-----------
        if (ls_PRs_In_Range.Count <= 0)
        {
            //-----------
            for (int i = 0; i < selectableTiles.Count; i++)
            {
                Tile _tile = selectableTiles[i];
                //-----------
                if (_tile.bl_Destroyable)
                {
                    ls_Dest_Tiles_In_Range.Add(_tile);
                    //dothibng
                }
                //-----------
            }
            //-----------
            return; //prevents AIs from infinitly moving until they are in range to attack if there's no PCs in range
        }
        //-----------
        else
        {
            FindAttackTarget();//works out which player robot to attack out of the ones in range 
        }
        //-----------
    }

    //---------------------------------------------------

    protected void FindAttackTarget() //can define different ways to prioritise targets here
    {
        PlayerRobot pr_Final_Attack_Target = null;
        //-----------
        for (int i=0; i < ls_PRs_In_Range.Count; i++)//looks at all player robots in range
        {
            //-----------
            if (pr_Final_Attack_Target == null)//if there is no target yet, set the first player in range
            {
                pr_Final_Attack_Target = ls_PRs_In_Range[i];
            }
            //-----------
            if (pr_Final_Attack_Target.int_Health > ls_PRs_In_Range[i].int_Health)//targets the player in range with lowest health, can change in a number of ways
            {                                                                     //this step could be skipped and just taken from the initial target finder from start of turn
                //-----------
                if (int_Attack_Range > 1)                                         //this prevents Ranged AI from shooting closer players that are behind walls if there is a further player that is not behind a wall
                {
                    RaycastHit hit;
                    //-----------
                    if (Physics.Raycast(transform.position, ls_PRs_In_Range[i].transform.position, out hit))
                    {
                        //-----------
                        if (hit.collider.GetComponent<PlayerRobot>() == ls_PRs_In_Range[i])
                        {
                            pr_Final_Attack_Target = ls_PRs_In_Range[i];
                        }
                        //-----------
                    }
                    //-----------
                }
                //-----------
                else
                    pr_Final_Attack_Target = ls_PRs_In_Range[i];                         //this just allows for more varience in targets.
            }
            //-----------
            AttackTarget(this, pr_Final_Attack_Target);//calls the function that applies damage
        }
        //-----------
    }
    //---------------------------------------------------
    #region Attack Tile
    protected void FindTileTarget(PlayerRobot _Target)
    {
        Tile tl_TileToAttack = null;
        Tile _Temp = null;
        float fl_Distance_To_Current_Target = Mathf.Infinity;
        //-----------
        for (int i = 0; i < ls_Dest_Tiles_In_Range.Count; i++)
        {
            _Temp = ls_Dest_Tiles_In_Range[i];
            float fl_Tiles_Distance_To_Target = Vector3.Distance(_Temp.transform.position, _Target.transform.position);
            //-----------
            if (fl_Tiles_Distance_To_Target < fl_Distance_To_Current_Target)
            {
                fl_Distance_To_Current_Target = fl_Tiles_Distance_To_Target;
                tl_TileToAttack = _Temp;
            }
            //-----------
        }
        //-----------
        AttackTile(tl_TileToAttack);
    }
    //---------------------------------------------------
    protected void AttackTile(Tile _TileTarget)
    {
        _TileTarget.int_health -= int_damage;
        //-----------
        if (_TileTarget.int_health <= 0)
        {
            _TileTarget.RemoveTile();
        }
        //-----------
    }
    #endregion
    #endregion
    //---------------------------------------------------
    #region Attacking a target
    protected void AttackTarget(CharacterBase cb_Attacker, CharacterBase cb_Target)//takes a reference to the attacker and the attackers target and applies damage
    {
        //-----------
        if (cb_Target == null)//if there is no target cancel this
        {
            return;
        }
        //-----------
        if (cb_Attacker.gameObject.GetComponent<AICharacter>() != null)
        {
            //-----------
            if (cb_Attacker.gameObject.GetComponent<AICharacter>().int_Attack_Range > 1)
            {
                RaycastHit hit;
                //-----------
                if (Physics.Raycast(transform.position, cb_Target.transform.position, out hit))
                {

                    lr_laser.SetPosition(0, transform.position);
                    lr_laser.SetPosition(1, hit.point);
                    StartCoroutine(LaserOff());
                    //-----------
                    if (hit.collider.gameObject.GetComponent<Tile>())//if a Tile is in the way hit that
                    {
                        AttackTile(hit.collider.gameObject.GetComponent<Tile>());
                    }
                    //-----------
                    else if (hit.collider.gameObject.GetComponent<CharacterBase>())//hit first player otherwise
                    {
                        cb_Target = hit.collider.gameObject.GetComponent<CharacterBase>();
                        cb_Target.int_Health -= cb_Attacker.int_damage;
                    }
                    //-----------
                    return;
                }
                //-----------
            }
            //-----------
        }
        //-----------
        cb_Target.int_Health -= cb_Attacker.int_damage;//atm just reduces defenders health by attackers damage value, can be expanded upon at some point

        //print(cb_Target+ " damage taken "+ cb_Attacker.int_damage+" health remaining = "+ cb_Target.int_Health);

        //cb_Target.go_health_bar.transform.localPosition = new Vector3(((float)int_Health - (float)int_Health_max) * (0.5f / int_Health_max), 0, 0);
        //cb_Target.go_health_bar.transform.localScale = new Vector3((1f / int_Health_max) * int_Health, 0.2f, 1);
    }

    //---------------------------------------------------

    protected void SetDamage(int int_Damage_No)
    {
        int_damage = int_Damage_No;
    }
    #endregion
    //---------------------------------------------------
    #region AI TurnStarter
    public void BeginAITurn()//this sets up the start of the AI's turn so they take their turn properlly each turn
    {
        bl_Is_Active = true;
        bl_Turn_Just_Started = true;
        fl_Turn_Timer = 0f;
        fl_Turn_Timer += Time.deltaTime; //if AI gets stuck taking their turn this boots them out!
    }
    #endregion
    //---------------------------------------------------
    #region Hazard
    public void CheckHazard()
    {
        HazardTile _HT = CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<HazardTile>();
        //-----------
        if (_HT != null)
        {
            _HT.ApplyHazard(this);
        }
        //-----------
    }
    #endregion
    //---------------------------------------------------
    #region Laser Shrink
    public IEnumerator LaserOff()
    {
        //-----------
        for (int i = 0; i < int_damage + 1; i++)
        {
            yield return new WaitForSeconds(0.01f);
            lr_laser.startWidth = (int_damage - i) * 0.05f;
            lr_laser.endWidth = (int_damage - i) * 0.05f;
        }
        //-----------
        yield return null;
    } 
    #endregion
}//=======================================================================================
