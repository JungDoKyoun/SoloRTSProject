using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitState
{
    public void Enter(UnitController unitController, Vector3 destination);
    public void Exit();
    public void Update();
    public void FixedUpdate();
}

public class IdleState : IUnitState
{
    public void Enter(UnitController unitController, Vector3 destination)
    {
        
    }

    public void Exit()
    {
        
    }

    public void FixedUpdate()
    {
        
    }

    public void Update()
    {
        
    }
}

public class MoveState : IUnitState
{
    private UnitController _unitController;
    private Vector3 _destination;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _destination = destination;
        if(_unitController != null && _destination != null)
        {
            _unitController.MoveTo(_destination);
        }
    }

    public void Exit()
    {
        _unitController.MoveStop();
    }

    public void FixedUpdate()
    {
        
    }

    public void Update()
    {
        if(_unitController.IsArrive())
        {
            _unitController.UnitStateManager.SetState(new IdleState(), _unitController);
        }
    }
}
