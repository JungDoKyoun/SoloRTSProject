using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherCommand : ICommand
{
    private UnitController _unitController;
    private Resource _resources;
    private Vector3 _destination;

    public GatherCommand(UnitController unitController, Resource resources, Vector3 destination)
    {
        _unitController = unitController;
        _resources = resources;
        _destination = destination;
    }

    public void Execute()
    {
        if(_unitController.UnitType != UnitType.Worker)
        {
            return;
        }

        UnitTask task = UnitTask.None;

        if (_resources.Type == ResourcesType.Gold)
            task = UnitTask.GatheringGold;

        else if (_resources.Type == ResourcesType.Wood)
            task = UnitTask.GatheringWood;

        if (_unitController.IsAIUnit)
        {
            if (_unitController.CurrentUnitTask != task || Vector3.Distance(_unitController.GetMoveDestination(), _destination) > 0.1f)
            {
                _unitController.SetTask(task);
                _unitController.SetResources(_resources);
                _unitController.SetMoveDestination(_destination);
                _unitController.RequestStateChange("MoveToGatherState", _destination);
            }
        }
        else
        {
            _unitController.SetTask(task);
            _unitController.SetResources(_resources);
            _unitController.SetMoveDestination(_destination);
            _unitController.RequestStateChange("MoveToGatherState", _destination);
        }
    }
}
