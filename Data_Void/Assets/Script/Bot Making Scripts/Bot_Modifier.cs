using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public bool[] bl_arms;
    public bool[] bl_bodies;
    public bool[] bl_heads;
    public bool[] bl_legs;

    public string[] st_arr_resources;
    public int int_bots_spawned;
    public GameObject go_bot_maker_open_button;

    public Text[] ui_txt_text;
    public GameObject go_leg_weight_limit_bar;

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

        bl_arms = new bool[int_part_array_max];
        bl_bodies = new bool[int_part_array_max];
        bl_heads = new bool[int_part_array_max];
        bl_legs = new bool[int_part_array_max];



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

        #region PartText
        string[] temp_text;

        temp_text = go_arr_heads[int_body_type[0]].GetComponent<HeadScript>().txt_info.text.Split(new string[] { "," }, System.StringSplitOptions.None);

        ui_txt_text[0].text =
        "Head : " + temp_text[0] + "\n" +
        "Veiw Distance : " + go_arr_heads[int_body_type[0]].GetComponent<HeadScript>().int_part_veiw_distance + "\n" +
        "Effect : " + "\n" + temp_text[1] + "\n" + "\n" +
        "Weight : " + go_arr_heads[int_body_type[0]].GetComponent<HeadScript>().int_part_weight
        ;
        //-------------------------------------------------------
        temp_text = go_arr_bodies[int_body_type[1]].GetComponent<BodyScript>().txt_info.text.Split(new string[] { "," }, System.StringSplitOptions.None);

        ui_txt_text[1].text =
        "Body : " + temp_text[0] + "\n" +
        "Health : " + go_arr_bodies[int_body_type[1]].GetComponent<BodyScript>().int_part_health + "\n"+
        "Effect : " + "\n" + temp_text[1] + "\n" + "\n" +
        "Weight : " + go_arr_bodies[int_body_type[1]].GetComponent<BodyScript>().int_part_weight
        ;
        //-------------------------------------------------------
        temp_text = go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().txt_info.text.Split(new string[] { "," }, System.StringSplitOptions.None);

        ui_txt_text[2].text =
        "Weapon : " + temp_text[0] + "\n" +
        "Range : " + go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().int_part_range + "\n" +
        "Base Damage : " + go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().int_part_damage_bracket[0] + "-" + go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().int_part_damage_bracket[1] + "\n" +
        "Base Effect : " + "\n" + temp_text[1] + "\n" + "\n" +
        "Overheat Damage : " + go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().int_part_overheat_damage_bracket[0] + "-" + go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().int_part_overheat_damage_bracket[1] + "\n" +
        "Overheat Effect : " + "\n" + temp_text[2] + "\n" + "\n" +
        "Weight : " + go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().int_part_weight
        ;
        //-------------------------------------------------------
        temp_text = go_arr_legs[int_body_type[3]].GetComponent<LegScript>().txt_info.text.Split(new string[] { "," }, System.StringSplitOptions.None);

        ui_txt_text[3].text =
        "Legs : " + temp_text[0] + "\n" +
        "Weight Limit : " + go_arr_legs[int_body_type[3]].GetComponent<LegScript>().int_part_weight_limit + "\n" +
        "Max Move : " + go_arr_legs[int_body_type[3]].GetComponent<LegScript>().int_part_max_move + " Min Move : " + go_arr_legs[int_body_type[3]].GetComponent<LegScript>().int_part_min_move + "\n" +
        "Effect : " + "\n" + temp_text[1] + "\n"
        ;
        #endregion
        WeightCalc();
    }

    #region Part Changes
    //On button press change head
    public void HeadChange()
    {
        string[] temp_text;
        //Adds to head int array
        int_body_type[0]++;

        //sets the head back to 0
        if (int_body_type[0] >= int_part_array_max)
        {
            int_body_type[0] = 0;
        }
        if (bl_heads[int_body_type[0]] == true)
        {
            int_body_type[0]++;
        }

        temp_text = go_arr_heads[int_body_type[0]].GetComponent<HeadScript>().txt_info.text.Split(new string[] { "," }, System.StringSplitOptions.None);

        ui_txt_text[0].text =
        "Head : " + temp_text[0] + "\n" +
        "Veiw Distance : " + go_arr_heads[int_body_type[0]].GetComponent<HeadScript>().int_part_veiw_distance + "\n" +
        "Effect : " + "\n" + temp_text[1] + "\n" + "\n" +
        "Weight : " + go_arr_heads[int_body_type[0]].GetComponent<HeadScript>().int_part_weight;

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
        WeightCalc();
    }
    //On button press change body
    public void BodyChange()
    {
        string[] temp_text;
        //Adds to body int array
        int_body_type[1]++;
        //sets the body back to 0
        if (int_body_type[1] >= int_part_array_max)
        {
            int_body_type[1] = 0;
        }
        if (bl_bodies[int_body_type[1]] == true)
        {
            int_body_type[1]++;
        }

        temp_text = go_arr_bodies[int_body_type[1]].GetComponent<BodyScript>().txt_info.text.Split(new string[] { "," }, System.StringSplitOptions.None);

        ui_txt_text[1].text =
        "Body : " + temp_text[0] + "\n" +
        "Health : " + go_arr_bodies[int_body_type[1]].GetComponent<BodyScript>().int_part_health + "\n" +
        "Effect : " + "\n" + temp_text[1] + "\n" + "\n" +
        "Weight : " + go_arr_bodies[int_body_type[1]].GetComponent<BodyScript>().int_part_weight;

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
        WeightCalc();
    }
    //On button press change arms
    public void ArmChange()
    {
        string[] temp_text;
        //Adds to arms int array
        int_body_type[2]++;
        //sets the arms back to 0
        if (int_body_type[2] >= int_part_array_max)
        {
            int_body_type[2] = 0;
        }
        if (bl_arms[int_body_type[2]] == true)
        {
            int_body_type[2]++;
        }

        temp_text = go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().txt_info.text.Split(new string[] { "," }, System.StringSplitOptions.None);

        ui_txt_text[2].text =
        "Weapon : " + temp_text[0] + "\n" +
        "Range : " + go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().int_part_range + "\n" +
        "Base Damage : " + go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().int_part_damage_bracket[0] + " - " + go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().int_part_damage_bracket[1] + "\n" +
        "Base Effect : " + "\n" + temp_text[1] + "\n" + "\n" +
        "Overheat Damage : " + go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().int_part_overheat_damage_bracket[0] + " - " + go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().int_part_overheat_damage_bracket[1] + "\n" +
        "Overheat Effect : " + "\n" + temp_text[2] + "\n" + "\n" +
        "Weight : " + go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().int_part_weight

        ;

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
        WeightCalc();
    }
    //On button press change legs
    public void LegChange()
    {
        string[] temp_text;
        //Adds to legs int array
        int_body_type[3]++;

        //sets the legs back to 0
        if (int_body_type[3] >= int_part_array_max)
        {
            int_body_type[3] = 0;
        }

        if (bl_legs[int_body_type[3]] == true)
        {
            int_body_type[3]++;
        }

        temp_text = go_arr_legs[int_body_type[3]].GetComponent<LegScript>().txt_info.text.Split(new string[] { "," }, System.StringSplitOptions.None);

        ui_txt_text[3].text =
        "Legs : " + temp_text[0] + "\n" +
        "Weight Limit : " + go_arr_legs[int_body_type[3]].GetComponent<LegScript>().int_part_weight_limit + "\n" +
        "Max Move : " + go_arr_legs[int_body_type[3]].GetComponent<LegScript>().int_part_max_move + " Min Move : " + go_arr_legs[int_body_type[3]].GetComponent<LegScript>().int_part_min_move + "\n" +
        "Effect : " + "\n" + temp_text[1] + "\n"
        ;


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
        WeightCalc();
    }
    #endregion

    void WeightCalc()
    {
        int temp_int_max = go_arr_legs[int_body_type[3]].GetComponent<LegScript>().int_part_weight_limit;
        int temp_int = go_arr_heads[int_body_type[0]].GetComponent<HeadScript>().int_part_weight + go_arr_bodies[int_body_type[1]].GetComponent<BodyScript>().int_part_weight + go_arr_arms[int_body_type[2]].GetComponent<WeaponScript>().int_part_weight;

        if (temp_int_max <= temp_int)
        {
            go_leg_weight_limit_bar.GetComponentInChildren<Image>().color = Color.red;
        }
        else
        {
            go_leg_weight_limit_bar.GetComponentInChildren<Image>().color = Color.green;
        }

        go_leg_weight_limit_bar.transform.localScale = new Vector3((1f / temp_int_max) * temp_int, 1, 1);
    }

    //Makes the playable robot
    public void BotPrint()
    {
        #region OldMethod
        /*
        //temp gameobject
        GameObject go_temp = null;
        //temp player script
        PlayerRobot pr_in_game = null;
        //adds to number of robots made
        int_bots_spawned++;
        //Creates robot and stores it in the temp gameobject
        Instantiate(pr_in_game);
        //changes the name of the robot
        go_temp.name = "Robot_" + int_bots_spawned;
        //gets script from robot
        pr_in_game = go_temp.GetComponent<PlayerRobot>();        
        for (int i = 0; i < int_part_array_max; i++)
        {
            pr_in_game.int_arr_parts[i] = int_body_type[i];
        }
        */
        #endregion
        if (bl_heads[int_body_type[0]] == true || bl_bodies[int_body_type[1]] == true || bl_arms[int_body_type[2]] == true || bl_legs[int_body_type[3]] == true)
            return;
        //spawns the same type of parts on new robot that are on this script
        CSGameManager.gameManager.AddRobot(1, 1, int_body_type);

        bl_heads[int_body_type[0]] = true;
        bl_bodies[int_body_type[1]] = true;
        bl_arms[int_body_type[2]] = true;
        bl_legs[int_body_type[3]] = true;

        WeightCalc();

        /*
        int_body_type[0]++;
        int_body_type[1]++;
        int_body_type[2]++;
        int_body_type[3]++;
        */


    }
    public void CloseBotBuilder()
    {
        go_bot_maker_open_button.SetActive(true);
        gameObject.SetActive(false);
    }

}