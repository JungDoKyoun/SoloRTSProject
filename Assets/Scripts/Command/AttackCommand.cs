using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : ICommand
{
    private UnitController _unitController;
    private UnitController _target;

    public AttackCommand(UnitController unitController, UnitController target)
    {
        _unitController = unitController;
        _target = target;
    }

    public void Execute()
    {
        _unitController.SetTarget(_target, true);
        _unitController.UnitStateManager.SetState(new ChaseState(), _unitController);
    }
}
