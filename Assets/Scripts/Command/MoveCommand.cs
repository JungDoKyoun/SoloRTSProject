using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : ICommand
{
    private UnitController _unitController;
    private Vector3 _destination;
    private UnitStateManager _state;

    public MoveCommand(UnitController unit, Vector3 destination, UnitStateManager state)
    {
        _unitController = unit;
        _destination = destination;
        _state = state;
    }

    public void Execute()
    {
        _state.SetState(new MoveState(_unitController, _destination));
    }
}
