using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    //--------------------------------------
    public int int_X;
    public int int_Z;
    public int int_Distance_From_Start = 0;
    public int int_Move_Cost = 1;
    public int int_Child = 0;
    public List<Tile> ls_Tile_Neighbours = new List<Tile>();
    public bool bl_Is_Walkable = true;
    public bool bl_Walking_Selection = false;
    public bool bl_Attack_Selection = false;

    //---------------------------------------
    public Renderer rend_Colour;
    //---------------------------------------

	void Start () {
        //rend_Colour = gameObject.GetComponentInChildren<Renderer>();
        rend_Colour = gameObject.transform.GetChild(int_Child).GetComponent<Renderer>();
    }
	
	void Update () {
        
            if (bl_Walking_Selection)
            {
                rend_Colour.material.color = Color.cyan;
            }
            else if (bl_Attack_Selection)
            {
                rend_Colour.material.color = Color.red;
            }
            else
            {
                rend_Colour.material.color = Color.white;
            }
        
	}

    public void FindNeighbours(Tile startTile)
    {
        int x = this.int_X;
        int z = this.int_Z;
        CheckTiles(x, z + 1);
        CheckTiles(x, z - 1);
        CheckTiles(x + 1, z);
        CheckTiles(x - 1, z);

    }

    public void CheckTiles(int cX, int cZ)
    {
        Tile tT = null;
        if (cX >= 0 && cX <= 9 && cZ >= 0 && cZ <= 9)
        {
            tT = CSGameManager.gameManager.map[cX, cZ];
            ls_Tile_Neighbours.Add(tT);
        }
    }

    private void OnMouseUp()
    {
        if (bl_Walking_Selection)
        {
            Robot rob = CSGameManager.gameManager.currentRobot;
            rob.transform.position = new Vector3(int_X, rob.transform.position.y, int_Z);
            rob.int_x = int_X;
            rob.int_z = int_Z;
            rob.Clear_Selection();

        }

    }

    private void OnMouseOver()
    {
        if (bl_Attack_Selection)
        {
            Robot rob = CSGameManager.gameManager.currentRobot;

            RaycastHit hit;

            Vector3 dir = new Vector3(transform.position.x, 1, transform.position.z) - rob.transform.position;
            dir = dir.normalized;
            float fl_Ray_Range = Vector3.Distance(new Vector3(transform.position.x, 1, transform.position.z), rob.transform.position);

            Ray ray_cast = new Ray(rob.transform.position, dir * fl_Ray_Range);

            Debug.DrawRay(rob.transform.position, dir * fl_Ray_Range, Color.green, 0.1f);

            if(Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray_cast, out hit, fl_Ray_Range))
                {

                    if(hit.collider.gameObject.GetComponent<Tile>())
                    {
                        hit.collider.gameObject.GetComponent<Tile>().RemoveTile();
                    }

                }
            }
            //rob.Clear_Selection();
        }
    }

    public void RemoveTile()
    {
        Destroy(gameObject.transform.GetChild(int_Child).gameObject);
        int_Child++;
        Debug.Log("Fire!");
        Instantiate(Resources.Load<GameObject>("MapParts/MapElement_" + 0), gameObject.transform);
        bl_Is_Walkable = true;
        gameObject.GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);
        rend_Colour = gameObject.transform.GetChild(int_Child).GetComponent<Renderer>();
        CSGameManager.gameManager.RefreshTile();

    }
}
