using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCommand : ICommand
{
    private UnitController _unitController;
    private Vector3 _destination;
    private BuildingBlueprintDataSO _data;
    private Building _building;

    public BuildCommand(UnitController unit, Vector3 destination, Building building, BuildingBlueprintDataSO data)
    {
        _unitController = unit;
        _destination = destination;
        _data = data;
        _building = building;
    }

    public void Execute()
    {
        _unitController.SetBuilding(_building);
        _unitController.SetBuildData(_data);
        _unitController.SetMoveDestination(_destination);
        _unitController.RequestStateChange("MoveToBuildstate", _destination);
    }
}
