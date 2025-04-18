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
        _stateManager = unitController.UnitStateManager;

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

        if (target != null)
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
    private UnitStateManager _stateManager;
    private Vector3 _destination;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _destination = destination;
        _stateManager = unitController.UnitStateManager;

        Debug.Log("이동");
        if (_destination != null)
        {
            unitController.MoveTo(_destination);
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
        _target = unitController.GetTarget();
        _stateManager = unitController.UnitStateManager;
        Debug.Log("추격");

        if (_target != null)
        {
            unitController.MoveTo(_target.transform.position);
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

        if (_target.IsDie || _target == null)
        {
            _unitController.ClearTarget();
            _stateManager.SetState(new IdleState(), _unitController);
            return;
        }

        float distance = Vector3.Distance(_unitController.transform.position, _target.transform.position);

        if (distance <= _unitController.UnitData.AttackRange)
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
        _target = unitController.GetTarget();
        _stateManager = unitController.UnitStateManager;
    }

    public void Exit()
    {
        _unitController.StopAttack();
    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {
        _unitController.RotateToTarget(_target);

        if (!_unitController.IsAttack)
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
        _stateManager = unitController.UnitStateManager;

        unitController.SetMoveDestination(_destination);
        unitController.MoveTo(_destination);
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

        if (target != null)
        {
            float distance = Vector3.Distance(_unitController.transform.position, target.transform.position);

            if (distance <= _unitController.UnitData.AttackRange)
            {
                _unitController.SetTarget(target, false);
                _stateManager.SetState(new AttackState(), _unitController);
                return;
            }
        }
    }
}

public class MoveToGatherState : IUnitState
{
    private UnitController _unitController;
    private UnitStateManager _stateManager;
    private Resources _resources;
    private Vector3 _destination;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _stateManager = unitController.UnitStateManager;
        _resources = unitController.GetResources();
        _destination = destination;

        unitController.MoveTo(destination);
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
        if(_resources == null || _resources.RemainAmount <= 0)
        {
            var res = Utils.FindNearestAvailableResource(_unitController.transform.position, _unitController.UnitData.GatherSearchRadius, _resources.Type);

            if(res != null)
            {
                _resources = res;
                _unitController.SetResources(_resources);
                _destination = _resources.transform.position;
                _unitController.MoveTo(_destination);
                return;
            }
            else if(res == null)
            {
                _stateManager.SetState(new IdleState(), _unitController);
                return;
            }
        }

        if (Vector3.Distance(_unitController.transform.position, _destination) <= _unitController.UnitData.AttackRange)
        {
            _stateManager.SetState(new GatherState(), _unitController);
            return;
        }
    }
}

public class GatherState : IUnitState
{
    private UnitController _unitController;
    private UnitStateManager _stateManager;
    private Resources _resources;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _stateManager = unitController.UnitStateManager;
        _resources = unitController.GetResources();

        unitController.StartGather();
    }

    public void Exit()
    {
        _unitController.StopGather();
    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {
        if(_resources == null || _resources.RemainAmount <= 0)
        {
            var res = Utils.FindNearestAvailableResource(_unitController.transform.position, _unitController.UnitData.GatherSearchRadius, _resources.Type);

            if (res != null)
            {
                Vector3 destination;

                _resources = res;
                _unitController.SetResources(_resources);
                destination = _resources.transform.position;
                _stateManager.SetState(new MoveToGatherState(), _unitController, destination);
                return;
            }
            else if (res == null)
            {
                _stateManager.SetState(new ReturnToBaseState(), _unitController);
                return;
            }
        }

        if(_unitController.IsFull)
        {
            _stateManager.SetState(new ReturnToBaseState(), _unitController);
            return;
        }

        if (!_unitController.IsGather)
        {
            _stateManager.SetState(new IdleState(), _unitController);
            return;
        }
    }
}

public class ReturnToBaseState : IUnitState
{
    UnitController _unitController;
    private UnitStateManager _stateManager;
    Vector3 _destination;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _stateManager = unitController.UnitStateManager;
        _destination = destination;
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
