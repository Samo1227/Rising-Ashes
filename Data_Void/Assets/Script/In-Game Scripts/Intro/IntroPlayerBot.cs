using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPlayerBot : CharacterBase
{
    public GameObject go_Targetmove;
    public GameObject go_TargetAttack;
    public bool bl_isFinished = false;
    public bool bl_canMove = false;
    public float stepSpeed = 5f;
    bool bl_CarveGroovePlease=false;

    private void Start()
    {
        GameObject _go_temp = Instantiate(go_Targetmove);
        _go_temp.transform.position = new Vector3(4, 0.51f, 8);
        _go_temp.SetActive(false);
        _go_temp.GetComponent<IntroTarget>().ipb_PR = this;
        go_Targetmove = _go_temp;
        GameObject _go_CubeTemp = Instantiate(go_TargetAttack);
        _go_CubeTemp.transform.position = new Vector3(4, 0, 9);
        _go_CubeTemp.SetActive(false);
        _go_CubeTemp.GetComponent<IntroAttackTarget>().ipb_PR = this;
        go_TargetAttack = _go_CubeTemp;
    }
    private void Update()
    {
        if (bl_isFinished)
            return;
        if (bl_CarveGroovePlease == false)
        {
            if (Vector3.Distance(transform.position, new Vector3(go_Targetmove.transform.position.x, 1, go_Targetmove.transform.position.z)) <= 0.1f)
            {
                Clear_Selection();
                go_Targetmove.SetActive(false);
                int x = Mathf.RoundToInt(transform.position.x);
                int z = Mathf.RoundToInt(transform.position.z);
                tl_Current_Tile = CSGameManager.gameManager.map[x, z];
                int_x = x;
                int_z = z;
                Find_Attack_Tile_Range();
                //fungus dialogue
                bl_CarveGroovePlease = true;
                go_TargetAttack.SetActive(true);
            }
        }
    }
    private void OnMouseUp()
    {
        if (bl_isFinished)
            return;
        FindMoveTiles();
        TurnOnTargetMoveTile();
        //fungus dialogue
        bl_canMove = true;
    }
    public void TurnOnTargetMoveTile()
    {
        go_Targetmove.SetActive(true);
    }
   
    public IEnumerator GoTherePlease()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(go_Targetmove.transform.position.x, transform.position.y, go_Targetmove.transform.position.z), stepSpeed * Time.deltaTime);
            yield return null;
        }

    }
}
