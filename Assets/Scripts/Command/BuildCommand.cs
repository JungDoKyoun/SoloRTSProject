using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCommand : ICommand
{
    private UnitController _unit;
    private Vector3 _destination;
    private BuildingBlueprintDataSO _data;
    private Building _building;

    public BuildCommand(UnitController unit, Vector3 destination, Building building, BuildingBlueprintDataSO data)
    {
        _unit = unit;
        _destination = destination;
        _data = data;
        _building = building;
    }

    public void Execute()
    {
        _unit.SetBuilding(_building);
        _unit.SetBuildData(_data);
        _unit.UnitStateManager.SetState(new MoveToBuildstate(), _unit, _destination);
    }
}
