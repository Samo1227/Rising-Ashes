using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIStates          //an enum of AI states                            
{
    waiting,                  //do nothing but maybe make some checks state
    patrolling,               //Wander around state
    chasing,                  //close in and attack
    retreating,               //make space and attack
}

public class AICharacter : CharacterBase {

    public PlayerRobot pr_Target; //character the AI will move towards and attack
    public Renderer rnd_Rendereer;
    public AIStates ais_CurrentState;

    public int int_ChaseRange = 6;
    public int int_TriggerRange = 2;
    // private IEnumerator coroutine;
    //---------------------------------------------------
    #region Start
    void Start()
    {
        int_Health = int_Health_max;
        SetAttackRange(1);
        SetDamage(1);
        CSGameManager.gameManager.ls_AI_Characters_In_Level.Add(this);
        rnd_Rendereer = gameObject.transform.GetComponent<Renderer>();
        tl_Current_Tile = CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();
        tl_Current_Tile.bl_Occupied_By_AI = true;
        SetState(AIStates.waiting);
       // coroutine = FireRay(2.0f);
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
        if (fl_Turn_Timer >= fl_Time_limit)
        {
            CSGameManager.gameManager.EndAITurn();
        }
        //-----------
        if (bl_Is_Active)
        {
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
            TakeChaseTurn();
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
        print("taking turn");
        //-----------
        if (bl_Turn_Just_Started)
        {
            pr_Target = null; //rework out target at the start of each turn
            bl_Turn_Just_Started = false;
        }
        //-----------
        if (!bl_Moving)//don't keep checking for a target when moving
        {
            tl_Current_Tile.bl_Occupied_By_AI = false;
            FindNearestPlayerRobot();
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
                print("Is this called?");
                FindAlternateRoute();
                //-----------
                if (tl_Target_Square != null)
                {
                    return;
                }
                //-----------
                Clear_Selection();//putting this everywhere now :S
                int_x = (int)transform.position.x;
                int_z = (int)transform.position.z;
                tl_Current_Tile = CSGameManager.gameManager.map[int_x, int_z].gameObject.GetComponent<Tile>();
                tl_Current_Tile.bl_Occupied_By_AI = true;
                Find_Attack_Tile_Range();
                FindPRsInRange();
                Clear_Selection();
                bl_Is_Active = false;
                CSGameManager.gameManager.EndAITurn();
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
                Clear_Selection();
                pr_Target = null;
                bl_Is_Active = false;
                CSGameManager.gameManager.EndAITurn();
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
    #region Mouse Interaction
    private void OnMouseOver()//this doesn't work for shooting enemies
    {

        //-----------
        if (tl_Current_Tile.bl_Attack_Selection == true)
        {
            PlayerRobot rob = CSGameManager.gameManager.pr_currentRobot;
            rnd_Rendereer.material.color = Color.yellow;

            RaycastHit hit;

            Vector3 dir = new Vector3(transform.position.x, 1, transform.position.z) - rob.transform.position;
            dir = dir.normalized;
            float fl_Ray_Range = Vector3.Distance(new Vector3(transform.position.x, 1, transform.position.z), rob.transform.position);

            Ray ray_cast = new Ray(rob.transform.position, dir * fl_Ray_Range);

            Debug.DrawRay(rob.transform.position, dir * fl_Ray_Range, Color.green, 0.1f);
            
            //-----------
            if (Input.GetMouseButtonDown(0))
            {
                //-----------
                if (Physics.Raycast(ray_cast, out hit, fl_Ray_Range))
                {
                    //-----------
                    if (hit.collider.gameObject.GetComponent<AICharacter>())
                    {
                        rob.bl_Has_Acted = true;

                        rnd_Rendereer.material.color = Color.red;
                        rob.Clear_Selection();
                        hit.collider.gameObject.GetComponent<AICharacter>().AttackTarget(rob, this);
                        StartCoroutine(FireRay(5f, rob));

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
    #region Visual Cyllinder for attacks, not really working atm
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
            case AIStates.chasing:
                TakeChaseTurn();
                break;
            case AIStates.retreating:
                //retreat and attack
                break;
            case AIStates.patrolling:
                //move to a nearby square? or randome or something
                break;
            default:
                print("No state");
                break;
        }
    }
    #endregion
}//=======================================================================================
