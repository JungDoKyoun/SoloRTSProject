using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMapping
{
    private Dictionary<int, UnitDataSO> _unitIDMap = new Dictionary<int, UnitDataSO>();
    private Dictionary<int, BuildingBlueprintDataSO> _buildingIDMap = new Dictionary<int, BuildingBlueprintDataSO>();

    public void Init(RaceType race)
    {
        switch(race)
        {
            case RaceType.Human:
                AddUnit("Human/UnitData/Peasant");
                AddUnit("Human/UnitData/Shield");
                AddUnit("Human/UnitData/Halberdier");
                AddUnit("Human/UnitData/HeavySwordman");
                AddUnit("Human/UnitData/Archer");
                AddUnit("Human/UnitData/Crossbowman");
                AddUnit("Human/UnitData/HighPriest");
                AddUnit("Human/UnitData/Knight");

                AddBuilding("Human/BuildingGhostData/TownHallPreview");
                AddBuilding("Human/BuildingGhostData/HousePreview");
                AddBuilding("Human/BuildingGhostData/BarracksPreview");
                AddBuilding("Human/BuildingGhostData/ArcheryPreview");
                AddBuilding("Human/BuildingGhostData/TemplePreview");
                AddBuilding("Human/BuildingGhostData/StablesPreview");
                AddBuilding("Human/BuildingGhostData/BlacksmithPreview");
                AddBuilding("Human/BuildingGhostData/LibraryPreview");
                AddBuilding("Human/BuildingGhostData/CastlePreview");
                break;
        }
    }

    private void AddUnit(string path)
    {
        var unitData = Resources.Load<UnitDataSO>(path);

        if(unitData != null)
        {
            if(!_unitIDMap.ContainsKey(unitData.ID))
            {
                _unitIDMap.Add(unitData.ID, unitData);
            }
        }
    }

    private void AddBuilding(string path)
    {
        var buildingData = Resources.Load<BuildingBlueprintDataSO>(path);

        if(buildingData != null)
        {
            if(!_buildingIDMap.ContainsKey(buildingData.ID))
            {
                _buildingIDMap.Add(buildingData.ID, buildingData);
            }
        }
    }

    public UnitDataSO GetUnitByID(int id)
    {
        _unitIDMap.TryGetValue(id, out var unit);
        return unit;
    }

    public BuildingBlueprintDataSO GetBuildingByID(int id)
    {
        _buildingIDMap.TryGetValue(id, out var building);
        return building;
    }
}
