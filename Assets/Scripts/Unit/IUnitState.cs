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
        //UnitController target = _unitController.FindTarget();

        //if (target != null)
        //{
        //    _unitController.SetTarget(target, true);
        //    _stateManager.SetState(new ChaseState(), _unitController);
        //    return;
        //}
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
    private Resource _resources;
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
        _unitController.RotateToMoveDirection();
    }

    public void Update()
    {
        if (_resources == null || _resources.RemainAmount <= 0)
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
    private Resource _resources;
    private Vector3 _destination;

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
            var res = Utils.FindNearestAvailableResource(_unitController.transform.position, _unitController.UnitData.GatherSearchRadius, _unitController.CurrentResourceType);

            if (res != null)
            {
                _resources = res;
                _unitController.SetResources(_resources);
                _destination = _resources.transform.position;
                _stateManager.SetState(new MoveToGatherState(), _unitController, _destination);
                return;
            }
            else if (res == null && _unitController.IsReturnToBase(out _destination))
            {
                _stateManager.SetState(new ReturnToBaseState(), _unitController, _destination);
                return;
            }
            else
            {
                _stateManager.SetState(new IdleState(), _unitController);
                return;
            }
        }

        if(_unitController.IsFull && _unitController.IsReturnToBase(out _destination))
        {
            _stateManager.SetState(new ReturnToBaseState(), _unitController, _destination);
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
    private UnitController _unitController;
    private UnitStateManager _stateManager;
    private Resource _resources;
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
        _unitController.RotateToMoveDirection();
    }

    public void Update()
    {
        var depot = Utils.FindNearestOwnedDepot(_unitController);

        if(depot != null)
        {
            if(_unitController.IsCarryOver())
            {
                if(_resources != null && _resources.RemainAmount > 0)
                {
                    _stateManager.SetState(new MoveToGatherState(), _unitController, _resources.transform.position);
                }
                else
                {
                    var res = Utils.FindNearestAvailableResource(_unitController.transform.position, _unitController.UnitData.GatherSearchRadius, _unitController.CurrentResourceType);

                    if(res != null)
                    {
                        _resources = res;
                        _unitController.SetResources(_resources);
                        _destination = _resources.transform.position;
                        _stateManager.SetState(new MoveToGatherState(), _unitController, _destination);
                    }
                    else
                    {
                        _stateManager.SetState(new IdleState(), _unitController, _destination);
                    }
                }
            }
        }
        else
        {
            _stateManager.SetState(new IdleState(), _unitController);
        }
    }
}

public class MoveToBuildstate : IUnitState
{
    private UnitController _unitController;
    private UnitStateManager _stateManager;
    private Vector3 _destination;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _stateManager = unitController.UnitStateManager;
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
        float distance = Vector3.Distance(_unitController.transform.position, _destination);

        if(distance <= _unitController.UnitData.BuildDistance)
        {
            _stateManager.SetState(new BuildState(), _unitController, _destination);
            return;
        }
    }
}

public class BuildState : IUnitState
{
    private UnitController _unitController;
    private UnitStateManager _stateManager;
    private Vector3 _destination;
    private Building _building;
    private BuildingBlueprintDataSO _data;
    private Player _player;

    public void Enter(UnitController unitController, Vector3 destination)
    {
        _unitController = unitController;
        _stateManager = unitController.UnitStateManager;
        _destination = destination;
        _building = unitController.GetBuilding();
        _data = unitController.GetBuildData();
        _player = _building.Player;

        unitController.PlayAnime("Build", true);
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
        if(_building == null)
        {
            _stateManager.SetState(new IdleState(), _unitController);
            return;
        }

        _building.Construct(Time.deltaTime);

        if(_building.IsComplet)
        {
            var newBuilding = GameObject.Instantiate(_data.BuildingPrefab, _destination, Quaternion.identity);
            var building = newBuilding.GetComponent<Building>();
            building.Init(_player, _building.CurrentHP);
            GameObject.Destroy(_building.gameObject);
            _stateManager.SetState(new IdleState(), _unitController);
        }
    }
}
