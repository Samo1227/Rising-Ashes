using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadScript : MonoBehaviour
{
    public int int_part_veiw_distance;
    public int int_part_weight;

    public int int_part_effect;

    public TextAsset txt_info;

    void Awake()
    {
        if (GetComponentInParent<PlayerRobot>() != null)
        {
            PlayerRobot pr_player = GetComponentInParent<PlayerRobot>();

            pr_player.int_Veiw_Distance = int_part_veiw_distance;
            pr_player.int_Weight_Current += int_part_weight;
            pr_player.int_head_effect = int_part_effect;

        }
        if (GetComponentInParent<Bot_Modifier>() != null)
        {

        }
    }
}