using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    //--------------------------------------
    #region Variables
    public int int_X;
    public int int_Z;
    public int int_Distance_From_Start = 0;//for pathfinding
    public int int_Move_Cost = 1;//can be higher to represent difficult to move through terrain
    public int int_Attack_Range_Cost = 1;//can be higher to represent shooting through a substance perhaps?
    public int int_Child = 0;//needed for destroying and recreating walls in the same space
    public List<Tile> ls_Tile_Neighbours = new List<Tile>();
    public bool bl_Is_Walkable = true;
    public bool bl_Walking_Selection = false;
    public bool bl_Attack_Selection = false;
    public bool bl_Occupied_By_PC = false;
    public bool bl_Occupied_By_AI = false;
    //---------------------------------------
    public Tile tl_Start_Tile = null;
    //for A*
    public float f = 0;
    public float g = 0;
    public float h = 0;
    //---------------------------------------
    public Renderer rend_Colour;
    #endregion
    //---------------------------------------
    #region Start & Update
    void Start () {
        rend_Colour = gameObject.transform.GetChild(int_Child).GetComponent<Renderer>();//allows changing of tiles colour and takes into account tiles being changed (destroyed/created)
    }

    //--------------------------------------------

    void Update () { //can make the colours a public selection so can set it ip in inspector
        
            if (bl_Walking_Selection)//if this tile is in walking range
            {
                rend_Colour.material.color = Color.blue;
            }
            else if (bl_Attack_Selection)//if this tile is in attack range
            {
                rend_Colour.material.color = Color.red;
            }
            else//normal  colour
            {
                rend_Colour.material.color = Color.white;
            }
        
	}
    #endregion
    //--------------------------------------------
    #region Neighbour Finder
    public void FindNeighbours(Tile startTile)//finds and stores this tiles neighbours for pathfinding purposes
    {
        ls_Tile_Neighbours.Clear();//clears the neighbours list before rebuilding it so it doesn't keep adding to it
        int x = this.int_X;//reference to this tiles position
        int z = this.int_Z;
        CheckTiles(x, z + 1);//checks up, down, right and left of this tile
        CheckTiles(x, z - 1);
        CheckTiles(x + 1, z);
        CheckTiles(x - 1, z);

    }

    //--------------------------------------------

    public void CheckTiles(int cX, int cZ)
    {
        Tile tT = null;
        if (cX >= 0 && cX < CSGameManager.gameManager.map.GetLength(0) && cZ >= 0 && cZ < CSGameManager.gameManager.map.GetLength(1)) //works based on size of game managers map array
        {
            tT = CSGameManager.gameManager.map[cX, cZ];//sets the temporary tile to the tile in the possition of the map array
            if (tT!=null)//only add to neighbours if not null
            {
                ls_Tile_Neighbours.Add(tT);//adds the tile to the neighbours list
            }
        }
    }
    #endregion
    //--------------------------------------------
    #region Clicking On Tile
    private void OnMouseUp()//could be on mouse down, this reduces the risk of clicking another tile right away though
    {
        if (bl_Walking_Selection) //if this tile is part of the current walking selection
        {
            if (!bl_Occupied_By_PC && !bl_Occupied_By_AI) //only able to move to unnocupied squares
            {
                PlayerRobot rob = CSGameManager.gameManager.pr_currentRobot;//gets a reference to the currently selected player robot
               // rob.transform.position = new Vector3(int_X, rob.transform.position.y, int_Z);//atm teleports robot to selected square

                 rob.MoveToTargetSquare(this);
                 rob.bl_Moving = true;                

                //updates and resets the robots position references
                rob.tl_Current_Tile.bl_Occupied_By_PC = false; //start position tile is no longer occupied
                rob.int_x = int_X;
                rob.int_z = int_Z;//robots current position storage is updated to match
                rob.tl_Current_Tile = CSGameManager.gameManager.map[rob.int_x, rob.int_z].gameObject.GetComponent<Tile>(); //robots reference tile is set to new position
                rob.tl_Current_Tile.bl_Occupied_By_PC = true;//new tile is now occupied
                rob.bl_Has_Moved = true;//robot can no longer move
                rob.Clear_Selection();//clear tile highlighting
            }
        }

    }

    //--------------------------------------------

    private void OnMouseOver()//this doesn't work for shooting enemies, this is when destructable tiles are shot
    {
        if (bl_Attack_Selection)
        {
            PlayerRobot rob = CSGameManager.gameManager.pr_currentRobot;//gets a reference to the currently selected player robot

            RaycastHit hit;

            Vector3 dir = new Vector3(transform.position.x, 1, transform.position.z) - rob.transform.position;
            dir = dir.normalized;
            float fl_Ray_Range = Vector3.Distance(new Vector3(transform.position.x, 1, transform.position.z), rob.transform.position);

            Ray ray_cast = new Ray(rob.transform.position, dir * fl_Ray_Range);//draws a ray between target and selected robot

            Debug.DrawRay(rob.transform.position, dir * fl_Ray_Range, Color.green, 0.1f);//visual representation of ray for editor

            if(Input.GetMouseButtonUp(0))//when left mouse button is clicked
            {
                if (Physics.Raycast(ray_cast, out hit, fl_Ray_Range))//if the raycast has hit something
                {

                    if(hit.collider.gameObject.GetComponent<Tile>())//the collider hit is a tile
                    {
                        rob.bl_Has_Acted = true;//robot has done it's action
                        rob.Clear_Selection();//clear tile highlighting 
                        hit.collider.gameObject.GetComponent<Tile>().RemoveTile();//destroy the tile
                    }

                }
            }
        }
    }

    //--------------------------------------------
    public void RemoveTile()//replaces tile with a plain walkable tile, may want to make alternate versions for different left over tile types
    {
        Destroy(gameObject.transform.GetChild(int_Child).gameObject);//destroys the childed wall
        int_Child++;//increments child index for creating/destroying other objects
        Debug.Log("Fire!");
        Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 0), gameObject.transform);//create the empty tile object, this can be changed for different tile types (hazards, walls, hidering, etc.)
        bl_Is_Walkable = true;
        gameObject.GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);//add a box collider
        rend_Colour = gameObject.transform.GetChild(int_Child).GetComponent<Renderer>();//sets the renderer reference to the new object
        CSGameManager.gameManager.RefreshTile();//rechecks the neighbours as the map has now changed

    }
    #endregion
    //--------------------------------------------
}//=====================================================
