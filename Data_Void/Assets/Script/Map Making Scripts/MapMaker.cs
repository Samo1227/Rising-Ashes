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

    public int int_type_shift;

    public string st_level_name;

    public MapMakerElements[,] map = new MapMakerElements[10, 10];//at the moment the map array is limited to this size, this needs changing

    public TextAsset txt_level;
    public string st_level;
    public string[] arr_at_level;
    int[,] int_map_data = new int[10, 10];

    // Use this for initialization
    void Start ()
    {
        int_map_data = new int[map_x, map_z];

        /*
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
        */
        if (txt_level == null)
        {
            MakeMap();
        }
        else
        {
            TextToMapInt();
        }


    }

    void TextToMapInt()
    {
        st_level = txt_level.text;
        arr_at_level = st_level.Split(new string[] { "," }, System.StringSplitOptions.None);
        int temp_int = 0;

        map_x = System.Convert.ToInt32(arr_at_level[temp_int]);
        temp_int++;

        map_z = System.Convert.ToInt32(arr_at_level[temp_int]);
        temp_int++;

        map = new MapMakerElements[map_x, map_z];
        int_map_data = new int[map_x, map_z];

        //-----------
        for (int j = 0; j < map_z; j++)
        {
            //-----------
            for (int i = 0; i < map_x; i++)
            {
                Debug.Log(arr_at_level[temp_int]);
                int_map_data[i, j] = System.Convert.ToInt32(arr_at_level[temp_int]);
                temp_int++;
            }
            //-----------
        }
        //-----------
        MakeMap();
    }

    void MakeMap()
    {

        MapMaker mM_self = gameObject.GetComponent<MapMaker>();

        //-----------
        for (int z = 0; z < map_z; z++)
        {
            //-----------
            for (int x = 0; x < map_x; x++)
            {
                GameObject button = Instantiate(go_button, new Vector3((float)x * x_offset, 0, (float)z * z_offset), Quaternion.identity, gameObject.transform) as GameObject;

                MapMakerElements ui_button = button.GetComponent<MapMakerElements>();

                ui_button.SetLocation(x, z, int_map_data[x, z]);

                ui_button.GetMap(mM_self);

            }
            //-----------
        }
        //-----------

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
        string path = Application.dataPath + "/Map Text/Level_" + st_level_name + ".txt";
        string sr_temp = null;

        sr_temp += map_x + "," + map_z +"," + "\n";

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

        //print map_x map_z
        

    }

}
