using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class CSGameManager : MonoBehaviour {
    //---------------------------------------------------
    #region Variables
    public Tile tile;//publicly assigned objects
    public PlayerRobot pr_PC;
    public AICharacter ai_Enemy_Test;//might need a more robust system for the actuall game
    public Tile[,] map = new Tile[10, 10];//at the moment the map array is limited to this size, this needs changing
    public PlayerRobot pr_currentRobot;//reference to the currently selected player robot
    public TextAsset txt_level;
    public string st_level;
    public string[] arr_at_level;
    int[,] map_layout = new int[10,10];
    int int_Turn_Count = 0;//might be useful for timers, Spawning AIs at certain turns, Events, etc...

    public int int_map_x;
    public int int_map_z;

    public List<PlayerRobot> ls_Player_Robots_In_Level = new List<PlayerRobot>();//list of live player robots
    public List<AICharacter> ls_AI_Characters_In_Level = new List<AICharacter>();//list of living enemies (should perhaps be expanded for non hostile AI)
    public bool bl_Player_Turn = true;//if it is the players turn
    public List<PlayerRobot> ls_Player_Robots_With_Turns_Left = new List<PlayerRobot>();//used to automatically go to AI turn when Player has moved all PRs
    public Queue<AICharacter> qu_AI_Turns = new Queue<AICharacter>(); //enemy turn queue
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
            DontDestroyOnLoad(gameObject);// persists through sceen changes
        }
        //-----------
        else if (gameManager != this) // if we get called again. desroy new version and keep old
        {
            Destroy(gameObject); // kill subsequent versions
        }
        //-----------

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
                Debug.Log(arr_at_level[temp_int]);
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
        MakeMap();//generates map
        RefreshTile();

        //AddRobot(2, 4);
        //AddRobot(4, 4);

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
                Tile newTile = SetTile(x, z);//makes a tile for every quare in the map
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
                if (map_layout[x, z] == 3)
                {
                    AddEnemy(x, z);
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
        //Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + map_layout[x,z]),newTile.gameObject.transform);
        //-----------
        if (map_layout[x, z] == 0)
        {
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 0), newTile.gameObject.transform);
        }
        //-----------
        else if (map_layout[x, z] == 1)
        {
            newTile.bl_Is_Walkable = false;
            newTile.GetComponent<BoxCollider>().size = new Vector3(1, 3, 1);
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 1), newTile.gameObject.transform);
        }
        //-----------
        else if (map_layout[x, z] == 3)
        {
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 0), newTile.gameObject.transform);
            //AddEnemy(x, z);
        }
        //-----------

        newTile.name = ("Tile " + x + " " + z);
        map[x, z] = newTile;
        return newTile;
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
    public void AddRobot(int cX, int cZ, PlayerRobot pr_made)
    {
        PlayerRobot tRo = null;
        tRo = Instantiate(pr_made);
        tRo.transform.position = new Vector3(cX, transform.position.y + 1f, cZ);
        tRo.int_x = cX;
        tRo.int_z = cZ;
    }
    #endregion
    //---------------------------------------------------
    #region Add AI
    public void AddEnemy(int cX, int cZ)
    {
        AICharacter temp_AI = null;
        temp_AI = Instantiate(ai_Enemy_Test);
        temp_AI.transform.position = new Vector3(cX, transform.position.y + 1f, cZ);
        temp_AI.int_x = cX;
        temp_AI.int_z = cZ;
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
            EndPlayerTurn(pr_currentRobot);
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
        print(qu_AI_Turns.Count);
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
            print("Player turn");
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
        }
        if (ls_AI_Characters_In_Level.Count <= 0)
        {
            Debug.Log("Win");
        }
    }
    #endregion
}//=======================================================================================
