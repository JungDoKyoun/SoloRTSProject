using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : ICommand
{
    private UnitController _unitController;
    private Vector3 _destination;

    public MoveCommand(UnitController unit, Vector3 destination)
    {
        _unitController = unit;
        _destination = destination;
    }

    public void Execute()
    {
        if (_unitController.IsAIUnit)
        {
            if (_unitController.CurrentUnitTask != UnitTask.Moveing || Vector3.Distance(_unitController.GetMoveDestination(), _destination) > 0.1f)
            {
                _unitController.SetTask(UnitTask.Moveing);
                _unitController.SetMoveDestination(_destination);
                _unitController.RequestStateChange("MoveState", _destination);
            }
        }
        else
        {
            _unitController.SetTask(UnitTask.Moveing);
            _unitController.SetMoveDestination(_destination);
            _unitController.RequestStateChange("MoveState", _destination);
        }
    }
}
