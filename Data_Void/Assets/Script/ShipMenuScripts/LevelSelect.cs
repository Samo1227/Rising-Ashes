using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public GameObject go_UIText;//ref to ui where info is displayed
    [TextArea]
    public string st_AreaDescription;//info on the level
    public string st_TakeToLevel;//name of scene this links to
    public bool bl_Completed = false;
    public bool bl_Accessable = false;
    public LevelSelect[] ls_linkedLevels;
    public float fl_FadeSpeed = 5f;
    Renderer r_Render;
    Color c_Red_Alpha = Color.red;

    private void Start()
    {
        r_Render = gameObject.GetComponent<Renderer>();
        if (bl_Completed == false)
        {
            for (int i = 0; i < ls_linkedLevels.Length; i++)
            {
                if (ls_linkedLevels[i].bl_Completed == true)
                {
                    bl_Accessable = true;
                }
            }
        }
    }

    public void CheckAccess()
    {
        if (bl_Completed == false)
        {
            for (int i = 0; i < ls_linkedLevels.Length; i++)
            {
                if (ls_linkedLevels[i].bl_Completed == true)
                {
                    bl_Accessable = true;
                }
            }
        }
    }

    void Update()
    {
        if (bl_Completed == true)
        {
            r_Render.material.color = Color.blue;
        }
        else if(bl_Accessable == true)
        {
            r_Render.material.color = Color.red;
            FadeInnOut();
        }
        else if (bl_Accessable == false)
        {
            r_Render.material.color = Color.gray;
        }
    }

    void FadeInnOut()
    {

        float f = (Mathf.Sin(fl_FadeSpeed * Time.time) + 1) / 2;
        c_Red_Alpha.a = f;
        r_Render.material.color=c_Red_Alpha;
    }

    private void OnMouseOver()
    {
        if (bl_Accessable == true)
        {
            go_UIText.GetComponentInChildren<Text>().text = st_AreaDescription;
            go_UIText.SetActive(true);
        }
    }
    private void OnMouseExit()
    {
        go_UIText.SetActive(false); 
    }

    private void OnMouseUpAsButton()
    {
        if (bl_Completed == true)
            return;//can't replay level
        if (bl_Accessable == true)
            SceneManager.LoadScene(st_TakeToLevel);//take to the level this links too
    }
}
