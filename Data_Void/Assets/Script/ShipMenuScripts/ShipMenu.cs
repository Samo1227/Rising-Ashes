using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMenu : MonoBehaviour
{
    public LevelSelect[] go_Levels;

    void Awake()
    {
        go_Levels[0].bl_Accessable = true;
        for (int i = 0; i < ProgressTracker.pt_ProgressTracker.bl_LevelsCompleted.Length; i++)
        {
            
            if (ProgressTracker.pt_ProgressTracker.bl_LevelsCompleted[i] == true)
            {
                if(i>go_Levels.Length)
                    go_Levels[i].bl_Completed = true;
            }
        }
    }
    
    void Update()
    {
        
    }
}
