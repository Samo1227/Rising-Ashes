using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum AIStates          //an enum of AI states                            
{
    waiting,                  //do nothing but maybe make some checks state
    patrolling,               //Wander around state. not yet implemented
    chasing,                  //close in and attack
    retreating,               //make space and attack. 
    inactive,                 //Await for some kind of event trigger perhaps?
}

public class AICharacter : CharacterBase {
    //---------------------------------------------------
    #region Variables
    public PlayerRobot pr_Target; //character the AI will move towards and attack
    public Renderer rnd_Rendereer;
    public AIStates ais_CurrentState;

    public int int_ChaseRange = 6;
    public int int_TriggerRange = 2;
    public int int_RetreatRange = 0;

    public bool bl_is_invisible;
    public bool bl_is_tagged;
    #endregion
    //---------------------------------------------------
    #region Start
    void Start()
    {
        int_Health = int_Health_max;
      //  SetAttackRange(1);
      //  SetDamage(1);
        CSGameManager.gameManager.ls_AI_Characters_In_Level.Add(this);
        rnd_Rendereer = gameObject.transform.GetComponent<Renderer>();
        tl_Current_Tile = CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();
        tl_Current_Tile.bl_Occupied_By_AI = true;
        if (ais_CurrentState == AIStates.inactive)
            return;
        SetState(AIStates.waiting);
    }
    #endregion
    //---------------------------------------------------
    #region Update
    void Update()
    {
        //-----------
        if (CSGameManager.gameManager.ls_Player_Robots_In_Level.Count <= 0)
        {
            return;
        }
        //-----------
        if (int_Health <= 0)
        {
            AICharacterDestruction();
        }
        //-----------
        if (ais_CurrentState == AIStates.waiting)
        {
            if (int_Health < int_Health_max)
            {
                SetState(AIStates.chasing);
            }
        }
        //-----------
        if (fl_Turn_Timer >= fl_Time_limit)
        {
            CSGameManager.gameManager.EndAITurn();
        }
        //-----------
        if (bl_Is_Active)
        {
            //-----------
            if (bl_Turn_Just_Started)
            {
                pr_Target = null; //rework out target at the start of each turn
                bl_Turn_Just_Started = false;
            }
            DecideTurnAction();
        }
        //-----------

        go_health_bar.transform.localPosition = new Vector3(((float)int_Health - (float)int_Health_max) * (0.5f / int_Health_max), 0, 0);
        go_health_bar.transform.localScale = new Vector3((1f / int_Health_max) * int_Health, 0.2f, 1);

    }
    #endregion
    //---------------------------------------------------
    #region Turn Behaviour
    //---------------------------------------------------
    #region Waiting Behaviour
    void Waiting()
    {
        CheckPRisInPullRange();
    }
    void CheckPRisInPullRange()//not certain of the efficiency of this solution, it might be better to loop through all the PRs to find their distance instead
    {
        int layerMask = 1 << 9;//player robot layer
        Collider[] _hitColliders = Physics.OverlapSphere(transform.position, int_ChaseRange, layerMask);
        //-----------
        if (_hitColliders.Length != 0)//if there is at least one player robot in the chase range the AI will switch to chasing
        {
            print("I chase now");
            //    SetState(AIStates.chasing);
            TriggerNearbyAI();//this actually triggers itself... 
            FindNearestPlayerRobot();
            TakeChaseTurn();
        }
        //-----------
        else
        {
            EndTurn();
        }
        //-----------
    }
    //---------------------------------------------------
    void TriggerNearbyAI()//I feel this solution is better here as there could potentially be a huge number of AI characters to loop through?
    {
        int layerMask = 1 << 10;//enemy robot layer
        Collider[] _hitColliders = Physics.OverlapSphere(transform.position, int_TriggerRange, layerMask);
        //-----------
        if (_hitColliders.Length != 0)//if there is at least one AI robot in trigger range the AI in range will be switched to chasing
        {
            //-----------
            for (int i = 0; i < _hitColliders.Length; i++)
            {
                AICharacter _tempAI = null;
                _tempAI = _hitColliders[i].GetComponent<AICharacter>();
                //-----------
                if (_tempAI != null)
                {
                    print("Triggered" + _tempAI.gameObject.name);
                    _tempAI.SetState(AIStates.chasing);
                }
                //-----------
            }
            //-----------
        }
        //-----------
        else
        {
            Clear_Selection();
            pr_Target = null;
            bl_Is_Active = false;
            CSGameManager.gameManager.EndAITurn();
        }
        //-----------
    }
    #endregion
    //---------------------------------------------------
    #region Chasing Behaviour
    void TakeChaseTurn()
    {
       
        //-----------
        if (!bl_Moving)//don't keep checking for a target when moving
        {
            tl_Current_Tile.bl_Occupied_By_AI = false;
            CalculatePath();
            FindMoveTilesAI();//in theory should stop them from moving through players and stopping on other AI... 
            //-----------
            if (tl_Target_Square)//if it has a target square, start moving
            {
                bl_Moving = true;
            }
            //-----------
            else
            {   //this is what happens if there is no path to target square I guess...
                //attack and end turn
                FindAlternateRoute();
                //-----------
                if (tl_Target_Square != null)
                {
                    MoveToTargetSquare(tl_Target_Square);
                   // bl_Moving = true;
                }
                //-----------
                else
                {
                    Clear_Selection();//putting this everywhere now :S
                    Find_Attack_Tile_Range();
                    FindPRsInRange();
                    //-----------
                    if (ls_Dest_Tiles_In_Range.Count != 0)//should only get called when there is no player in range, in theory
                    {
                        FindTileTarget(pr_Target);
                    } 
                    //-----------
                    int_x = (int)transform.position.x;
                    int_z = (int)transform.position.z;
                    tl_Current_Tile = CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();
                    tl_Current_Tile.bl_Occupied_By_AI = true;
                    Find_Attack_Tile_Range();
                    FindPRsInRange();
                    EndTurn();
                }
            }
            //-----------
        }
        //-----------
        else
        {
            MoveToTarget();
            //-----------
            if (!bl_Moving)
            {
                Clear_Selection();//putting this everywhere now :S
                int_x = (int)transform.position.x;
                int_z = (int)transform.position.z;
                tl_Current_Tile = CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();
                tl_Current_Tile.bl_Occupied_By_AI = true;
                Find_Attack_Tile_Range();
                FindPRsInRange();
                //-----------
                if (ls_Dest_Tiles_In_Range.Count != 0)//should only get called when there is no player in range, in theory
                {
                    FindTileTarget(pr_Target);
                }
                //-----------
                EndTurn();
            }
            //-----------
        }
        //-----------

    }

    void FindAlternateRoute()
    {
        Tile _tile = null;
        float fl_NearestTileDistToTarget = Mathf.Infinity;
        //-----------
        for (int i = 0; i < selectableTiles.Count; i++)
        {
            _tile = selectableTiles[i];
            float _fl_DistanceToPR = Vector3.Distance(_tile.transform.position, pr_Target.transform.position);
            //-----------
            if (_fl_DistanceToPR < fl_NearestTileDistToTarget)
            {
                fl_NearestTileDistToTarget = _fl_DistanceToPR;
                tl_Target_Square = _tile;
            }
            //-----------
        }
        //-----------
    }
    #endregion
    //---------------------------------------------------
    #region RetreatBehaviour
    void RetreatBehaviour()
    {

        //-----------
        if (!bl_Moving)//don't keep checking for a target when moving
        {
            tl_Current_Tile.bl_Occupied_By_AI = false;
            FindMoveTilesAI();//in theory should stop them from moving through players and stopping on other AI... 
            FindRetreatTargetTile();
            //-----------
            if (tl_Target_Square)//if it has a target square, start moving
            {
                bl_Moving = true;
            }
            //-----------
            else
            {   //this is what happens if there is no path to target square I guess...
                //attack and end turn
                {
                    Clear_Selection();//putting this everywhere now :S
                    Find_Attack_Tile_Range();
                    FindPRsInRange();
                    //-----------
                    if (ls_Dest_Tiles_In_Range.Count != 0)//should only get called when there is no player in range, in theory
                    {
                        FindTileTarget(pr_Target);
                    }
                    //-----------
                    int_x = (int)transform.position.x;
                    int_z = (int)transform.position.z;
                    tl_Current_Tile = CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();
                    tl_Current_Tile.bl_Occupied_By_AI = true;
                    Find_Attack_Tile_Range();
                    FindPRsInRange();
                    EndTurn();
                }
            }
            //-----------
        }
        //-----------
        else
        {
            MoveToTarget();
            //-----------
            if (!bl_Moving)
            {
                Clear_Selection();//putting this everywhere now :S
                int_x = (int)transform.position.x;
                int_z = (int)transform.position.z;
                tl_Current_Tile = CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();
                tl_Current_Tile.bl_Occupied_By_AI = true;
                Find_Attack_Tile_Range();
                FindPRsInRange();
                //-----------
                if (ls_Dest_Tiles_In_Range.Count != 0)//should only get called when there is no player in range, in theory
                {
                    FindTileTarget(pr_Target);
                }
                //-----------
                EndTurn();
            }
            //-----------
        }
        //-----------
    } 
    #endregion
    #endregion
    //---------------------------------------------------
    #region FindNearest PR
    void FindNearestPlayerRobot()//this is a vary quick and simple way to find the nearest target, it's lacking in versatility and elegance
    {
        PlayerRobot pr_Nearest = null;
        float fl_Distance_To_Current_Target = Mathf.Infinity;

        //-----------
        foreach (PlayerRobot tPR in CSGameManager.gameManager.ls_Player_Robots_In_Level)
        {
            float fl_Distance_To_Target = Vector3.Distance(transform.position, tPR.transform.position); //inefficient way to check distance to all targets, also ignores obsticles

            if (fl_Distance_To_Target < fl_Distance_To_Current_Target) //if distance to the target it's looking at is lower that distance(to current target) 
                                                                       //-----------
            {
                fl_Distance_To_Current_Target = fl_Distance_To_Target;
                pr_Nearest = tPR; //nearest Player Robot = what it's looking at
            }
            //-----------
        }
        //-----------
        pr_Target = pr_Nearest; //once its looked at all PRs the closest one is target, long term needs to be different as there may be no path to closest target
        float fl_TargetDistance = Vector3.Distance(pr_Target.transform.position, transform.position);
        if (fl_TargetDistance <= int_RetreatRange)
        {
            SetState(AIStates.retreating);
            RetreatBehaviour();
           // return;
        }
        else
        {
            SetState(AIStates.chasing);
            TakeChaseTurn();
           // return;
        }
        
    }
    #endregion
    //---------------------------------------------------
    #region Calculate Path
    void CalculatePath()
    {
        Tile tl_Target_Tile = CSGameManager.gameManager.map[pr_Target.int_x, pr_Target.int_z].gameObject.GetComponent<Tile>();
        Tile tl_Current_Tile = CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();
        PathFinding(tl_Current_Tile, tl_Target_Tile);
    }
    #endregion
    //---------------------------------------------------
    #region RetreatTarget
    void FindRetreatTargetTile()
    {
        Tile tl_TargetTile = null;
        Tile _TargetTile = null;
        float fl_RetreatDistance = 0;
        for (int i = 0; i < selectableTiles.Count; i++)
        {
            _TargetTile = selectableTiles[i];
            float fl_DistFromTargetPR = Vector3.Distance(_TargetTile.transform.position, pr_Target.transform.position);
            if (fl_DistFromTargetPR > fl_RetreatDistance && fl_RetreatDistance <= int_RetreatRange)
            {
                tl_TargetTile = _TargetTile;
                fl_RetreatDistance = fl_DistFromTargetPR;
            }
        }
        MoveToTargetSquare(tl_TargetTile);
    } 
    #endregion
    //---------------------------------------------------
    #region Mouse Interaction
    private void OnMouseOver()//this doesn't work for shooting enemies
    {
        //-----------
        if (EventSystem.current.IsPointerOverGameObject())
            return;
            //-----------
            if (tl_Current_Tile.bl_Attack_Selection == true)
            {
            PlayerRobot rob = CSGameManager.gameManager.pr_currentRobot;//gets a reference to the currently selected player robot
            rnd_Rendereer.material.color = Color.yellow;
            RaycastHit[] hits = new RaycastHit[2];

            Vector3 dir = new Vector3(transform.position.x, 1, transform.position.z) - rob.transform.position;
            dir = dir.normalized;
            float fl_Ray_Range;
            Ray ray_cast;
            //-----------
            if (rob.int_effect == 0)
            {
                fl_Ray_Range = 1000;

                ray_cast = new Ray(rob.transform.position, dir * fl_Ray_Range);//draws a ray between target and selected robot

                Debug.DrawRay(rob.transform.position, dir * fl_Ray_Range, Color.green, 0.1f);//visual representation of ray for editor
            }
            //-----------
            else
            {
                fl_Ray_Range = Vector3.Distance(new Vector3(transform.position.x, 1, transform.position.z), rob.transform.position);

                ray_cast = new Ray(rob.transform.position, dir * fl_Ray_Range);//draws a ray between target and selected robot

                Debug.DrawRay(rob.transform.position, dir * fl_Ray_Range, Color.green, 0.1f);//visual representation of ray for editor
            }
            //-----------
            if (Input.GetMouseButtonDown(0))
            {
                //-----------
                if (Physics.RaycastNonAlloc(ray_cast, hits, fl_Ray_Range) > 0)
                {
                    //-----------
                    if (hits[0].collider.gameObject.GetComponent<CharacterBase>())//the collider hit is a tile
                    {
                        rob.RandomDamage();
                        rob.bl_Has_Acted = true;//robot has done it's action
                        rob.int_Actions--;
                        rob.Clear_Selection();//clear tile highlighting 
                        //-----------
                        if (rob.int_effect == 0)
                        {
                            rob.lr_laser.startWidth = rob.int_damage * 0.05f;
                            rob.lr_laser.endWidth = rob.int_damage * 0.05f;
                            rob.lr_laser.SetPosition(0, rob.transform.position);
                            rob.lr_laser.SetPosition(1, hits[0].point);
                            //-----------
                            if (rob.bl_overheat == false)
                            {
                                hits[0].collider.gameObject.GetComponent<CharacterBase>().int_Health -= rob.int_damage;
                                rob.int_heat_current += 1;
                                //-----------
                                if (hits[0].collider.gameObject.GetComponent<CharacterBase>().int_Health < 0)
                                {
                                    rob.lr_laser.SetPosition(1, hits[1].point);
                                    //-----------
                                    if (hits[1].collider.gameObject.GetComponent<Tile>())
                                    {
                                        hits[1].collider.gameObject.GetComponent<Tile>().int_health += hits[0].collider.gameObject.GetComponent<CharacterBase>().int_Health;
                                    }
                                    //-----------
                                    else if (hits[1].collider.gameObject.GetComponent<CharacterBase>())
                                    {
                                        hits[1].collider.gameObject.GetComponent<CharacterBase>().int_Health += hits[0].collider.gameObject.GetComponent<CharacterBase>().int_Health;
                                    }
                                    //-----------
                                    else if (hits[1].collider.gameObject == null)
                                    {
                                        return;
                                    }
                                    //-----------
                                }
                                //-----------
                            }
                            //-----------
                            else
                            {

                                rob.lr_laser.SetPosition(0, rob.transform.position);
                                rob.lr_laser.SetPosition(1, hits[1].point);
                                //-----------
                                if (hits[1].collider.gameObject.GetComponent<Tile>())
                                {
                                    hits[1].collider.gameObject.GetComponent<Tile>().int_health -= rob.int_damage;
                                    rob.int_heat_current += 1;
                                }
                                //-----------
                                else if (hits[1].collider.gameObject.GetComponent<CharacterBase>())
                                {
                                    hits[1].collider.gameObject.GetComponent<CharacterBase>().int_Health -= rob.int_damage;
                                    rob.int_heat_current += 1;
                                }
                                //-----------
                            }
                            //-----------
                        }
                        //-----------
                        if (rob.int_effect == 1)
                        {
                            hits[0].collider.gameObject.GetComponent<CharacterBase>().int_Health -= rob.int_damage;

                        }
                        //-----------
                    }
                    //-----------
                }
                //-----------
            }
            //-----------
            //rob.Clear_Selection();
        }
        //-----------
    }
    //---------------------------------------------------
    private void OnMouseExit()
    {
        //-----------
        if (tl_Current_Tile.bl_Attack_Selection == true)
        {
            rnd_Rendereer.material.color = Color.red;
        }
        //-----------
    }
    #endregion
    //---------------------------------------------------
    #region EndTurn
    public void EndTurn()
    {
        Clear_Selection();
        pr_Target = null;
        bl_Is_Active = false;
        CSGameManager.gameManager.EndAITurn();
    } 
    #endregion
    //---------------------------------------------------
    #region Death
    public void AICharacterDestruction()
    {
        CSGameManager.gameManager.ls_AI_Characters_In_Level.Remove(this);
        CSGameManager.gameManager.CheckLossOrWin();
        tl_Current_Tile.bl_Occupied_By_AI = false;
        Destroy(this.gameObject);
    }
    #endregion
    //---------------------------------------------------
    #region Visual Cyllinder for attacks. UNUSED
    /*
    private IEnumerator FireRay(float waitTime, PlayerRobot Rob)
    {
        //  Gizmos.DrawLine(Rob.transform.position, this.transform.position);
        GameObject _GO_Cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        _GO_Cylinder.transform.position = Rob.transform.position;
        _GO_Cylinder.gameObject.transform.localScale = new Vector3(0.5f, Rob.int_Attack_Range, 0.5f);
        _GO_Cylinder.transform.LookAt(this.transform.position, Rob.transform.position);
        _GO_Cylinder.transform.Rotate(0, 90, 90);
        _GO_Cylinder.transform.position += new Vector3(Rob.int_Attack_Range, 0, 0);
        yield return new WaitForSeconds(waitTime);
        Destroy(_GO_Cylinder.gameObject);
    }
    */
    #endregion
    //---------------------------------------------------
    #region Set/Get State
    public void SetState(AIStates _state)
    {
        ais_CurrentState = _state;
    }
    //---------------------------------------------------
    public AIStates GetState()
    {
        return ais_CurrentState;
    }
    #endregion
    //---------------------------------------------------
    #region SwitchState
    public void DecideTurnAction()
    {
        switch (ais_CurrentState)
        {
            case AIStates.waiting:
                Waiting();
                break;
            //-----------
            case AIStates.chasing:
                FindNearestPlayerRobot();
              //  TakeChaseTurn();
                break;
            //-----------
            case AIStates.retreating:
                FindNearestPlayerRobot();
              //  RetreatBehaviour();
                break;
            //-----------
            case AIStates.patrolling:
                //move to a nearby square? or randome or something
                break;
            //-----------
            case AIStates.inactive: //use this to keep AI inactive until some kind of event trigger
                EndTurn();
                break;
            //-----------
            default:
                print("No state");
                break;
                //-----------
        }
    }
    #endregion

    public void VisibleEnemy ()
    {
        if (tl_Current_Tile.bl_in_view_zone)
        {
            if (bl_is_invisible && tl_Current_Tile.bl_radar != true)
            {
                GetComponent<Renderer>().enabled = false;
                transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                GetComponent<Renderer>().enabled = true;
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }
            if (tl_Current_Tile.bl_tag)
            {
                bl_is_tagged = true;
            }
        }
        else
        {
            if (bl_is_tagged)
            {
                GetComponent<Renderer>().enabled = true;
                transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                GetComponent<Renderer>().enabled = false;
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}//=======================================================================================
