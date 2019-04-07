using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour {
    //--------------------------------------
    #region Variables
    public int int_X;
    public int int_Z;
    public int int_Distance_From_Start = 0;//for pathfinding
    public int int_Move_Cost = 1;//can be higher to represent difficult to move through terrain
    public int int_Attack_Range_Cost = 1;//can be higher to represent shooting through a substance perhaps?
    public int int_Child = 1;//needed for destroying and recreating walls in the same space
    public List<Tile> ls_Tile_Neighbours = new List<Tile>();
    public bool bl_Is_Walkable = true;
    public bool bl_Walking_Selection = false;
    public bool bl_Attack_Selection = false;
    public bool bl_Occupied_By_PC = false;
    public bool bl_Occupied_By_AI = false;
    public bool bl_Current_Tile = false;
    public bool bl_Destroyable = false;

    public int int_health;
    public int int_health_max;
    public GameObject go_health_bar;
    public GameObject go_health_bar_back;
    //---------------------------------------
    public Tile tl_Start_Tile = null;
    //for A*
    public float f = 0;
    public float g = 0;
    public float h = 0;
    //---------------------------------------
    public Renderer rend_Colour;
    public bool bl_explosive;

    public float fl_ExplodeRaduis;
    public float fl_height;

    public bool bl_spawnable_zone;

    public bool bl_in_view_zone;
    public bool bl_opaque;
    public Renderer go_fog;
    //public int int_view_state;
    public bool bl_radar;
    public bool bl_tag;
    public List<CharacterBase> ls_flame_ice_hit = new List<CharacterBase>();

    #endregion
    //---------------------------------------
    #region Start & Update
    void Start () {
        rend_Colour = gameObject.transform.GetChild(int_Child).GetComponent<Renderer>();//allows changing of tiles colour and takes into account tiles being changed (destroyed/created)
        fl_ExplodeRaduis = 1;

        if(bl_opaque == false)
        {
            GameObject temp = gameObject.transform.GetChild(int_Child).gameObject;
            go_fog = temp.transform.GetChild(0).GetComponent<Renderer>();
        }

    }

    //--------------------------------------------

    void Update () { //can make the colours a public selection so can set it ip in inspecto

        go_health_bar.transform.localPosition = new Vector3(((float)int_health - (float)int_health_max) * (0.5f / int_health_max), 0, 0);
        go_health_bar.transform.localScale = new Vector3((1f / int_health_max) * int_health, 0.2f, 1);

        if (int_health < int_health_max && int_health > 0)
        {
            go_health_bar.SetActive(true);
            go_health_bar_back.SetActive(true);

        }
        else if(int_health <= 0)
        {
            go_health_bar.SetActive(false);
            go_health_bar_back.SetActive(false);
        }
        else
        {
            go_health_bar.SetActive(false);
            go_health_bar_back.SetActive(false);
        }

        if (bl_Current_Tile)
        {
            rend_Colour.material.color = Color.green;
        }
        else if (bl_Walking_Selection)//if this tile is in walking range
        {
            rend_Colour.material.color = Color.blue;
        }
        else if (bl_Attack_Selection)//if this tile is in attack range
        {
            rend_Colour.material.color = Color.red;
        }
        else if (bl_spawnable_zone == true && CSGameManager.gameManager.bl_storing_robot == true && bl_Is_Walkable && bl_Occupied_By_PC == false && bl_Occupied_By_AI == false)
        {
            rend_Colour.material.color = Color.green;
        }
        else//normal  colour
        {
            rend_Colour.material.color = Color.white;
        }
        if(bl_Is_Walkable == false && int_health <= 0)
        {
            RemoveTile();
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

        if (EventSystem.current.IsPointerOverGameObject())
        {
           // print("Why do you click?");
            return;
        }
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
                rob.tl_Current_Tile.bl_Current_Tile = false;
                rob.int_x = int_X;
                rob.int_z = int_Z;//robots current position storage is updated to match
                rob.tl_Current_Tile = CSGameManager.gameManager.map[rob.int_x, rob.int_z].gameObject.GetComponent<Tile>(); //robots reference tile is set to new position
                rob.tl_Current_Tile.bl_Occupied_By_PC = true;//new tile is now occupied
                rob.bl_Has_Moved = true;//robot can no longer move
                rob.int_Actions--;
                rob.Clear_Selection();//clear tile highlighting
                StartCoroutine(rob.Clear_View());
                foreach (PlayerRobot pr in CSGameManager.gameManager.ls_Player_Robots_In_Level)
                {
                    StartCoroutine(pr.FindVeiwableTiles());
                }
            }
        }



    }

    //--------------------------------------------

    private void OnMouseOver()//this doesn't work for shooting enemies, this is when destructable tiles are shot
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (bl_Attack_Selection)
        {
            PlayerRobot rob = CSGameManager.gameManager.pr_currentRobot;//gets a reference to the currently selected player robot

            RaycastHit[] hits = new RaycastHit[3];

            float[] fl_Ray_Range = new float[0];
            Ray[] ray_cast = new Ray[0];
            int int_rays;

            if (rob.int_effect == 0)//Laser
            {
                Vector3 dir = new Vector3(transform.position.x, 1, transform.position.z) - rob.transform.position;
                dir = dir.normalized;

                int_rays = 0;

                ray_cast = new Ray[int_rays + 1];

                fl_Ray_Range = new float[int_rays + 1];

                fl_Ray_Range[0] = 1000;

                ray_cast[int_rays] = new Ray(rob.transform.position, dir * fl_Ray_Range[0]);//draws a ray between target and selected robot

                Debug.DrawRay(rob.transform.position, dir * fl_Ray_Range[0], Color.green, 0.1f);//visual representation of ray for editor
                
            }
            else if (rob.int_effect == 3)
            {

                int_rays = 4;

                ray_cast = new Ray[int_rays + 1];

                float[] ray_direction_x = new float[int_rays + 1];
                float[] ray_direction_z = new float[int_rays + 1];

                fl_Ray_Range = new float[int_rays + 1];

                int temp_dir_x = int_X - rob.int_x;
                int temp_dir_z = int_Z - rob.int_z;

                for (int i = 0; i < int_rays + 1; i++)
                {

                    if(temp_dir_x != 0)
                    {
                        ray_direction_x[i] = (temp_dir_x * 2) + transform.position.x;
                    }
                    else
                    {
                        ray_direction_x[i] = ((i - 2)*0.8f) + transform.position.x;
                    }
                    if (temp_dir_z != 0)
                    {
                        ray_direction_z[i] = (temp_dir_z * 2) + transform.position.z;
                    }
                    else
                    {
                        ray_direction_z[i] = ((i - 2) * 0.8f) + transform.position.z;
                    }
                    //Debug.Log(ray_direction_x[i] + " , " + ray_direction_z[i]);

                }
                for (int i = 0; i < int_rays + 1; i++)
                {
                    
                    Vector3 dir = new Vector3 (ray_direction_x[i] - transform.position.x, 0,ray_direction_z[i] - transform.position.z);
                    dir = dir.normalized;

                    fl_Ray_Range[i] = Vector3.Distance(new Vector3(ray_direction_x[i], 1, ray_direction_z[i]),new Vector3(transform.position.x, 1, transform.position.z));

                    ray_cast[i] = new Ray(new Vector3(transform.position.x, 1, transform.position.z), dir * fl_Ray_Range[i]);//draws a ray between target and selected robot

                    Debug.DrawRay(new Vector3(transform.position.x, 1, transform.position.z), dir * fl_Ray_Range[i], Color.green, 0.1f);//visual representation of ray for editor

                }
            }
            else
            {
                Vector3 dir = new Vector3(transform.position.x, 1, transform.position.z) - rob.transform.position;
                dir = dir.normalized;

                int_rays = 0;

                ray_cast = new Ray[int_rays + 1];

                fl_Ray_Range[0] = Vector3.Distance(new Vector3(transform.position.x, 1, transform.position.z), rob.transform.position);

                ray_cast[int_rays] = new Ray(rob.transform.position, dir * fl_Ray_Range[0]);//draws a ray between target and selected robot

                Debug.DrawRay(rob.transform.position, dir * fl_Ray_Range[0], Color.green, 0.1f);//visual representation of ray for editor
            }

            //for (int i = 0; i < int_rays + 1; i++)
            //{
            //    ray_cast[int_rays] = new Ray(rob.transform.position, dir * fl_Ray_Range);//draws a ray between target and selected robot

            //    Debug.DrawRay(rob.transform.position, dir * fl_Ray_Range, Color.green, 0.1f);//visual representation of ray for editor
            //}

            if (Input.GetMouseButtonUp(0))//when left mouse button is clicked
            {
                Vector3 v3_aim_direction;
                Vector3 v3_aim;
                float fl_angle;

                v3_aim_direction = new Vector3(int_X,0,int_Z);
                v3_aim.x = rob.int_x - v3_aim_direction.x;
                v3_aim.z = rob.int_z - v3_aim_direction.z;
                fl_angle = Mathf.Atan2(v3_aim.x, v3_aim.z) * Mathf.Rad2Deg;
                rob.transform.rotation = Quaternion.Euler(new Vector3(0, fl_angle + 90, 0));

                
                if (rob.int_effect == 2)
                {
                    rob.bl_Has_Acted = true;
                    rob.int_Actions--;
                    rob.Clear_Selection();
                    CreatePlayerTile();
                    rob.RandomDamage();
                    int_health_max = rob.int_damage;
                    rob.int_heat_current += 1;
                    bl_explosive = rob.bl_overheat;

                }
                else if (rob.int_effect == 3)
                {
                    Debug.Log("( ͡° ͜ʖ ͡°)");
                    for (int i = 0; i < 5; i++)
                    {
                        Debug.Log("for");

                        if (Physics.RaycastNonAlloc(ray_cast[i], hits, fl_Ray_Range[i]) > 0)//if the raycast has hit something
                        {
                            Debug.Log("RayCast Hit");
                            foreach (RaycastHit hit in hits)
                            {
                                Debug.Log("foreach");
                                if (hit.collider != null)
                                {
                                    if (hit.collider.gameObject.GetComponent<CharacterBase>())
                                    {
                                        Debug.Log("Get");
                                        if(hit.collider.gameObject.GetComponent<CharacterBase>().ice_fire_select == false)
                                        {
                                            ls_flame_ice_hit.Add(hit.collider.gameObject.GetComponent<CharacterBase>());
                                            hit.collider.gameObject.GetComponent<CharacterBase>().ice_fire_select = true;
                                        }

                                    }
                                    else if (hit.collider.gameObject.GetComponent<Tile>())
                                    {
                                        goto OUTERLOOP; 
                                    }

                                    Debug.Log("hit");

                                    //rob.lr_laser.SetPosition(i+1, hit.point);
                                }
                            }
                        OUTERLOOP:;
                        }


                    }

                    foreach (CharacterBase CB_item in ls_flame_ice_hit)
                    {
                        if (CB_item.gameObject.GetComponent<PlayerRobot>())
                        {
                            if (rob.bl_overheat)//fire
                            {
                                CB_item.gameObject.GetComponent<PlayerRobot>().int_heat_current++;
                            }
                            else//ice
                            {
                                CB_item.gameObject.GetComponent<PlayerRobot>().int_heat_current =+ 2;
                            }
                        }
                        else if (CB_item.gameObject.GetComponent<AICharacter>())
                        {
                            rob.RandomDamage();

                            if (rob.bl_overheat)
                            {
                                CB_item.gameObject.GetComponent<AICharacter>().int_Health -= rob.int_damage;
                            }
                            else
                            {
                                CB_item.gameObject.GetComponent<AICharacter>().int_Health -= rob.int_damage;
                            }
                        }
                        CB_item.gameObject.GetComponent<PlayerRobot>().ice_fire_select = false;
                    }
                    rob.int_heat_current += 1;
                    ls_flame_ice_hit.Clear();
                    Debug.Log("( ͡° ͜ʖ ͡°)");
                    rob.bl_Has_Acted = true;//robot has done it's action
                    rob.int_Actions--;
                    rob.Clear_Selection();//clear tile highlighting 

                }
                else if (rob.int_effect == 4)
                {
                    PlayerRobot rob_held = rob.Held_robot;
                    Debug.Log("place");

                    if (!bl_Occupied_By_PC && !bl_Occupied_By_AI && rob_held != null) //only able to move to unnocupied squares
                    {
                        Debug.Log("place2");
                        rob_held.gameObject.transform.parent = null;
                        //rob_held.MoveToTargetSquare(this);
                        //rob_held.bl_Moving = true;

                        //updates and resets the robots position references
                        
                        rob_held.int_x = int_X;
                        rob_held.int_z = int_Z;//robots current position storage is updated to match
                        rob_held.tl_Current_Tile = CSGameManager.gameManager.map[int_X, int_Z].gameObject.GetComponent<Tile>(); //robots reference tile is set to new position
                        rob_held.tl_Current_Tile.bl_Occupied_By_PC = true;//new tile is now occupied
                        rob_held.GetComponent<Collider>().enabled = true;
                        rob_held.transform.position = new Vector3(rob_held.transform.position.x, 1f, rob_held.transform.position.z);
                        rob.int_Actions--;
                        rob.bl_Has_Acted = true;
                        rob.Clear_Selection();//clear tile highlighting
                        rob.Held_robot = null;

                    }
                }
                else
                {
                    if (Physics.RaycastNonAlloc(ray_cast[0], hits, fl_Ray_Range[0]) > 0)//if the raycast has hit something
                    {

                        if (hits[0].collider.gameObject.GetComponent<Tile>())//the collider hit is a tile
                        {
                            rob.RandomDamage();

                            if (rob.int_effect == 0)
                            {
                                rob.lr_laser.startWidth = rob.int_damage * 0.05f;
                                rob.lr_laser.endWidth = rob.int_damage * 0.05f;
                                StartCoroutine(rob.LaserOff());
                                rob.lr_laser.SetPosition(0, rob.transform.position);
                                rob.lr_laser.SetPosition(1, hits[0].point);

                                if (rob.bl_overheat == false)
                                {
                                    hits[0].collider.gameObject.GetComponent<Tile>().int_health -= rob.int_damage;
                                    rob.int_heat_current += 1;
                                    if (hits[0].collider.gameObject.GetComponent<Tile>().int_health < 0)
                                    {
                                        rob.lr_laser.SetPosition(1, hits[1].point);

                                        if (hits[1].collider.gameObject.GetComponent<Tile>())
                                        {
                                            hits[1].collider.gameObject.GetComponent<Tile>().int_health += hits[0].collider.gameObject.GetComponent<Tile>().int_health;
                                        }
                                        if (hits[1].collider.gameObject.GetComponent<CharacterBase>() && hits[1].collider.gameObject.GetComponent<CharacterBase>().bl_Shield == false)
                                        {
                                            hits[1].collider.gameObject.GetComponent<CharacterBase>().int_Health += hits[0].collider.gameObject.GetComponent<Tile>().int_health;
                                        }
                                        else if (hits[1].collider.gameObject.GetComponent<CharacterBase>() && hits[1].collider.gameObject.GetComponent<CharacterBase>().bl_Shield == true)
                                        {
                                            hits[1].collider.gameObject.GetComponent<CharacterBase>().bl_Shield = false;
                                        }
                                        hits[0].collider.gameObject.GetComponent<Tile>().int_health = 0;
                                    }
                                }
                                else
                                {

                                    rob.lr_laser.SetPosition(0, rob.transform.position);
                                    rob.lr_laser.SetPosition(1, hits[1].point);

                                    if (hits[1].collider.gameObject.GetComponent<Tile>())
                                    {
                                        hits[1].collider.gameObject.GetComponent<Tile>().int_health -= rob.int_damage;
                                        rob.int_heat_current += 1;
                                    }
                                    else if (hits[1].collider.gameObject.GetComponent<CharacterBase>())
                                    {
                                        hits[1].collider.gameObject.GetComponent<CharacterBase>().int_Health -= rob.int_damage;
                                        rob.int_heat_current += 1;
                                    }

                                }
                                rob.bl_Has_Acted = true;//robot has done it's action
                                rob.int_Actions--;
                                rob.Clear_Selection();//clear tile highlighting 

                            }
                            if (rob.int_effect == 1)//drill
                            {
                                hits[0].collider.gameObject.GetComponent<Tile>().int_health -= rob.int_damage * 2;
                                rob.int_heat_current += 1;
                                if (rob.bl_overheat == true)
                                {
                                    StartCoroutine(rob.DrillOverheat(int_X, int_Z));

                                }
                                else
                                {
                                    rob.bl_Has_Acted = true;//robot has done it's action
                                    rob.int_Actions--;
                                    rob.Clear_Selection();//clear tile highlighting 
                                }

                            }

                        }
                        if (hits[0].collider.gameObject.GetComponent<CharacterBase>())//the collider hit is a tile
                        {
                            rob.RandomDamage();

                            if (rob.int_effect == 0)
                            {
                                rob.lr_laser.startWidth = rob.int_damage * 0.05f;
                                rob.lr_laser.endWidth = rob.int_damage * 0.05f;
                                StartCoroutine(rob.LaserOff());
                                rob.lr_laser.SetPosition(0, rob.transform.position);
                                rob.lr_laser.SetPosition(1, hits[0].point);

                                if (rob.bl_overheat == false)
                                {
                                    hits[0].collider.gameObject.GetComponent<CharacterBase>().int_Health -= rob.int_damage;
                                    rob.int_heat_current += 1;
                                    if (hits[0].collider.gameObject.GetComponent<CharacterBase>().int_Health < 0)
                                    {
                                        rob.lr_laser.SetPosition(1, hits[1].point);

                                        if (hits[1].collider.gameObject.GetComponent<Tile>())
                                        {
                                            hits[1].collider.gameObject.GetComponent<Tile>().int_health += hits[0].collider.gameObject.GetComponent<CharacterBase>().int_Health;
                                        }
                                        else if (hits[1].collider.gameObject.GetComponent<CharacterBase>())
                                        {
                                            hits[1].collider.gameObject.GetComponent<CharacterBase>().int_Health += hits[0].collider.gameObject.GetComponent<CharacterBase>().int_Health;
                                        }
                                        else if (hits[1].collider.gameObject == null)
                                        {
                                            return;
                                        }
                                    }
                                    rob.bl_Has_Acted = true;//robot has done it's action
                                    rob.int_Actions--;
                                    rob.Clear_Selection();//clear tile highlighting 
                                }
                                else
                                {

                                    rob.lr_laser.SetPosition(0, rob.transform.position);
                                    rob.lr_laser.SetPosition(1, hits[1].point);

                                    if (hits[1].collider.gameObject.GetComponent<Tile>())
                                    {
                                        hits[1].collider.gameObject.GetComponent<Tile>().int_health -= rob.int_damage;
                                        rob.int_heat_current += 1;
                                    }
                                    else if (hits[1].collider.gameObject.GetComponent<CharacterBase>())
                                    {
                                        hits[1].collider.gameObject.GetComponent<CharacterBase>().int_Health -= rob.int_damage;
                                        rob.int_heat_current += 1;
                                    }
                                    rob.bl_Has_Acted = true;//robot has done it's action
                                    rob.int_Actions--;
                                    rob.Clear_Selection();//clear tile highlighting 

                                }
                            }
                        }


                    }
                }

            }
        }
        if ((bl_spawnable_zone == true && CSGameManager.gameManager.bl_storing_robot == true && bl_Is_Walkable && bl_Occupied_By_PC == false && bl_Occupied_By_AI == false))
        {

            if(Input.GetMouseButtonUp(0))
            {
                CSGameManager.gameManager.AddRobot(int_X, int_Z);
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
        GameObject temp = gameObject.transform.GetChild(int_Child).gameObject;
        go_fog = temp.transform.GetChild(0).GetComponent<Renderer>();
        bl_Is_Walkable = true;
        bl_Destroyable = false;
        bl_opaque = false;
        gameObject.GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);//add a box collider
        rend_Colour = gameObject.transform.GetChild(int_Child).GetComponent<Renderer>();//sets the renderer reference to the new object
        CSGameManager.gameManager.RefreshTile();//rechecks the neighbours as the map has now changed
        int_Child--;
        if (bl_explosive == true)
        {
            ExplodeCast();
        }
    }

    public void CreatePlayerTile()//replaces tile with a plain walkable tile, may want to make alternate versions for different left over tile types
    {
        Destroy(gameObject.transform.GetChild(int_Child).gameObject);//destroys the childed wall
        int_Child++;//increments child index for creating/destroying other objects
        go_fog = null;
        Debug.Log("Fire!");
        Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + "PlayerMade"), gameObject.transform);//create the empty tile object, this can be changed for different tile types (hazards, walls, hidering, etc.)
        bl_Is_Walkable = false;
        bl_Destroyable = true;
        bl_opaque = true;
        gameObject.GetComponent<BoxCollider>().size = new Vector3(1, 3, 1);//add a box collider
        rend_Colour = gameObject.transform.GetChild(int_Child).GetComponent<Renderer>();//sets the renderer reference to the new object
        CSGameManager.gameManager.RefreshTile();//rechecks the neighbours as the map has now changed
        int_health = int_health_max;
        int_Child--;

    }

    void ExplodeCast ()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, fl_ExplodeRaduis);

        foreach(Collider hit in hits)
        {

            if (hit.transform.gameObject.GetComponent<Tile>())
            {
                hit.transform.gameObject.GetComponent<Tile>().int_health -= int_health_max;
            }
            if(hit.transform.gameObject.GetComponent<CharacterBase>() && hit.transform.gameObject.GetComponent<CharacterBase>().bl_Shield == false)
            {
                hit.transform.gameObject.GetComponent<CharacterBase>().int_Health -= int_health_max;
            }
            else if(hit.transform.gameObject.GetComponent<CharacterBase>().bl_Shield == true)
            {
                hit.transform.gameObject.GetComponent<CharacterBase>().bl_Shield = false;
            }
        }

    }
    /*
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fl_ExplodeRaduis);
    }
    */
    #endregion

    public IEnumerator VisibleTile()
    {

        Debug.Log("go fog");
        if (go_fog != null)
        {
            go_fog.enabled = !bl_in_view_zone;
        }
        /*
        if(bl_in_view_zone)
        {
            int_view_state = view_rob.int_head_effect;
            
        }
        */
        yield return null;
    }
    //--------------------------------------------
}//=====================================================
