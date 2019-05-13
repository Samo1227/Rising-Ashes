using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
public class CSGameManager : MonoBehaviour
{
    //---------------------------------------------------
    #region Variables
    public bool bl_IsMission = false; //not sure about this, this is to manage wether the scene is a battle or not but it might be better to have the level builder not be persistant 
    //and instead have a seperate gamemanager

    public Tile tile;//publicly assigned objects
    public HazardTile haz;
    public PlayerRobot pr_PC;
    public int int_PRNumber = 0;
    public IntroPlayerBot ipb_PC; //for intro scene
    public AICharacter ai_Enemy_Test;//might need a more robust system for the actuall game
    public AICharacter ai_Enemy_Test_range;//might need a more robust system for the actuall game
    public AICharacter ai_Enemy_Test_boss;//might need a more robust system for the actuall game

    public Tile[,] map = new Tile[10, 10];//at the moment the map array is limited to this size, this needs changing
    public PlayerRobot pr_currentRobot;//reference to the currently selected player robot
    public TextAsset txt_level;
    public string st_level;
    public string[] arr_at_level;
    int[,] map_layout = new int[10,10];
    int int_Turn_Count = 0;//might be useful for timers, Spawning AIs at certain turns, Events, etc...

    public int int_map_x;
    public int int_map_z;

    public bool bl_MissionStarted = false;

    public List<PlayerRobot> ls_Player_Robots_In_Level = new List<PlayerRobot>();//list of live player robots
    public List<AICharacter> ls_AI_Characters_In_Level = new List<AICharacter>();//list of living enemies (should perhaps be expanded for non hostile AI)
    public bool bl_Player_Turn = true;//if it is the players turn
    public List<PlayerRobot> ls_Player_Robots_With_Turns_Left = new List<PlayerRobot>();//used to automatically go to AI turn when Player has moved all PRs
    public List<AICharacter> ls_SeenAIs = new List<AICharacter>();
    public Queue<AICharacter> qu_AI_Turns = new Queue<AICharacter>(); //enemy turn queue
    public int[] int_temp_robot_data;
    public bool bl_storing_robot;
    public Bot_Modifier robot_mod;

    public AudioSource auSource;
    public AudioClip acCalm;
    public AudioClip acIntense;
    public bool bl_CalmMusic = true;
    public OpenBotMaker botMod;
    #endregion
    //---------------------------------------------------
    #region Singleton
    public static CSGameManager gameManager; //gamemanager singleton
    #endregion
    //---------------------------------------------------
    #region Awake
    private void Awake()            // runs before start
    {
        //-----------
        if (gameManager == null)            // has it been set up before?
        {
            gameManager= this;             // no, it's the first GM, so store our instance
          //  DontDestroyOnLoad(gameObject);// persists through sceen changes
        }
        //-----------
        else if (gameManager != this) // if we get called again. desroy new version and keep old
        {
            Destroy(gameObject); // kill subsequent versions
        }
        //-----------
        //if (SceneManager.GetActiveScene().name == "TileTester")//intended to allow the gameManager to create a battle when on a battle scene but seems to be missing something
        //{
        //    print("mission");
        //    bl_IsMission = true;
        //  //  TextToMapInt();
        //  //  Start();
        //}
        //if (bl_IsMission == false)
        //    return;

        TextToMapInt();

    }
    #endregion
    //---------------------------------------------------
    #region Map Creator
    void TextToMapInt()
    {
        st_level = txt_level.text;
        arr_at_level = st_level.Split(new string[] { "," }, System.StringSplitOptions.None);
        int temp_int = 0;

        int_map_x = System.Convert.ToInt32(arr_at_level[temp_int]);
        temp_int++;

        int_map_z = System.Convert.ToInt32(arr_at_level[temp_int]);
        temp_int++;

        map = new Tile[int_map_x, int_map_z];
        map_layout = new int[int_map_x, int_map_z];

        //-----------
        for (int j = 0; j < int_map_z; j++)
        {
            //-----------
            for (int i = 0; i < int_map_x; i++)
            {
               // Debug.Log(arr_at_level[temp_int]);
                map_layout[i, j] = System.Convert.ToInt32(arr_at_level[temp_int]);
                temp_int++;
            }
            //-----------
        }
        //-----------

        #region text test
        string str = "";
        //-----------
        for (int j = 0; j < 10; j++)
        {
            //-----------
            for (int i = 0; i < 10; i++)
            {
                //-----------
                if (i == 0)
                {
                    str += "{" + map_layout[i, j];
                }
                //-----------
                else if (i == 10 - 1)
                {
                    str += ", " + map_layout[i, j] + "}, \n";
                }
                //-----------
                else
                {
                    str += ", " + map_layout[i, j];
                }
                //-----------
            }
            //-----------
        }
        //-----------
        Debug.Log(str);
        #endregion
        
    }
    #endregion
    //---------------------------------------------------
    #region Start
    void Start() {
        //if (bl_IsMission == false)
        //    return;

        MakeMap();//generates map
        RefreshTile();
        if(SceneManager.GetActiveScene().name== "IntroScene")
        {
            AddIntroBot(4, 4);
        }
        //AddRobot(2, 4);
        //AddRobot(4, 4);
        auSource = gameObject.AddComponent<AudioSource>();
        auSource.clip = acCalm;
        auSource.loop = true;
        auSource.Play();
    }
    #endregion
    //---------------------------------------------------
    #region Refresh Tiles
    public void RefreshTile()
    {
        //-----------
        foreach (Tile tile in map)
        {
            tile.FindNeighbours(tile);
        }
        //-----------
    }
    #endregion
    //---------------------------------------------------
    #region MakeMap
    void MakeMap()
    {
        //-----------
        for (int z = 0; z < int_map_z; z++)
        {
            //-----------
            for (int x = 0; x < int_map_x; x++)
            {
                if(map_layout[x, z] == 2 || map_layout[x, z] == 3 || map_layout[x, z] == 4 || map_layout[x, z] == 5 || map_layout[x, z] == 6 || map_layout[x, z] == 7)
                {
                    HazardTile newHaz = SetHazard(x, z);//makes a tile for every quare in the map

                }
                else
                {
                    Tile newTile = SetTile(x, z);//makes a tile for every quare in the map
                }

            }
            //-----------
        }
        //-----------
        for (int z = 0; z < int_map_z; z++)
        {
            //-----------
            for (int x = 0; x < int_map_x; x++)
            {
                //-----------
                if (map_layout[x, z] == 11)
                {
                    AddEnemy(x, z, 11);
                }

                if (map_layout[x, z] == 12)
                {
                    AddEnemy(x, z, 12);
                }

                if (map_layout[x, z] == 13)
                {
                    AddEnemy(x, z, 13);
                }
                //-----------
            }
            //-----------
        }
        //-----------
    }
    #endregion
    //---------------------------------------------------
    #region Set Tiles
    Tile SetTile(int x, int z)
    {
        Tile newTile = null;
        newTile = Instantiate(tile);
        newTile.transform.SetParent(gameObject.transform);
        newTile.int_X = x;
        newTile.int_Z = z;
        newTile.transform.position = new Vector3(x, transform.position.y, z);
        //-----------
        if (map_layout[x, z] == 0)
        {
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 0), newTile.gameObject.transform);
            newTile.int_health = 0;
            newTile.bl_opaque = false;

        }
        //-----------
        else if (map_layout[x, z] == 1)
        {
            newTile.bl_Is_Walkable = false;
            newTile.bl_Destroyable = true;
            newTile.bl_opaque = true;
            newTile.GetComponent<BoxCollider>().size = new Vector3(1, 3, 1);
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 1), newTile.gameObject.transform);
            newTile.int_health = 5;

        }
        else if (map_layout[x, z] == 9)
        {
            newTile.bl_Is_Walkable = false;
            newTile.bl_Destroyable = true;
            newTile.bl_opaque = false;
            newTile.GetComponent<BoxCollider>().size = new Vector3(1, 3, 1);
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 1), newTile.gameObject.transform);
            newTile.int_health = 5;

        }
        //-----------
        else if (map_layout[x, z] == 17)
        {
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 0), newTile.gameObject.transform);
            newTile.int_health = 0;
            newTile.bl_spawnable_zone = true;
            newTile.bl_opaque = false;

        }
        else
        {
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 0), newTile.gameObject.transform);
            newTile.int_health = 0;
            newTile.bl_opaque = false;
        }
        newTile.name = ("Tile " + x + " " + z);
        map[x, z] = newTile;
        return newTile;
    }

    HazardTile SetHazard(int x, int z)
    {
        HazardTile newHaz = null;
        newHaz = Instantiate(haz);
        newHaz.transform.SetParent(gameObject.transform);
        newHaz.int_X = x;
        newHaz.int_Z = z;
        newHaz.transform.position = new Vector3(x, transform.position.y, z);

        if (map_layout[x, z] == 2)
        {
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 2), newHaz.gameObject.transform);
            newHaz.int_health = 0;
            newHaz.bl_opaque = true;
            newHaz.SetHazardType(HazardTileType.heat);
            newHaz.int_Heat = 1;
        }
        if (map_layout[x, z] == 3)
        {
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 3), newHaz.gameObject.transform);
            newHaz.int_health = 0;
            newHaz.bl_opaque = true;
            newHaz.SetHazardType(HazardTileType.damagingHeat);
            newHaz.int_Heat = 1;
            newHaz.int_Damage = 1;
        }
        if (map_layout[x, z] == 4)
        {
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 4), newHaz.gameObject.transform);
            newHaz.int_health = 0;
            newHaz.bl_opaque = true;
            newHaz.SetHazardType(HazardTileType.cold);
            newHaz.int_Cold = 1;
        }
        if (map_layout[x, z] == 5)
        {
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 5), newHaz.gameObject.transform);
            newHaz.int_health = 0;
            newHaz.bl_opaque = true;
            newHaz.SetHazardType(HazardTileType.damagingCold);
            newHaz.int_Cold = 1;
            newHaz.int_Damage = 1;

        }
        if (map_layout[x, z] == 6)
        {
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 6), newHaz.gameObject.transform);
            newHaz.int_health = 0;
            newHaz.bl_opaque = true;
            newHaz.SetHazardType(HazardTileType.collapsingTile);
            newHaz.int_WeightLimit = 1;
        }
        if (map_layout[x, z] == 7)
        {
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 7), newHaz.gameObject.transform);
            newHaz.int_health = 0;
            newHaz.bl_opaque = false;
            newHaz.SetHazardType(HazardTileType.difficultTerrain);
        }
        if (map_layout[x, z] == 10)
        {
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 10), newHaz.gameObject.transform);
            newHaz.int_health = 0;
            newHaz.bl_opaque = false;
            newHaz.SetHazardType(HazardTileType.goalTile);
        }
        newHaz.name = ("Tile " + x + " " + z);
        map[x, z] = newHaz;
        return newHaz;
    }
    #endregion
    //---------------------------------------------------
    #region Swap Tile
    public void SwapTile(int x, int z, int map_Element)
    {
        Tile temp_Tile = map[x, z];
        Destroy(temp_Tile.gameObject);
        //SetTile(x, z);    
        Tile newTile = null;
        newTile = Instantiate(tile);
        newTile.transform.SetParent(gameObject.transform);
        newTile.int_X = x;
        newTile.int_Z = z;
        newTile.transform.position = new Vector3(x, transform.position.y, z);
        Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 0), newTile.gameObject.transform);
        newTile.bl_Is_Walkable = true;
    }
    #endregion
    //---------------------------------------------------
    #region Add PR

    public void StorePlayer(int[] int_parts)
    {
        if (bl_storing_robot == false)
        {

            int_temp_robot_data = int_parts;
            bl_storing_robot = true;
        }
    }

    public void AddRobot(int cX, int cZ)
    {

        PlayerRobot tRo = null;
        tRo = Instantiate(pr_PC);
        tRo.transform.position = new Vector3(cX, transform.position.y + 1f, cZ);
        tRo.int_x = cX;
        tRo.int_z = cZ;

        if (SceneManager.GetActiveScene().name == "IntroScene")
        {
            tRo.tl_Current_Tile = map[cX, cZ];
            return;
        }
        tRo.int_arr_parts = int_temp_robot_data;
        bl_storing_robot = false;
       
        robot_mod.bl_heads[robot_mod.int_body_type[0]] = true;
        robot_mod.bl_bodies[robot_mod.int_body_type[1]] = true;
        robot_mod.bl_arms[robot_mod.int_body_type[2]] = true;
        robot_mod.bl_legs[robot_mod.int_body_type[3]] = true;

    }

    public void AddIntroBot(int cX, int cZ)
    {
        IntroPlayerBot tIPB = null;
        tIPB = Instantiate(ipb_PC);
        tIPB.transform.position = new Vector3(cX, transform.position.y + 1f, cZ);
        tIPB.int_x = cX;
        tIPB.int_z = cZ;
    }
    #endregion
    //---------------------------------------------------
    #region Add AI
    public void AddEnemy(int cX, int cZ,int type)
    {

        if(type == 11)
        {

            AICharacter temp_AI = null;
            temp_AI = Instantiate(ai_Enemy_Test);
            temp_AI.transform.position = new Vector3(cX, transform.position.y + 1f, cZ);
            temp_AI.int_x = cX;
            temp_AI.int_z = cZ;
        }
        if(type == 12)
        {

            AICharacter temp_AI = null;
            temp_AI = Instantiate(ai_Enemy_Test_range);
            temp_AI.transform.position = new Vector3(cX, transform.position.y + 1f, cZ);
            temp_AI.int_x = cX;
            temp_AI.int_z = cZ;
        }

        if(type == 13)
        {

            AICharacter temp_AI = null;
            temp_AI = Instantiate(ai_Enemy_Test_boss);
            temp_AI.transform.position = new Vector3(cX, transform.position.y + 1f, cZ);
            temp_AI.int_x = cX;
            temp_AI.int_z = cZ;
        }

       
    }
    #endregion
    //---------------------------------------------------
    #region Set Current PR
    public void SetCurrentRobot(PlayerRobot selectedRobot)
    {
        pr_currentRobot = null;
        pr_currentRobot = selectedRobot;
    } 
    #endregion
    //---------------------------------------------------
    #region Turn Manager
    public void EndPlayerTurn(PlayerRobot pr_Turn_Ended)
    {
        pr_Turn_Ended.CheckHazard();
        ls_Player_Robots_With_Turns_Left.Remove(pr_Turn_Ended);
        //-----------
        if (ls_Player_Robots_With_Turns_Left.Count <= 0)
        {
            bl_Player_Turn = false;
            PrepareAITurn();
        }
        //-----------
    }
    //---------------------------------------------------
    public void EndTurnButton()
    {
        if (pr_currentRobot != null)
        {
            pr_currentRobot.bl_Is_Active = false;
            pr_currentRobot.bl_Has_Acted = true;
            pr_currentRobot.bl_Has_Moved = true;
            pr_currentRobot.Clear_Selection();
            EndPlayerTurn(pr_currentRobot);
            pr_currentRobot = null;
        }
    }
    //---------------------------------------------------
    public void PrepareAITurn()
    {
        //-----------
        foreach (AICharacter ai_Temp in ls_AI_Characters_In_Level)
        {
            qu_AI_Turns.Enqueue(ai_Temp); //add all the living AIs to the turn queue
        }
        //-----------
        StartAITurn();
    }
    //---------------------------------------------------
    public void StartAITurn()
    {
     //   print(qu_AI_Turns.Count);
        //-----------
        if (qu_AI_Turns.Count > 0)
        {
            qu_AI_Turns.Peek().BeginAITurn();
        }
        //-----------
    }
    //---------------------------------------------------
    public void EndAITurn()//this does not work OK
    {
        qu_AI_Turns.Peek().CheckHazard();
        qu_AI_Turns.Dequeue();//removes AI from turn queue at end of their specific turn
      //  print(qu_AI_Turns.Count);
      //-----------
        if (qu_AI_Turns.Count > 0)
        {
            StartAITurn(); //if there are AIs left to go, do the next one.
        }
        //-----------
        else
        {
            //print("Player turn");
            PreparePlayerTurn();
        }
        //-----------
    }
    //---------------------------------------------------
    public void PreparePlayerTurn()//this works OK
    {
        ls_Player_Robots_With_Turns_Left.Clear();
        int_Turn_Count++;
        //-----------
        foreach (PlayerRobot pr_Temp in ls_Player_Robots_In_Level) //refresh turn
        {
            //-----------
            if (pr_Temp != null)
            {
                pr_Temp.RefreshPCs();
                ls_Player_Robots_With_Turns_Left.Add(pr_Temp);
            }
            //-----------
        }
        //-----------
        bl_Player_Turn = true;
    }
    //---------------------------------------------------
    #endregion
    //---------------------------------------------------
    #region Win/Loss
    public void CheckLossOrWin()
    {
        if (ls_Player_Robots_In_Level.Count <= 0)
        {
            Debug.Log("lose");
            ClearMapData();
            SceneManager.LoadScene("LoseScreen");
        }
        if (ls_AI_Characters_In_Level.Count <= 0)
        {
            ClearMapData();
            Debug.Log("Win");
            if (SceneManager.GetActiveScene().name == "TileTester")
            {
                ProgressTracker.pt_ProgressTracker.LevelComplete(0);
                SceneManager.LoadScene("Enemies Cleared");
            }
            else
            {
                SceneManager.LoadScene("WinScreen");
            }
        }
    }
    //---------------------------------------------------
    public void GoalReached()
    {
        Debug.Log("Goals");
        SceneManager.LoadScene("WinScreen");
    }
    #endregion
    //---------------------------------------------------
    #region ClearMap
    public void ClearMapData()
    {
        for (int x = 0; x < int_map_x; x++)
        {
            for (int z = 0; z < int_map_z; z++)
            {
                Tile _tile = map[x, z];
                if (_tile != null)
                {
                    Destroy(_tile.gameObject);
                }
            }
        }
        Array.Clear(map, 0, map.Length);//clears the map array so we can reuse it for different levels
        Array.Clear(map_layout, 0, map_layout.Length);
      //  ls_AI_Characters_In_Level.Clear();
      //  ls_Player_Robots_In_Level.Clear();
      //  bl_IsMission = false;
    }
    #endregion
    //---------------------------------------------------
    public void CheckSetAudio()
    {
        if (ls_SeenAIs.Count > 0)
        {
            if (bl_CalmMusic == true)
            {
                auSource.clip = acIntense;
                auSource.Play();
                bl_CalmMusic = false;
            }
        }
        if (ls_SeenAIs.Count == 0)
        {
            if (bl_CalmMusic == false)
            {
                auSource.clip = acCalm;
                auSource.Play();
                bl_CalmMusic = true;
            }
        }
        //foreach(AICharacter ai in ls_AI_Characters_In_Level)
        //{
        //    if (ai.bl_Seen == true)
        //    {
        //        if (bl_CalmMusic == true)
        //        {
        //            auSource.clip = acIntense;
        //            auSource.Play();
        //            bl_CalmMusic = false;
        //        }
        //        break;
        //    }
        //    else
        //    {
        //        if (bl_CalmMusic == false)
        //        {
        //            auSource.clip = acCalm;
        //            auSource.Play();
        //            bl_CalmMusic = true;
        //        }
        //    }

        //}
    }
    public void AddOrRemoveFromSeenList(bool seen, AICharacter _AI)
    {
        if (seen == true)
        {
            ls_SeenAIs.Add(_AI);
        }
        if(seen == false)
        {
            ls_SeenAIs.Remove(_AI);
        }
    } 
    //---------------------------------------------------
    public int ReturnPlayerRobotCount()
    {
        return int_PRNumber;
    }
    public void StartMission()
    {
        bl_MissionStarted = true;
    }
    public void IncrementPRNum()
    {
        int_PRNumber++;
    }
    //---------------------------------------------------
    public void EndPlayerTurn()
    {
        if (pr_currentRobot != null)
        {
            pr_currentRobot.Clear_Selection();
            pr_currentRobot = null;
        }
        foreach(PlayerRobot _PR in ls_Player_Robots_With_Turns_Left)
        {
            _PR.int_Actions = 0;
            EndTurnButton();
        }
    }

    #region Unused
    //public void BuildLevel()
    //{
    //    TextToMapInt();
    //    MakeMap();//generates map
    //    RefreshTile();
    //}

    //public void OnLevelWasLoaded(int level)
    //{
    //    if (SceneManager.GetActiveScene().name == "TileTester")
    //        BuildLevel();
    //}
    #endregion
}//=======================================================================================
