using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : ICommand
{
    private UnitController _unitController;
    private IAttackable _target;

    public AttackCommand(UnitController unitController, IAttackable target)
    {
        _unitController = unitController;
        _target = target;
    }

    public void Execute()
    {
        _unitController.SetTarget(_target, true);
        _unitController.RequestStateChange("ChaseState");
    }
}
