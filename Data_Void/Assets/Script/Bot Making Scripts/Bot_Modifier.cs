using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot_Modifier : MonoBehaviour
{
    //the size of the part arrays
    public int int_part_array_max;
    //
    public int[] int_body_type;

    //holds the prefabs for the robot parts
    public GameObject[] go_arr_arms_prefabs;
    public GameObject[] go_arr_bodies_prefabs;
    public GameObject[] go_arr_heads_prefabs;
    public GameObject[] go_arr_legs_prefabs;

    //holds the spawned instances for the robot parts
    public GameObject[] go_arr_arms;
    public GameObject[] go_arr_bodies;
    public GameObject[] go_arr_heads;
    public GameObject[] go_arr_legs;
    //holds the location of the differnt body parts
    public Transform[] tr_arr_parts;

    public string[] st_arr_resources;

    public GameObject go_ingame_bot;

    public int int_bots_spawned;
    
    private void Start()
    {
        //Makes all of the robot part arrays the same size
        #region SetArraySize
        //Will only ever be 4
        int_body_type = new int[4];

        go_arr_arms_prefabs = new GameObject[int_part_array_max];
        go_arr_bodies_prefabs = new GameObject[int_part_array_max];
        go_arr_heads_prefabs = new GameObject[int_part_array_max];
        go_arr_legs_prefabs = new GameObject[int_part_array_max];

        go_arr_arms = new GameObject[int_part_array_max];
        go_arr_bodies = new GameObject[int_part_array_max];
        go_arr_heads = new GameObject[int_part_array_max];
        go_arr_legs= new GameObject[int_part_array_max];

        #endregion
        //Gets robot part Prefabs from Resources
        #region GetPartPrefabs
        //Grabs a customisable amount of robot parts for every body part
        for (int i = 0; i < int_part_array_max; i++)
        {
            go_arr_arms_prefabs[i] = Resources.Load<GameObject>("BotBodyParts/Arms/Arms_" + i);
            go_arr_bodies_prefabs[i] = Resources.Load<GameObject>("BotBodyParts/Bodies/Body_" + i);
            go_arr_heads_prefabs[i] = Resources.Load<GameObject>("BotBodyParts/Heads/Head_" + i);
            go_arr_legs_prefabs[i] = Resources.Load<GameObject>("BotBodyParts/Legs/Legs_" + i);
        }
        #endregion
        //Puts the Prefabs in the scene and turns them off
        #region LoadPartsInScene
        GameObject temp_go = null;
        
        //Spawns the parts on the robot
        for (int i = 0; i < tr_arr_parts.Length; i++)
        {
            for (int j = 0; j < int_part_array_max; j++)
            {
                //Head Parts
                if(i == 0)
                {
                    temp_go = Instantiate(go_arr_heads_prefabs[j], new Vector3(tr_arr_parts[i].position.x, tr_arr_parts[i].position.y, tr_arr_parts[i].position.z), tr_arr_parts[i].rotation, tr_arr_parts[i]) as GameObject;
                    go_arr_heads[j] = temp_go;
                }
                //Body Parts
                else if (i == 1)
                {
                    temp_go = Instantiate(go_arr_bodies_prefabs[j], new Vector3(tr_arr_parts[i].position.x, tr_arr_parts[i].position.y, tr_arr_parts[i].position.z), tr_arr_parts[i].rotation, tr_arr_parts[i]) as GameObject;
                    go_arr_bodies[j] = temp_go;
                }
                //Arm Parts
                else if (i == 2)
                {
                    temp_go = Instantiate(go_arr_arms_prefabs[j], new Vector3(tr_arr_parts[i].position.x, tr_arr_parts[i].position.y, tr_arr_parts[i].position.z), tr_arr_parts[i].rotation, tr_arr_parts[i]) as GameObject;
                    go_arr_arms[j] = temp_go;
                }
                //Leg Parts
                else if (i == 3)
                {
                    temp_go = Instantiate(go_arr_legs_prefabs[j], new Vector3(tr_arr_parts[i].position.x, tr_arr_parts[i].position.y, tr_arr_parts[i].position.z), tr_arr_parts[i].rotation, tr_arr_parts[i]) as GameObject;
                    go_arr_legs[j] = temp_go;
                }
                //turns all objects off apart from 0
                if(j != 0)
                {
                    temp_go.SetActive(false);
                }
                //Stops all objects from casting a shadow
                temp_go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
        #endregion

    }

    #region Part Changes
    //On button press change head
    public void HeadChange()
    {
        //Adds to head int array
        int_body_type[0]++;
        //sets the head back to 0
        if(int_body_type[0] >= int_part_array_max)
        {
            int_body_type[0] = 0;
        }
        //sets what type of head is active
        for(int i = 0; i < int_part_array_max; i++)
        {
            //Turns off everything that is not the current head int
            if (i != int_body_type[0])
            {
                go_arr_heads[i].SetActive(false);
            }
            //Turns on the current head int
            else
            {
                go_arr_heads[i].SetActive(true);
            }
        }
    }
    //On button press change body
    public void BodyChange()
    {
        //Adds to body int array
        int_body_type[1]++;
        //sets the body back to 0
        if (int_body_type[1] >= int_part_array_max)
        {
            int_body_type[1] = 0;
        }
        //sets what type of body is active
        for (int i = 0; i < int_part_array_max; i++)
        {
            //Turns off everything that is not the current body int
            if (i != int_body_type[1])
            {
                go_arr_bodies[i].SetActive(false);
            }
            //Turns on the current body int
            else
            {
                go_arr_bodies[i].SetActive(true);
            }
        }
    }
    //On button press change arms
    public void ArmChange()
    {
        //Adds to arms int array
        int_body_type[2]++;
        //sets the arms back to 0
        if (int_body_type[2] >= int_part_array_max)
        {
            int_body_type[2] = 0;
        }
        //sets what type of arm is active
        for (int i = 0; i < int_part_array_max; i++)
        {
            //Turns off everything that is not the current arm int
            if (i != int_body_type[2])
            {
                go_arr_arms[i].SetActive(false);
            }
            //Turns on the current arm int
            else
            {
                go_arr_arms[i].SetActive(true);
            }
        }
    }
    //On button press change legs
    public void LegChange()
    {
        //Adds to legs int array
        int_body_type[3]++;
        //sets the legs back to 0
        if (int_body_type[3] >= int_part_array_max)
        {
            int_body_type[3] = 0;
        }
        //sets what type of leg is active
        for (int i = 0; i < int_part_array_max; i++)
        {
            //Turns off everything that is not the current leg int
            if (i != int_body_type[3])
            {
                go_arr_legs[i].SetActive(false);
            }
            //Turns on the current leg int
            else
            {
                go_arr_legs[i].SetActive(true);
            }
        }
    }
    #endregion
    //Makes the playable robot
    public void BotPrint()
    {
        //temp gameobject
        GameObject go_temp = null;
        //temp player script
        Playable_Bot pb_in_game = null;
        //adds to number of robots made
        int_bots_spawned++;
        //Creates robot and stores it in the temp gameobject

        go_temp = Instantiate(go_ingame_bot,new Vector3(2,1,0),Quaternion.identity) as GameObject;

        go_temp = Instantiate(go_ingame_bot,new Vector3(0,1,0),Quaternion.identity) as GameObject;







        //changes the name of the robot
        //go_temp.name = "Robot_" + int_bots_spawned;
        //gets script from robot
<<<<<<< HEAD
<<<<<<< HEAD
        pb_in_game = go_temp.GetComponent<Playable_Bot>();
        //spawns the same type of parts on new robot that are on this script
        for(int i = 0; i < int_part_array_max; i++)
=======
>>>>>>> parent of dad8b6b... Bot Builder and core game combined
=======
>>>>>>> parent of dad8b6b... Bot Builder and core game combined
        pr_in_game = go_temp.GetComponent<PlayerRobot>();

        CSGameManager.gameManager.AddRobot(0, 0, pr_in_game);
        //spawns the same type of parts on new robot that are on this script
        for (int i = 0; i < int_part_array_max; i++)

        {
            pb_in_game.int_arr_parts[i] = int_body_type[i];
        }
<<<<<<< HEAD
<<<<<<< HEAD

        CSGameManager.gameManager.AddRobot(0,0, pr_in_game);

=======
        CSGameManager.gameManager.AddRobot(0,0, pr_in_game);
>>>>>>> parent of dad8b6b... Bot Builder and core game combined
=======
        CSGameManager.gameManager.AddRobot(0,0, pr_in_game);
>>>>>>> parent of dad8b6b... Bot Builder and core game combined
    }

}