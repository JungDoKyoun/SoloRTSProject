using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MoveAttackCommand : ICommand
{
    private UnitController _unitController;
    private Vector3 _destination;

    public MoveAttackCommand(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _destination = destination;
    }

    public void Execute()
    {
        if (_unitController.IsAIUnit)
        {
            if (_unitController.CurrentUnitTask != UnitTask.Chasing || Vector3.Distance(_unitController.GetMoveDestination(), _destination) > 0.1f)
            {
                _unitController.ResetTask();
                _unitController.SetTask(UnitTask.Chasing);
                _unitController.SetMoveDestination(_destination);
                _unitController.RequestStateChange("MoveAttackState", _destination);
            }
        }
        else
        {
            _unitController.SetTask(UnitTask.Chasing);
            _unitController.SetMoveDestination(_destination);
            _unitController.RequestStateChange("MoveAttackState", _destination);
        }
    }
}
