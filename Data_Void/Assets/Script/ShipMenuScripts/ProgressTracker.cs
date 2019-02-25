using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressTracker : MonoBehaviour
{
    //---------------------------------------------------
    public static ProgressTracker pt_ProgressTracker; //gamemanager singleton
    public int int_NumberOfLevels = 2;
    public bool[] bl_LevelsCompleted;
    //---------------------------------------------------
   
    private void Awake()            // runs before start
    {
        //-----------
        if (pt_ProgressTracker == null)            // has it been set up before?
        {
            pt_ProgressTracker = this;             // no, it's the first GM, so store our instance
                                                   //  DontDestroyOnLoad(gameObject);// persists through sceen changes
        }
        //-----------
        else if (pt_ProgressTracker != this) // if we get called again. desroy new version and keep old
        {
            Destroy(gameObject); // kill subsequent versions
        }
        //-----------
        System.Array.Resize<bool>(ref bl_LevelsCompleted, int_NumberOfLevels);

    }
    //---------------------------------------------------
    public void LevelComplete(int _Level)
    {
        bl_LevelsCompleted[_Level] = true;
    }
    //---------------------------------------------------
}