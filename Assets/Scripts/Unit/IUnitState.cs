using Unity.VisualScripting;
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

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;

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
        if (_unitController.IsDestroyed)
        {
            _unitController.RequestStateChange("DieState");
            return;
        }

        UnitController target = _unitController.FindTarget();

        if (target != null)
        {
            _unitController.SetTarget(target, true);
            _unitController.RequestStateChange("ChaseState");
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
        if (_unitController.IsDestroyed)
        {
            _unitController.RequestStateChange("DieState");
            return;
        }

        _unitController.RotateToMoveDirection();

        if (_unitController.IsArrive())
        {
            _unitController.RequestStateChange("IdleState");
            return;
        }
    }
}

public class ChaseState : IUnitState
{
    private UnitController _unitController;
    private IAttackable _target;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _target = unitController.GetTarget();

        Debug.Log("추격");

        if (_target != null)
        {
            unitController.MoveTo(_target.Position);
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
        if (_unitController.IsDestroyed)
        {
            _unitController.RequestStateChange("DieState");
            return;
        }

        _unitController.RotateToTarget(_target);

        if (_target.IsDestroyed || _target == null)
        {
            _unitController.ClearTarget();
            _unitController.RequestStateChange("IdleState");
            return;
        }

        float distance = Vector3.Distance(_unitController.transform.position, _target.Position);

        if (distance <= _unitController.UnitData.AttackRange)
        {
            _unitController.RequestStateChange("AttackState");
            return;
        }
        else
        {
            _unitController.MoveTo(_target.Position);
            return;
        }
    }
}

public class AttackState : IUnitState
{
    private UnitController _unitController;
    private IAttackable _target;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _target = unitController.GetTarget();
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
        if (_unitController.IsDestroyed)
        {
            Debug.Log("1");
            _unitController.RequestStateChange("DieState");
            return;
        }

        _unitController.RotateToTarget(_target);

        if (!_unitController.IsAttack)
        {
            if (_target == null || _target.IsDestroyed)
            {
                Debug.Log("2");
                _unitController.ClearTarget();
                _unitController.RequestStateChange("IdleState");
                return;
            }

            float distance = Vector3.Distance(_unitController.transform.position, _target.Position);

            if (distance > _unitController.UnitData.AttackRange)
            {
                _unitController.RequestStateChange("ChaseState");
                return;
            }

            if (_unitController.CanAttack())
            {
                _unitController.Attack();
            }
        }
    }
}

public class MoveAttackState : IUnitState
{
    private UnitController _unitController;
    private Vector3 _destination;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        Debug.Log("어택 무브");
        _unitController = unitController;
        _destination = destination;

        unitController.MoveTo(_destination);
    }

    public void Exit()
    {
        _unitController.MoveStop();
    }

    public void FixedUpdate()
    {
        _unitController.RotateToMoveDirection();
    }

    public void Update()
    {
        if (_unitController.IsDestroyed)
        {
            _unitController.RequestStateChange("DieState");
            return;
        }

        if (_unitController.IsArrive())
        {
            _unitController.RequestStateChange("IdleState");
            return;
        }

        UnitController target = _unitController.FindTarget();

        if (target != null)
        {
            float distance = Vector3.Distance(_unitController.transform.position, target.transform.position);

            if (distance <= _unitController.UnitData.AttackRange)
            {
                _unitController.SetTarget(target, false);
                _unitController.RequestStateChange("AttackState");
                return;
            }
        }
    }
}

public class MoveToGatherState : IUnitState
{
    private UnitController _unitController;
    private Resource _resources;
    private Vector3 _destination;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
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
        _unitController.RotateToMoveDirection();
    }

    public void Update()
    {
        if (_unitController.IsDestroyed)
        {
            _unitController.RequestStateChange("DieState");
            return;
        }

        if (_resources == null || _resources.RemainAmount <= 0)
        {
            var res = Utils.FindNearestAvailableResource(_unitController.transform.position, _unitController.UnitData.GatherSearchRadius, _unitController.CurrentResourceType);

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
                _unitController.RequestStateChange("IdleState");
                return;
            }
        }

        if (Vector3.Distance(_unitController.transform.position, _destination) <= _unitController.UnitData.AttackRange)
        {
            _unitController.RequestStateChange("GatherState");
            return;
        }
    }
}

public class GatherState : IUnitState
{
    private UnitController _unitController;
    private Resource _resources;
    private Vector3 _destination;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
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
        if (_unitController.IsDestroyed)
        {
            _unitController.RequestStateChange("DieState");
            return;
        }

        if (_resources == null || _resources.RemainAmount <= 0)
        {
            var res = Utils.FindNearestAvailableResource(_unitController.transform.position, _unitController.UnitData.GatherSearchRadius, _unitController.CurrentResourceType);

            if (res != null)
            {
                _resources = res;
                _unitController.SetResources(_resources);
                _destination = _resources.transform.position;
                _unitController.RequestStateChange("MoveToGatherState", _destination);
                return;
            }
            else if (res == null && _unitController.IsReturnToBase(out _destination))
            {
                _unitController.RequestStateChange("ReturnToBaseState", _destination);
                return;
            }
            else
            {
                _unitController.RequestStateChange("IdleState");
                return;
            }
        }

        if(_unitController.IsFull && _unitController.IsReturnToBase(out _destination))
        {
            _unitController.RequestStateChange("ReturnToBaseState", _destination);
            return;
        }

        if (!_unitController.IsGather)
        {
            _unitController.RequestStateChange("IdleState");
            return;
        }
    }
}

public class ReturnToBaseState : IUnitState
{
    private UnitController _unitController;
    private Resource _resources;
    private Vector3 _destination;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
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
        _unitController.RotateToMoveDirection();
    }

    public void Update()
    {
        if (_unitController.IsDestroyed)
        {
            _unitController.RequestStateChange("DieState");
            return;
        }

        var depot = Utils.FindNearestOwnedDepot(_unitController);

        if(depot != null)
        {
            if(_unitController.IsCarryOver())
            {
                if(_resources != null && _resources.RemainAmount > 0)
                {
                    _destination = _resources.transform.position;
                    _unitController.RequestStateChange("MoveToGatherState", _destination);
                    return;
                }
                else
                {
                    var res = Utils.FindNearestAvailableResource(_unitController.transform.position, _unitController.UnitData.GatherSearchRadius, _unitController.CurrentResourceType);

                    if(res != null)
                    {
                        _resources = res;
                        _unitController.SetResources(_resources);
                        _destination = _resources.transform.position;
                        _unitController.RequestStateChange("MoveToGatherState", _destination);
                        return;
                    }
                    else
                    {
                        _unitController.RequestStateChange("IdleState");
                        return;
                    }
                }
            }
        }
        else
        {
            _unitController.RequestStateChange("IdleState");
        }
    }
}

public class MoveToBuildstate : IUnitState
{
    private UnitController _unitController;
    private Vector3 _destination;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _destination = destination;

        unitController.MoveTo(destination);
    }

    public void Exit()
    {
        _unitController.MoveStop();
    }

    public void FixedUpdate()
    {
        _unitController.RotateToMoveDirection();
    }

    public void Update()
    {
        if (_unitController.IsDestroyed)
        {
            _unitController.RequestStateChange("DieState");
            return;
        }

        float distance = Vector3.Distance(_unitController.transform.position, _destination);

        if(distance <= _unitController.UnitData.BuildDistance)
        {
            _unitController.RequestStateChange("BuildState", _destination);
            return;
        }
    }
}

public class BuildState : IUnitState
{
    private UnitController _unitController;
    private Vector3 _destination;
    private Building _building;
    private BuildingBlueprintDataSO _data;
    private double _startTime;
    private double _completeTime;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _destination = destination;
        _building = unitController.GetBuilding();
        _data = _building.GetBuildData();

        unitController.PlayAnime("Build", true);
        _startTime = Utils.GetTime();
        _completeTime = _startTime + _data.BuildTime;
    }

    public void Exit()
    {
        _unitController.PlayAnime("Build", false);
    }

    public void FixedUpdate()
    {
        
    }

    public void Update()
    {
        if(_unitController.IsDestroyed)
        {
            _unitController.RequestStateChange("DieState");
            return;
        }

        if(_building == null)
        {
            _unitController.RequestStateChange("IdleState");
            return;
        }

        double currentTime = Utils.GetTime();

        if(currentTime >= _completeTime)
        {
            _building.IsComplete = true;
            _building.CompleteConstruction(_destination);
            _unitController.RequestStateChange("IdleState");
            return;
        }
    }
}

public class DieState : IUnitState
{
    private UnitController _unitController;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;

        Debug.Log("죽음");
        unitController.PlayAnime("Die", true);
        unitController.StartDie();
        unitController.ClearTarget();
        unitController.StopAttack();
        unitController.StopGather();
        unitController.MoveStop();
        unitController.StopAll();
    }

    public void Exit()
    {
        _unitController.PlayAnime("Die", false);
    }

    public void FixedUpdate()
    {
        
    }

    public void Update()
    {
        if(!_unitController.IsDestroyed)
        {
            _unitController.RequestStateChange("IdleState");
            return;
        }
    }
}
