using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Position
{
    north,
    south,
    east,
    west,
}

public class CamController : MonoBehaviour
{
    public Position position;
    public GameObject go_CamCont;
    public float fl_Min_MaxPos;
    public float fl_CamMoveSpeed=5f;

    private void OnMouseOver()
    {
        MoveCam();
    }

    public void MoveCam()
    {
        switch (position)
        {
            case (Position.north):
                if(go_CamCont.transform.position.y <= fl_Min_MaxPos)
                {
                    go_CamCont.transform.position += new Vector3(0, 1, 0)*fl_CamMoveSpeed * Time.deltaTime;
                }
                return;
            case (Position.south):
                if (go_CamCont.transform.position.y >= -fl_Min_MaxPos)
                {
                    go_CamCont.transform.position += new Vector3(0, -1, 0)*fl_CamMoveSpeed * Time.deltaTime;
                }
                return;
            case (Position.east):
                if (go_CamCont.transform.position.y <= fl_Min_MaxPos)
                {
                    go_CamCont.transform.position += new Vector3(1, 0, 0) * fl_CamMoveSpeed * Time.deltaTime;
                }
                return;
            case (Position.west):
                if (go_CamCont.transform.position.y >= -fl_Min_MaxPos)
                {
                    go_CamCont.transform.position += new Vector3(-1, 0, 0) * fl_CamMoveSpeed * Time.deltaTime;
                }
                return;
        }
    }
}
