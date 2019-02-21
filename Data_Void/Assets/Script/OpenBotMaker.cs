using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBotMaker : MonoBehaviour
{
    public GameObject go_bot_maker;

    public void OpenBotmMaker()
    {
        go_bot_maker.SetActive(true);
        gameObject.SetActive(false);
        if (go_bot_maker.GetComponent<Bot_Modifier>().bl_heads[go_bot_maker.GetComponent<Bot_Modifier>().int_body_type[0]] == true 
         || go_bot_maker.GetComponent<Bot_Modifier>().bl_bodies[go_bot_maker.GetComponent<Bot_Modifier>().int_body_type[1]] == true 
         || go_bot_maker.GetComponent<Bot_Modifier>().bl_arms[go_bot_maker.GetComponent<Bot_Modifier>().int_body_type[2]] == true 
         || go_bot_maker.GetComponent<Bot_Modifier>().bl_legs[go_bot_maker.GetComponent<Bot_Modifier>().int_body_type[3]] == true)
        {
            go_bot_maker.GetComponent<Bot_Modifier>().HeadChange();
            go_bot_maker.GetComponent<Bot_Modifier>().BodyChange();
            go_bot_maker.GetComponent<Bot_Modifier>().ArmChange();
            go_bot_maker.GetComponent<Bot_Modifier>().LegChange();
        }

        CSGameManager.gameManager.bl_storing_robot = false;

        for (int i = 0; i < 5; i++)
        {
            if (go_bot_maker.GetComponent<Bot_Modifier>().bl_heads[i] == false)
            {
                return;
            }
            else if (go_bot_maker.GetComponent<Bot_Modifier>().bl_bodies[i] == false)
            {
                return;
            }
            else if (go_bot_maker.GetComponent<Bot_Modifier>().bl_arms[i] == false)
            {
                return;
            }
            else if (go_bot_maker.GetComponent<Bot_Modifier>().bl_legs[i] == false)
            {
                return;
            }
        }
        go_bot_maker.SetActive(false);
        gameObject.SetActive(false);
    }
}
