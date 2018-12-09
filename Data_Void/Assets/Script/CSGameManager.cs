using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class CSGameManager : MonoBehaviour {
    public Tile tile;
    public PlayerRobot pr_PC;
    public AICharacter ai_Enemy_Test;
    public Tile[,] map = new Tile[10, 10];
    public PlayerRobot pr_currentRobot;
    public TextAsset txt_level;
    public string st_level;
    public string[] arr_at_level;
    int[,] map_layout = new int[10,10];
    int int_Turn_Count = 0;

    public List<PlayerRobot> ls_Player_Robots_In_Level = new List<PlayerRobot>();//list of live player robots
    public List<AICharacter> ls_AI_Characters_In_Level = new List<AICharacter>();//list of living enemies (should perhaps be expanded for non hostile AI)
    public bool bl_Player_Turn = true;
    public List<PlayerRobot> ls_Player_Robots_With_Turns_Left = new List<PlayerRobot>();
    public Queue<AICharacter> qu_AI_Turns = new Queue<AICharacter>(); //enemy turn queue



    //============================================
    public static CSGameManager gameManager;
    private void Awake()            // runs before start
    {
        if (gameManager == null)            // has it been set up before?
        {
            gameManager= this;             // no, it's the first GM, so store our instance
            DontDestroyOnLoad(gameObject);// persists through sceen changes
        }
        else if (gameManager != this) // if we get called again. desroy new version and keep old
        {
            Destroy(gameObject); // kill subsequent versions
        }

        TextToMapInt();

    }
    //============================================

    void TextToMapInt()
    {
        st_level = txt_level.text;
        arr_at_level = st_level.Split(new string[] { "," }, System.StringSplitOptions.None);
        int temp_int = 0;
        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < 10; i++)
            {
                Debug.Log(arr_at_level[temp_int]);
                map_layout[i, j] = System.Convert.ToInt32(arr_at_level[temp_int]);
                temp_int++;
            }
        }
        
        #region text test
        string str = "";

        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 0)
                {
                    str += "{" + map_layout[i, j];
                }
                else if (i == 10 - 1)
                {
                    str += ", " + map_layout[i, j] + "}, \n";
                }
                else
                {
                    str += ", " + map_layout[i, j];
                }
            }
        }
        Debug.Log(str);
        #endregion
        
    }

    void Start() {
        MakeMap();
        RefreshTile();
        AddRobot(2, 4);
        AddRobot(4, 4);

    }

    public void RefreshTile()
    {
        foreach (Tile tile in map)
        {
            tile.FindNeighbours(tile);
            //Debug.Log(tile);
        }
    }

    void MakeMap()
    {
        for(int z=0; z< 10; z++)
        {
            for(int x = 0; x < 10; x++)
            {
                Tile newTile = SetTile(x, z);
            }
        }

        for (int z = 0; z < 10; z++)
        {
            for (int x = 0; x < 10; x++)
            {
                if(map_layout[x, z] == 3)
                {
                    AddEnemy(x, z);
                }
            }
        }
    }

    Tile SetTile(int x, int z)
    {
        Tile newTile = null;
        newTile = Instantiate(tile);
        newTile.transform.SetParent(gameObject.transform);
        newTile.int_X = x;
        newTile.int_Z = z;
        newTile.transform.position = new Vector3(x, transform.position.y, z);
        //Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + map_layout[x,z]),newTile.gameObject.transform);
        if(map_layout[x, z] == 0)
        {
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 0), newTile.gameObject.transform);
        }

        else if (map_layout[x, z] == 1)
        {
            newTile.bl_Is_Walkable = false;
            newTile.GetComponent<BoxCollider>().size = new Vector3(1, 3, 1);
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 1), newTile.gameObject.transform);

        }
        else if (map_layout[x,z] == 3)
        {
            Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 0), newTile.gameObject.transform);
            //AddEnemy(x, z);
        }


        newTile.name=("Tile " + x + " " + z);
        map[x,z] = newTile;
        return newTile;
    }

    public void SwapTile(int x, int z, int map_Element)
    {
       Tile temp_Tile=  map[x, z];
        Destroy(temp_Tile.gameObject);
        //SetTile(x, z);    
        Tile newTile = null;
        newTile = Instantiate(tile);
        newTile.transform.SetParent(gameObject.transform);
        newTile.int_X = x;
        newTile.int_Z = z;
        newTile.transform.position = new Vector3(x, transform.position.y, z);
        Instantiate(Resources.Load<GameObject>("MapParts/MapElement_"+0), newTile.gameObject.transform);
        newTile.bl_Is_Walkable = true;
    }

    public void AddRobot(int cX, int cZ)
    {
        PlayerRobot tRo = null;
        tRo = Instantiate(pr_PC);
        tRo.transform.position = new Vector3(cX, transform.position.y + 1f, cZ);
        tRo.int_x = cX;
        tRo.int_z = cZ;
    }

    public void AddEnemy(int cX, int cZ)
    {
        AICharacter temp_AI = null;
        temp_AI = Instantiate(ai_Enemy_Test);
        temp_AI.transform.position = new Vector3(cX, transform.position.y + 1f, cZ);
        temp_AI.int_x = cX;
        temp_AI.int_z = cZ;
    }

    public void SetCurrentRobot(PlayerRobot selectedRobot)
    {
        pr_currentRobot = null;
        pr_currentRobot = selectedRobot;
    }

    #region Turn Manager
    public void EndPlayerTurn(PlayerRobot pr_Turn_Ended)
    {
        ls_Player_Robots_With_Turns_Left.Remove(pr_Turn_Ended);
        if(ls_Player_Robots_With_Turns_Left.Count <= 0)
        {
            bl_Player_Turn = false;
            PrepareAITurn();
        }
    }

    public void PrepareAITurn()
    {
        foreach(AICharacter ai_Temp in ls_AI_Characters_In_Level)
        {
            qu_AI_Turns.Enqueue(ai_Temp); //add all the living AIs to the turn queue
        }
        StartAITurn();
    }

    public void StartAITurn()
    {
        print(qu_AI_Turns.Count);
        if (qu_AI_Turns.Count > 0)
        {
            qu_AI_Turns.Peek().BeginAITurn();
        }
    }

    public void EndAITurn()//this does not work OK
    {
       qu_AI_Turns.Dequeue();//removes AI from turn queue at end of their specific turn
      //  print(qu_AI_Turns.Count);
        if (qu_AI_Turns.Count > 0)
        {
            StartAITurn(); //if there are AIs left to go, do the next one.
        }
        else
        {

            print("Player turn");
            PreparePlayerTurn();
        }
    }

    public void PreparePlayerTurn()//this works OK
    {
        ls_Player_Robots_With_Turns_Left.Clear();
        int_Turn_Count++;
        foreach (PlayerRobot pr_Temp in ls_Player_Robots_In_Level) //refresh turn
        {
            if (pr_Temp != null)
            {
                pr_Temp.RefreshPCs();
                ls_Player_Robots_With_Turns_Left.Add(pr_Temp);
            }
        }
        bl_Player_Turn = true;
    }

    #endregion
}
