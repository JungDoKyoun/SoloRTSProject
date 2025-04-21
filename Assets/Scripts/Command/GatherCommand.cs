using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherCommand : ICommand
{
    private UnitController _unitController;
    private Resource _resources;

    public GatherCommand(UnitController unitController, Resource resources)
    {
        _unitController = unitController;
        _resources = resources;
    }

    public void Execute()
    {
        if(_unitController.UnitType != UnitType.Worker)
        {
            return;
        }
        _unitController.SetResources(_resources);
        _unitController.UnitStateManager.SetState(new MoveToGatherState(), _unitController, _resources.transform.position);
    }
}
