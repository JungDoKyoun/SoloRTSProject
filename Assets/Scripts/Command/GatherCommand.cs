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
        _unitController.SetResources(_resources);
        _unitController.SetMoveDestination(_destination);
        _unitController.RequestStateChange("MoveToGatherState", _destination);
    }
}
