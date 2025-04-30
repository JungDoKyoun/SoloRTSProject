using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMapping
{
    private Dictionary<UnitRole, List<UnitDataSO>> _unitMap = new Dictionary<UnitRole, List<UnitDataSO>>();
    private Dictionary<BuildingRole, BuildingBlueprintDataSO> _buildingMap = new Dictionary<BuildingRole, BuildingBlueprintDataSO>();

    public Dictionary<UnitRole, List<UnitDataSO>> UnitMap { get { return _unitMap; } }
    public Dictionary<BuildingRole, BuildingBlueprintDataSO> BuildingMap { get { return _buildingMap; } }

    public void Init(RaceType race)
    {
        switch(race)
        {
            case RaceType.Human:
                AddUnit(UnitRole.Worker, "Human/UnitData/Peasant");
                AddUnit(UnitRole.Melee, "Human/UnitData/Shield");
                AddUnit(UnitRole.Melee, "Human/UnitData/Halberdier");
                AddUnit(UnitRole.Melee, "Human/UnitData/HeavySwordman");
                AddUnit(UnitRole.Ranged, "Human/UnitData/Archer");
                AddUnit(UnitRole.Ranged, "Human/UnitData/Crossbowman");
                AddUnit(UnitRole.Healer, "Human/UnitData/HighPriest");
                AddUnit(UnitRole.Rider, "Human/UnitData/Knight");

                AddBuilding(BuildingRole.Base, "Human/BuildingGhostData/TownHallPreview");
                AddBuilding(BuildingRole.SupplyDepot, "Human/BuildingGhostData/HousePreview");
                AddBuilding(BuildingRole.Barracks, "Human/BuildingGhostData/BarracksPreview");
                AddBuilding(BuildingRole.Archery, "Human/BuildingGhostData/ArcheryPreview");
                AddBuilding(BuildingRole.Temple, "Human/BuildingGhostData/TemplePreview");
                AddBuilding(BuildingRole.Stables, "Human/BuildingGhostData/StablesPreview");
                AddBuilding(BuildingRole.None, "Human/BuildingGhostData/CastlePreview");
                AddBuilding(BuildingRole.Tech1, "Human/BuildingGhostData/BlacksmithPreview");
                AddBuilding(BuildingRole.Tech2, "Human/BuildingGhostData/LibraryPreview");
                break;
        }
    }

    private void AddUnit(UnitRole unitRole, string path)
    {
        var unitData = Resources.Load<UnitDataSO>(path);

        if(unitData != null)
        {
            if(!_unitMap.ContainsKey(unitRole))
            {
                _unitMap[unitRole] = new List<UnitDataSO>();
            }

            _unitMap[unitRole].Add(unitData);
        }
    }

    private void AddBuilding(BuildingRole buildingRole, string path)
    {
        var buildingData = Resources.Load<BuildingBlueprintDataSO>(path);

        if(buildingData != null)
        {
            if(!_buildingMap.ContainsKey(buildingRole))
            {
                _buildingMap[buildingRole] = buildingData;
            }
        }
    }

    public BuildingBlueprintDataSO GetBuildingData(BuildingRole buildingRole)
    {
        _buildingMap.TryGetValue(buildingRole, out var buildingData);
        return buildingData;
    }
}
