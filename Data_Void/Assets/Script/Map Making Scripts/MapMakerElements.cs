using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMakerElements : MonoBehaviour {


    //Collider cl_bo

    //Tiles location in the array
    public int loc_x;
    public int loc_y;
    //Tiles type of element
    public int int_type;
    //script that this is being made by 
    public MapMaker mM_map;
    //Map Element Game Obejcts from resources 
    public GameObject[] go_arr_map_element_prefab;
    //Map Element Game Obejcts in this gameobject
    public GameObject[] go_arr_map_element;

    //Sent from the map maker script that created this object
    public void GetMap(MapMaker mM)
    {
        mM_map = mM;
    }
    //Set location in the map maker array of this obeject 
    public void SetLocation(int x, int y)
    {
        loc_x = x;
        loc_y = y;
    }

    // Use this for initialization
    void Start()
    {
        go_arr_map_element_prefab = new GameObject[mM_map.int_map_parts_max];
        go_arr_map_element = new GameObject[mM_map.int_map_parts_max];

        for (int i = 0; i < mM_map.int_map_parts_max; i++)
        {
            go_arr_map_element_prefab[i] = Resources.Load<GameObject>("MapParts/MapElement_" + i);
        }

        GameObject temp_go = null;

        for (int i = 0; i < mM_map.int_map_parts_max; i++)
        {
            temp_go = Instantiate(go_arr_map_element_prefab[i], new Vector3(transform.position.x,0, transform.position.z), Quaternion.identity, gameObject.transform) as GameObject;
            go_arr_map_element[i] = temp_go;

            if (i != 0)
            {
                temp_go.SetActive(false);
            }

        }


    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireCube(new Vector3(transform.position.x, 0.5f, transform.position.z), new Vector3(1, 0, 1));
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButton(0))
        {
            Debug.Log("yee");
            //TypeChangeCycle();
            MapChange();
        }
    }




    public void TypeChangeCycle()
    {
        if (int_type < mM_map.int_map_parts_max -1)
        {
            int_type++;
        }
        else
        {
            int_type = 0;
        }

        mM_map.ChangeMap(loc_x, loc_y, int_type);
        
        //sets what type of head is active
        for (int i = 0; i < mM_map.int_map_parts_max; i++)
        {
            //Turns off everything that is not the current head int
            if (i != int_type)
            {
                go_arr_map_element[i].SetActive(false);
            }
            //Turns on the current head int
            else
            {
                go_arr_map_element[i].SetActive(true);
            }
        }
    }

    void MapChange()
    {

        int_type = mM_map.int_type_shift;

        mM_map.ChangeMap(loc_x, loc_y, int_type);

        //sets what type of head is active
        for (int i = 0; i < mM_map.int_map_parts_max; i++)
        {
            //Turns off everything that is not the current head int
            if (i != int_type)
            {
                go_arr_map_element[i].SetActive(false);
            }
            //Turns on the current head int
            else
            {
                go_arr_map_element[i].SetActive(true);
            }
        }
    }

}
