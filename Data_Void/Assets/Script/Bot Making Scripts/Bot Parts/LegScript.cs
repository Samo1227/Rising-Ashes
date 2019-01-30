﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegScript : MonoBehaviour
{
    public int int_part_max_move;
    public int int_part_min_move;

    public int int_part_weight_limit;

    void Awake()
    {
        if (GetComponentInParent<PlayerRobot>() != null)
        {
            PlayerRobot pr_player = GetComponentInParent<PlayerRobot>();

            pr_player.int_Weight_Max = int_part_weight_limit;
            pr_player.int_Move_Max = int_part_max_move;
            pr_player.int_Move_Min = int_part_min_move;
        }
    }
}
