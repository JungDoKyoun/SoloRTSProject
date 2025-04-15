using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum TeamType
{
    Player, Ally, Enemy
}

public class UnitController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Renderer _renderer;
    private UnitStateManager _unitStateManager;
    private TeamType _teamType;
    private bool _isSelect = false;

    public UnitStateManager UnitStateManager { get { return _unitStateManager; } }
    public bool IsSelect { get { return _isSelect; } set { _isSelect = value; } }
    public bool IsPlayerUnit => _teamType == TeamType.Player;

    private void Start()
    {
        SetTeam(TeamType.Player);
        UnitRegistry.Instance.Register(this);
    }

    private void OnEnable()
    {
        SetTeam(TeamType.Player);
        UnitRegistry.Instance.Register(this);
    }

    private void OnDisable()
    {
        if (UnitRegistry.Instance != null)
        {
            UnitRegistry.Instance.UnRegister(this);
        }
    }

    public void Init(UnitManager unitManager)
    {
        _agent = unitManager.NavMeshAgent;
        _renderer = unitManager.Renderer;
        _unitStateManager = unitManager.UnitStateManager;

        _unitStateManager.SetState(new IdleState(), this);
    }

    public void SetTeam(TeamType team)
    {
        _teamType = team;
    }

    public void MoveTo(Vector3 destination)
    {
        _agent.isStopped = false;
        _agent.SetDestination(destination);
    }

    public bool IsArrive()
    {
        return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
    }

    public void MoveStop()
    {
        _agent.ResetPath();
    }
}
