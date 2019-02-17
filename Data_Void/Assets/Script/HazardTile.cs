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
    }
    #endregion
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
