using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMenu : MonoBehaviour
{
    public LevelSelect[] go_Levels;
   public  bool levlesChecked = false;

    void Start()
    {
        levlesChecked = false;
        go_Levels[0].bl_Accessable = true;
        go_Levels[1].bl_Accessable = true;
        for (int i = 0; i < ProgressTracker.pt_ProgressTracker.bl_LevelsCompleted.Length; i++)
        {
            Debug.Log("checkingLevels");
            if (ProgressTracker.pt_ProgressTracker.bl_LevelsCompleted[i] == true)
            {
                if (i > go_Levels.Length)
                {
                    go_Levels[i].bl_Completed = true;
                    go_Levels[i + 1].bl_Accessable = true;
                }
            }
        }
    }
    
    void Update()
    { if (levlesChecked == true)
            return;

        for (int i = 0; i < ProgressTracker.pt_ProgressTracker.bl_LevelsCompleted.Length; i++)
        {
            Debug.Log("checkingLevels");
            if (ProgressTracker.pt_ProgressTracker.bl_LevelsCompleted[i] == true)
            {
                if (i > go_Levels.Length)
                {
                    go_Levels[i].bl_Completed = true;
                    go_Levels[i + 1].bl_Accessable = true;
                }
            }
            levlesChecked = true;
        }
    }
}
