using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : ICommand
{
    private UnitController _unitController;
    private Vector3 _destination;

    public MoveCommand(UnitController unit, Vector3 destination, UnitStateManager state)
    {
        _unitController = unit;
        _destination = destination;
    }

    public void Execute()
    {
        _unitController.UnitStateManager.SetState(new MoveState(), _unitController, _destination);
    }
}
