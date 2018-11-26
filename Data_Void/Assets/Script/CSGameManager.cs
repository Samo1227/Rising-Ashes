using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSGameManager : MonoBehaviour {
    public Tile tile;
    public Robot pc;
    public Tile[,] map = new Tile[10, 10];
    public Robot currentRobot;

    int[,] map_layout;


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

        map_layout = new int[,]
        {
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        };
    }
    //============================================



    void Start() {
        MakeMap();
        RefreshTile();
        AddRobot(2, 4);



	}
	
    public void RefreshTile()
    {
        foreach (Tile tile in map)
        {
            tile.FindNeighbours(tile);
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
    }

    Tile SetTile(int x, int z)
    {
        Tile newTile = null;
        newTile = Instantiate(tile);
        newTile.transform.SetParent(gameObject.transform);
        newTile.int_X = x;
        newTile.int_Z = z;
        newTile.transform.position = new Vector3(x, transform.position.y, z);
        Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + map_layout[x,z]),newTile.gameObject.transform);

        if(map_layout[x, z] == 1)
        {
            newTile.bl_Is_Walkable = false;
            newTile.GetComponent<BoxCollider>().size = new Vector3(1,3,1);
        }


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
        Robot tRo = null;
        tRo = Instantiate(pc);
        tRo.transform.position = new Vector3(cX, transform.position.y + 1f, cZ);
        tRo.int_x = cX;
        tRo.int_z = cZ;
    }

    public void SetCurrentRobot(Robot selectedRobot)
    {
        currentRobot = selectedRobot;
    }
}
