using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public int int_part_weight;
    public int int_part_damage;
    public int int_part_effect;
    public int int_part_range;
    public int[] int_part_heat_range;
    public int int_part_heat_max;

    void Awake()
    {
        if (GetComponentInParent<PlayerRobot>() != null)
        {
            PlayerRobot pr_player = GetComponentInParent<PlayerRobot>();

            pr_player.int_Weight_Current += int_part_weight;
            pr_player.int_damage = int_part_damage;
            pr_player.int_effect = int_part_effect;
            pr_player.int_Attack_Range = int_part_range;
            pr_player.int_heat_range = int_part_heat_range;
            pr_player.int_heat_total = int_part_heat_max;
        }
    }
}