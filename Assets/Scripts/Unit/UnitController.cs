using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public enum TeamType
{
    Player, Ally, Enemy
}

public class UnitController : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform _firePoin;
    private NavMeshAgent _agent;
    private Renderer _renderer;
    private UnitDataSO _unitData;
    private UnitStateManager _unitStateManager;
    private UnitController _target; //유닛이 타겟으로 지정할 목표
    private Animator _anime;
    private IAttackStrategy _attackStrategy;
    private TeamType _teamType;
    private Vector3 _moveDestination;
    private int _maxHP;
    private double _lastAttack;
    private float _currentHP;
    private bool _isSelect = false;
    private bool _isDie = false;
    private bool _isManualAttack = false;
    private bool _isAttack = false;

    public Transform FirePoin { get { return _firePoin; } }
    public UnitDataSO UnitData { get { return _unitData; } }
    public UnitStateManager UnitStateManager { get { return _unitStateManager; } }
    public float CurrentHP { get { return _currentHP; } set { _currentHP = value; } }
    public bool IsSelect { get { return _isSelect; } set { _isSelect = value; } }
    public bool IsPlayerUnit => _teamType == TeamType.Player;
    public bool IsDie { get { return _isDie; } set { _isDie = value; } }
    public bool IsManualAttack { get { return _isManualAttack; } set { _isManualAttack = value; } }
    public bool IsAttack { get { return _isAttack; } set { _isAttack = value; } }

    private void Start()
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

    public void OnHit()
    {
        if(_target == null || _target.IsDie)
        {
            return;
        }

        _attackStrategy.ExecuteAttack(this, _target);
        Debug.Log(_attackStrategy);
    }

    public void Init(UnitManager unitManager)
    {
        _agent = unitManager.NavMeshAgent;
        _renderer = unitManager.Renderer;
        _unitStateManager = unitManager.UnitStateManager;
        _anime = unitManager.Anime;
        _unitData = unitManager.UnitDataSO;

        _unitStateManager.SetState(new IdleState(), this);
        SetAttackType();
        _agent.updateRotation = false;
        _agent.angularSpeed = 0f; 
        _agent.acceleration = 1000f;
        _agent.stoppingDistance = 0f;
        _agent.speed = _unitData.MoveSpeed;
        _maxHP = _unitData.MaxHp;
        _currentHP = _maxHP;
    }

    public void SetTeam(TeamType team)
    {
        _teamType = team;
    }

    private void SetAttackType()
    {
        switch(_unitData.AttackType)
        {
            case AttackType.Melee:
                {
                    _attackStrategy = new MeleeAttack();
                    break;
                }
            case AttackType.Ranged:
                {
                    _attackStrategy = new RangedAttack();
                    break;
                }
        }
    }

    public void PlayAnime(string animeName)
    {
        if(GameModManager.IsMultiplayer)
        {
            photonView.RPC("RPCTriggerAnime", RpcTarget.All, animeName);
        }

        else
        {
            _anime.SetTrigger(animeName);
        }
    }

    [PunRPC]
    private void RPCTriggerAnime(string animeName)
    {
        _anime.SetTrigger(animeName);
    }

    public void PlayAnime(string animeName, bool TorF)
    {
        if (GameModManager.IsMultiplayer)
        {
            photonView.RPC("RPCBoolAnime", RpcTarget.All, animeName, TorF);
        }

        else
        {
            _anime.SetBool(animeName, TorF);
        }
    }

    [PunRPC]
    private void RPCBoolAnime(string animeName, bool TorF)
    {
        _anime.SetBool(animeName, TorF);
    }

    public void MoveTo(Vector3 destination)
    {
        _agent.isStopped = false;
        _agent.SetDestination(destination);
        SetMoveDestination(destination);
        PlayAnime("Walk", true);
    }

    public void RotateToMoveDirection(bool onlyWhenMoving = true)
    {
        if(!_agent.pathPending && _agent.hasPath)
        {
            if(onlyWhenMoving && _agent.velocity.sqrMagnitude < 0.01f)
            {
                return;
            }

            Vector3 dir = _agent.desiredVelocity;

            if(_agent.velocity.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }

    public void RotateToTarget(UnitController target)
    {
        if(target != null)
        {
            Vector3 dir = target.transform.position - transform.position;

            dir.y = 0;

            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    public bool IsArrive()
    {
        return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
    }

    public void MoveStop()
    {
        _agent.ResetPath();
        PlayAnime("Walk", false);
    }

    public void SetMoveDestination(Vector3 Destination)
    {
        _moveDestination = Destination;
    }

    public Vector3 GetMoveDestination()
    {
        return _moveDestination;
    }

    public void SetTarget(UnitController target , bool isManualAttack)
    {
        _target = target;
        _isManualAttack = isManualAttack;
    }

    public void ClearTarget()
    {
        _target = null;
        _isManualAttack = false;
    }

    public UnitController GetTarget()
    {
        return _target;
    }

    public bool IsEnemy(UnitController me, UnitController other)
    {
        if(me._teamType == TeamType.Player)
        {
            return other._teamType == TeamType.Enemy;
        }

        if(me._teamType == TeamType.Ally)
        {
            return other._teamType == TeamType.Enemy;
        }

        if(me._teamType == TeamType.Enemy)
        {
            return other._teamType == TeamType.Player || other._teamType == TeamType.Ally;
        }
        return false;
    }

    public UnitController FindTarget()
    {
        float minDist = _unitData.DetectRange;
        UnitController target = null;

        foreach(var other in UnitRegistry.Instance.AllUnits)
        {
            if(other == null || other == this || !IsEnemy(this, other))
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, other.transform.position);

            if(distance <= minDist && !other.IsDie)
            {
                minDist = distance;
                target = other;
            }
        }
        return target;
    }

    public void Attack()
    {
        PlayAnime("Attack");
        _isAttack = true;
        _lastAttack = Utils.GetTime();
        StartCoroutine(AttackCo());
    }

    public bool CanAttack()
    {
        double currentTime = Utils.GetTime();
        return currentTime - _lastAttack >= UnitData.AttackCoolTime;
    }

    public void ResetBool()
    {
        _isAttack = false;
    }

    public void TakeDamage(float damage)
    {
        _currentHP -= Mathf.Max(1, damage - _unitData.Defend);

        if(_currentHP <= 0)
        {
            _currentHP = 0;
            Die();

            if(GameModManager.IsMultiplayer && PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("SynceDie", RpcTarget.Others);
            }
        }

        if (GameModManager.IsMultiplayer && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SynceHP", RpcTarget.Others, _currentHP);
        }
    }

    [PunRPC]
    public void RPCTakeDamage(float damage)
    {
        if(_isDie)
        {
            return;
        }

        TakeDamage(damage);
    }

    [PunRPC]
    private void SynceHP(float HP)
    {
        _currentHP = HP;
    }

    public void Die()
    {
        _isDie = true;
    }

    [PunRPC]
    private void SynceDie()
    {
        _isDie = true;
    }

    private IEnumerator AttackCo()
    {
        yield return null;
        yield return new WaitUntil(() => _anime.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

        _isAttack = false;
    }
}
