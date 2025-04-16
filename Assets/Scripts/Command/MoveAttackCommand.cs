using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        _unitController.UnitStateManager.SetState(new MoveAttackState(), _unitController, _destination);
    }
}
