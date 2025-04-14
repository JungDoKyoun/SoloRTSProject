using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitState
{
    public void Enter();
    public void Exit();
    public void Update();
    public void FixedUpdate();
}

public class IdleState : IUnitState
{
    public void Enter()
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

    public MoveState(UnitController unit, Vector3 destination)
    {
        _unitController = unit;
        _destination = destination;
    }

    public void Enter()
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
