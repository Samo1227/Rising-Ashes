using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class IntroPlayerBot : CharacterBase
{
    public GameObject go_Targetmove;
    public GameObject go_TargetAttack;
    public bool bl_isFinished = false;
    public bool bl_canMove = false;
    public float stepSpeed = 5f;
    bool bl_CarveGroovePlease=false;
    public Flowchart flowchart;
    private bool bl_SelectedRob = false;

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
        StartCoroutine(FindVeiwableTiles());
        go_TargetAttack = _go_CubeTemp;
        flowchart = FindObjectOfType<Flowchart>();
    }
    private void Update()
    {
        if (bl_isFinished)
            return;
        if (flowchart.GetBooleanVariable("move") == true)
        {
            FindMoveTiles();
            TurnOnTargetMoveTile();
            flowchart.SetBooleanVariable("move",false);
        }
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
                flowchart.ExecuteBlock("NextToTarget");
                bl_CarveGroovePlease = true;
                go_TargetAttack.SetActive(true);
            }
        }
    }
    private void OnMouseUp()
    {
        if (bl_SelectedRob == true)
            return;
        if (bl_isFinished)
            return;

        flowchart.ExecuteBlock("Movetothere");
        bl_SelectedRob = true;
    
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

    public IEnumerator FindVeiwableTiles()//player robot version
    {

        Tile currentTile = CSGameManager.gameManager.map[this.int_x, this.int_z];//this shouldn't really be neccessary as it's set outside of this, but this is just to be safe

        Queue<Tile> process = new Queue<Tile>();//processes all the tiles this can move to, runs till no possible tiles are left
        process.Enqueue(currentTile);//starts with the current tile this is on
                                     //-----------
            while (process.Count > 0)
            {
                Tile tempTile = process.Dequeue(); //takes the tile out of the queue and processes it
                                                   //---------
                if (tempTile.bl_opaque == false)   //if the tile is walkable add it to the selectable process
                {

                    selectableTiles.Add(tempTile);
                    tempTile.bl_in_view_zone = true;

                    if (tempTile.go_fog != null)
                    {
                        tempTile.go_fog.enabled = !tempTile.bl_in_view_zone;
                    }

                    if (int_Veiw_Type == 3)
                    {
                        tempTile.bl_tag = true;
                    }

                    //----------
                    if (tempTile.int_Distance_From_Start < int_Veiw_Distance) // if it's within move range -1 check the neighbours
                    {
                        //----------
                        foreach (Tile neighbourTile in tempTile.ls_Tile_Neighbours)//for every neihbour, check  
                        {
                            //----------
                            if (neighbourTile != currentTile)//stops path getting stuck looping
                            {
                                //----------
                                if (neighbourTile.tl_Start_Tile == null)//only do if there isn't already a start tile for this tile, this should prevent infinite looping 
                                    neighbourTile.tl_Start_Tile = tempTile;
                                //----------
                            }
                            //----------
                            neighbourTile.int_Distance_From_Start = neighbourTile.int_Move_Cost + tempTile.int_Distance_From_Start;//sets the neighbour tiles distance from the start point for move distance limits
                                                                                                                                   //----------
                            if (neighbourTile.int_Distance_From_Start <= int_Veiw_Distance)//if the neighbours are within movement range add them to the process queue
                            {

                                process.Enqueue(neighbourTile);//add any walkable neighbours to the queue to process their neighbours
                            }
                        }
                        //----------
                    }
                    //----------
                }
                //----------
            
        }
        //----------
        foreach (AICharacter tl_Temp_ai in CSGameManager.gameManager.ls_AI_Characters_In_Level)//will be slower with begger maps, will have to test to see if this is a problem
        {
            tl_Temp_ai.VisibleEnemy();
        }
        yield return null;
    }
    public void FinalFungusScene()
    {
        flowchart.ExecuteBlock("Final");
    }
}
