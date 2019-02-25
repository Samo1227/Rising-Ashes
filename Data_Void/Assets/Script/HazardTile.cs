using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HazardTileType
{
    heat,
    damagingHeat,
    cold,
    damagingCold,
    collapsingTile,
    difficultTerrain,
}
//=======================================================================================
public class HazardTile : Tile
{
    #region Variables
    public HazardTileType hazardType;
    public int int_Heat = 0;
    public int int_Damage = 0;
    public int int_Cold = 0;
    public int int_MoveDifficulty = 2;
    public int int_WeightLimit = 0;
    #endregion
    //---------------------------------------------------
    #region Start
    void Start()
    {
        if (hazardType == HazardTileType.difficultTerrain)
        {
            int_Move_Cost = int_MoveDifficulty;
        }
        rend_Colour = gameObject.transform.GetChild(int_Child).GetComponent<Renderer>();//allows changing of tiles colour and takes into account tiles being changed (destroyed/created)
        fl_ExplodeRaduis = 1;

        if (bl_opaque == false)
        {
            GameObject temp = gameObject.transform.GetChild(int_Child).gameObject;
            //go_fog = temp.transform.GetChild(0).GetComponent<Renderer>();
        }

    }
    #endregion
    void Update()
    { //can make the colours a public selection so can set it ip in inspecto

        //go_health_bar.transform.localPosition = new Vector3(((float)int_health - (float)int_health_max) * (0.5f / int_health_max), 0, 0);
        //go_health_bar.transform.localScale = new Vector3((1f / int_health_max) * int_health, 0.2f, 1);

        if (int_health < int_health_max && int_health > 0)
        {
            go_health_bar.SetActive(true);
            go_health_bar_back.SetActive(true);

        }
        else if (int_health <= 0)
        {
            go_health_bar.SetActive(false);
            go_health_bar_back.SetActive(false);
        }
        else
        {
            go_health_bar.SetActive(false);
            go_health_bar_back.SetActive(false);
        }

        if (bl_Current_Tile)
        {
            rend_Colour.material.color = Color.green;
        }
        else if (bl_Walking_Selection)//if this tile is in walking range
        {
            rend_Colour.material.color = Color.blue;
        }
        else if (bl_Attack_Selection)//if this tile is in attack range
        {
            rend_Colour.material.color = Color.red;
        }
        else if (bl_spawnable_zone == true && CSGameManager.gameManager.bl_storing_robot == true && bl_Is_Walkable && bl_Occupied_By_PC == false && bl_Occupied_By_AI == false)
        {
            rend_Colour.material.color = Color.green;
        }
        else//normal  colour
        {
            rend_Colour.material.color = Color.white;
        }
        if (bl_Is_Walkable == false && int_health <= 0)
        {
            RemoveTile();
        }

    }
    //---------------------------------------------------
    public void SetHazardType(HazardTileType _hazardType)
    {
        hazardType = _hazardType;
    }
    //---------------------------------------------------
    #region HazardSwitch
    public void ApplyHazard(CharacterBase _character)
    {
        switch (hazardType)
        {
            case (HazardTileType.heat):
                ApplyHeat(_character);
                return;
            case (HazardTileType.damagingHeat):
                ApplyHeatAndDamage(_character);
                return;
            case (HazardTileType.cold):
                ReduceHeat(_character);
                return;
            case (HazardTileType.damagingCold):
                ReduceHeatDealDamage(_character);
                return;
            case (HazardTileType.collapsingTile):
                Collapse(_character);
                return;
            default:
                print("No Hazard");
                return;
        }
    }
    #endregion
    //---------------------------------------------------
    #region Heat
    public void ApplyHeat(CharacterBase _character)
    {
        PlayerRobot _TempPR = _character.GetComponent<PlayerRobot>();
        if (_TempPR != null)
        {
            _TempPR.int_heat_current += int_Heat;
        }
    }
    #endregion
    //---------------------------------------------------
    #region Heat+Damage
    public void ApplyHeatAndDamage(CharacterBase _character)
    {
        PlayerRobot _TempPR = _character.GetComponent<PlayerRobot>();
        if (_TempPR != null)
        {
            _TempPR.int_heat_current += int_Heat;
        }
        _character.int_Health -= int_Damage;
    }
    #endregion
    //---------------------------------------------------
    #region Cold
    public void ReduceHeat(CharacterBase _character)
    {
        PlayerRobot _TempPR = _character.GetComponent<PlayerRobot>();
        if (_TempPR != null)
        {
            _TempPR.int_heat_current -= int_Cold;
        }
    }
    #endregion
    //---------------------------------------------------
    #region ColdDamage
    public void ReduceHeatDealDamage(CharacterBase _character)
    {
        PlayerRobot _TempPR = _character.GetComponent<PlayerRobot>();
        if (_TempPR != null)
        {
            _TempPR.int_heat_current -= int_Cold;
        }
        _character.int_Health -= int_Damage;
    }
    #endregion
    //---------------------------------------------------
    #region Collapse
    public void Collapse(CharacterBase _character)
    {
        if (_character.int_Weight_Current >= int_WeightLimit)
        {
            print("Collapse");
        }
    } 
    #endregion
    //---------------------------------------------------
}//=======================================================================================
