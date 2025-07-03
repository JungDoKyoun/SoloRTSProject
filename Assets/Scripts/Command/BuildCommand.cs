using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
        if (_unitController.IsAIUnit)
        {
            if (_unitController.CurrentUnitTask != UnitTask.Building || Vector3.Distance(_unitController.GetMoveDestination(), _destination) > 0.1f)
            {
                _unitController.SetTask(UnitTask.Building);
                _unitController.SetBuilding(_building);
                _unitController.SetBuildData(_data);
                _unitController.SetMoveDestination(_destination);
                _unitController.RequestStateChange("MoveToBuildstate", _destination);
            }
        }
        else
        {
            _unitController.SetTask(UnitTask.Building);
            _unitController.SetBuilding(_building);
            _unitController.SetBuildData(_data);
            _unitController.SetMoveDestination(_destination);
            _unitController.RequestStateChange("MoveToBuildstate", _destination);
        }
    }
}
