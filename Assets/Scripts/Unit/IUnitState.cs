using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
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
    private UnitController _unitController;
    private UnitStateManager _stateManager;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _stateManager = _unitController.UnitStateManager;
        _unitController.ResetBool();
        Debug.Log("대기");
    }

    public void Exit()
    {
        
    }

    public void FixedUpdate()
    {
        
    }

    public void Update()
    {
        UnitController target = _unitController.FindTarget();

        if(target != null)
        {
            _unitController.SetTarget(target, true);
            _stateManager.SetState(new ChaseState(), _unitController);
            return;
        }
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
        Debug.Log("이동");
        if(_unitController != null && _destination != null)
        {
            _unitController.ResetBool();
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
        _unitController.RotateToMoveDirection();

        if (_unitController.IsArrive())
        {
            _unitController.UnitStateManager.SetState(new IdleState(), _unitController);
            return;
        }
    }
}

public class ChaseState : IUnitState
{
    private UnitController _unitController;
    private UnitController _target;
    private UnitStateManager _stateManager;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _target = _unitController.GetTarget();
        _stateManager = _unitController.UnitStateManager;
        _unitController.ResetBool();
        Debug.Log("추격");

        if (_target != null)
        {
            _unitController.MoveTo(_target.transform.position);
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
        _unitController.RotateToTarget(_target);

        if(_target.IsDie || _target == null)
        {
            _unitController.ClearTarget();
            _stateManager.SetState(new IdleState(), _unitController);
            return;
        }

        float distance = Vector3.Distance(_unitController.transform.position, _target.transform.position);

        if(distance <= _unitController.UnitData.AttackRange)
        {
            _stateManager.SetState(new AttackState(), _unitController);
            return;
        }
        else
        {
            _unitController.MoveTo(_target.transform.position);
            return;
        }
    }
}

public class AttackState : IUnitState
{
    private UnitController _unitController;
    private UnitController _target;
    private UnitStateManager _stateManager;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _target = _unitController.GetTarget();
        _stateManager = _unitController.UnitStateManager;
        _unitController.ResetBool();
    }

    public void Exit()
    {
        
    }

    public void FixedUpdate()
    {
        
    }

    public void Update()
    {
        if(!_unitController.IsAttack)
        {
            if (_target == null || _target.IsDie)
            {
                _unitController.ClearTarget();
                _stateManager.SetState(new IdleState(), _unitController);
                return;
            }

            float distance = Vector3.Distance(_unitController.transform.position, _target.transform.position);

            if (distance > _unitController.UnitData.AttackRange)
            {
                if (_unitController.IsManualAttack)
                {
                    _stateManager.SetState(new ChaseState(), _unitController);
                }
                else
                {
                    _unitController.ClearTarget();
                    _stateManager.SetState(new MoveAttackState(), _unitController, _unitController.GetMoveDestination());
                }
                return;
            }

            _unitController.RotateToTarget(_target);

            if (_unitController.CanAttack())
            {
                Debug.Log("공격");
                _unitController.Attack();
            }
        }
    }
}

public class MoveAttackState : IUnitState
{
    private UnitController _unitController;
    private UnitStateManager _stateManager;
    private Vector3 _destination;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        Debug.Log("어택 무브");
        _unitController = unitController;
        _destination = destination;
        _stateManager = _unitController.UnitStateManager;

        if(_unitController != null && _destination != null)
        {
            _unitController.ResetBool();
            _unitController.SetMoveDestination(_destination);
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
        _unitController.RotateToMoveDirection();

        if (_unitController.IsArrive())
        {
            _stateManager.SetState(new IdleState(), _unitController);
            return;
        }

        UnitController target = _unitController.FindTarget();

        if(target != null)
        {
            float distance = Vector3.Distance(_unitController.transform.position, target.transform.position);

            if(distance <= _unitController.UnitData.AttackRange)
            {
                _unitController.SetTarget(target, false);
                _stateManager.SetState(new AttackState(), _unitController);
                return;
            }
        }
    }
}
