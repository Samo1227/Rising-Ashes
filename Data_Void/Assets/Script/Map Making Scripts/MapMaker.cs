using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MapMaker : MonoBehaviour {

    public GameObject go_button;

    public int map_x;
    public int map_z;
    public int[,] int_ar_map;

    public float x_offset;
    public float z_offset;

    public int int_map_parts_max;

    public int[,] int_map_data = new int[20,20];

    public int int_type_shift;

    public int int_level;

    // Use this for initialization
    void Start ()
    {
        int_map_data = new int[map_x, map_z];

        MapMaker mM_self = gameObject.GetComponent<MapMaker>();

        for (int j = 0; j < map_z; j++)
        {
            for (int i = 0; i < map_x; i++)
            {
                GameObject button = Instantiate(go_button, new Vector3((float)i * x_offset,0, (float)j * z_offset), Quaternion.identity, gameObject.transform) as GameObject;

                MapMakerElements ui_button = button.GetComponent<MapMakerElements>();

                ui_button.SetLocation(i, j);

                ui_button.GetMap(mM_self);
            }
        }

    }
	
    public void ChangeMap(int data_x, int data_z, int int_status)
    {
        int_map_data[data_x,data_z] = int_status;
    }

    public void Reset()
    {
        //SceneManager. ("map maker");
    }

    public void TypeChange(int new_type)
    {
        int_type_shift = new_type;
    }

    public void PrintMap()
    {
        string str = "";

        for (int j = 0; j < map_z; j++)
        {
            for (int i = 0; i < map_x; i++)
            {
                if (i == 0)
                {
                    str += "{" + int_map_data[i,j];
                }
                else if (i == map_z-1)
                {
                    str += ", " + int_map_data[i, j] + "}, \n";
                }
                else
                {
                    str += ", " + int_map_data[i, j];
                }
            }
        }
        Debug.Log(str);
        CreateText();
    }

    void CreateText()
    {
        string path = Application.dataPath + "/Map Text/Level" + int_level + ".txt";
        string sr_temp = null;

        for (int j = 0; j < map_z; j++)
        {
            for (int i = 0; i < map_x; i++)
            {
                if (i == map_z - 1)
                {
                    sr_temp += int_map_data[i, j] +"," + "\n";

                }
                else
                {
                    sr_temp += int_map_data[i, j] + ",";
                }
            }
        }
        File.WriteAllText(path, sr_temp);

        

    }

}
