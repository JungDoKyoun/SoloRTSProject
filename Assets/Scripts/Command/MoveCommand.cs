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
        _unitController.SetMoveDestination(_destination);
        _unitController.RequestStateChange("MoveState", _destination);
    }
}
