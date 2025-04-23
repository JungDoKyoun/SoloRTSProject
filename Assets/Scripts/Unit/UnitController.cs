using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum TeamType
{
    Ally, Enemy
}

public class UnitController : MonoBehaviourPunCallbacks, IAttackable
{
    [Header("공용")]
    private NavMeshAgent _agent;
    private Renderer _renderer;
    private UnitDataSO _unitData;
    private UnitStateManager _unitStateManager;
    private IAttackable _target; //유닛이 타겟으로 지정할 목표
    private Animator _anime;
    private Player _player;
    private UnitType _unitType;
    private Vector3 _moveDestination;
    private int _maxHP;
    private float _currentHP;
    private bool _isSelect = false;
    private bool _isDie = false;

    [Header("공격관련")]
    private IAttackStrategy _attackStrategy;
    private double _lastAttack;
    private bool _isManualAttack = false;
    private bool _isAttack = false;

    [Header("원거리 관련")]
    [SerializeField] Transform _firePoin;

    [Header("일꾼관련")]
    private Resource _currentResources;
    private Building _building;
    private BuildingBlueprintDataSO _buildData;
    private ResourcesType _currentResourceType;
    private int _maxCarryAmount;
    private int _currentCarryAmount;
    private int _gatherAmountPerTick;
    private float _gatherTickInterval;
    private bool _isGather = false;
    private bool _isFull = false;
    private Coroutine _gatherCoroutine;

    public Transform FirePoin { get { return _firePoin; } }
    public UnitDataSO UnitData { get { return _unitData; } }
    public UnitStateManager UnitStateManager { get { return _unitStateManager; } }
    public Player Player { get { return _player; } }
    public UnitType UnitType { get { return _unitType; } }
    public ResourcesType CurrentResourceType { get { return _currentResourceType; } }
    public float CurrentHP { get { return _currentHP; } set { _currentHP = value; } }
    public int CurrentCarryAmount { get { return _currentCarryAmount; } set { _currentCarryAmount = value; } }
    public bool IsSelect { get { return _isSelect; } set { _isSelect = value; } }
    public bool IsManualAttack { get { return _isManualAttack; } set { _isManualAttack = value; } }
    public bool IsAttack { get { return _isAttack; } set { _isAttack = value; } }
    public bool IsGather { get { return _isGather; } set { _isGather = value; } }
    public bool IsFull { get { return _isFull; } set { _isFull = value; } }
    public bool IsDestroyed { get { return _isDie; } set { _isDie = value; } }
    public Vector3 Position => transform.position;

    private void Start()
    {
        UnitRegistry.Instance.Register(this);
    }

    private new void OnDisable()
    {
        if (UnitRegistry.Instance != null)
        {
            UnitRegistry.Instance.UnRegister(this);
        }
    }

    public void OnHit()
    {
        if (_target == null || _target.IsDestroyed)
        {
            return;
        }

        _attackStrategy.ExecuteAttack(this, _target);
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
        _unitType = _unitData.UnitType;
        _maxCarryAmount = _unitData.MaxCarryAmount;
        _gatherAmountPerTick = _unitData.GatherAmountPerTick;
        _gatherTickInterval = _unitData.GatherTickInterval;
        _currentHP = _maxHP;
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public bool IsPlayerUnit(Player player)
    {
        return _player == player;
    }

    private void SetAttackType()
    {
        switch (_unitData.AttackType)
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
        _anime.SetTrigger(animeName);

        //if (GameModManager.IsMultiplayer)
        //{
        //    photonView.RPC("RPCTriggerAnime", RpcTarget.All, animeName);
        //}

        //else
        //{
        //    _anime.SetTrigger(animeName);
        //}
    }

    //[PunRPC]
    //private void RPCTriggerAnime(string animeName)
    //{
    //    _anime.SetTrigger(animeName);
    //}

    public void PlayAnime(string animeName, bool TorF)
    {
        _anime.SetBool(animeName, TorF);

        //if (GameModManager.IsMultiplayer)
        //{
        //    photonView.RPC("RPCBoolAnime", RpcTarget.All, animeName, TorF);
        //}

        //else
        //{
        //    _anime.SetBool(animeName, TorF);
        //}
    }

    //[PunRPC]
    //private void RPCBoolAnime(string animeName, bool TorF)
    //{
    //    _anime.SetBool(animeName, TorF);
    //}

    public void MoveTo(Vector3 destination)
    {
        SetMoveDestination(destination);
        _agent.isStopped = false;
        _agent.SetDestination(destination);
        PlayAnime("Walk", true);
    }

    public void RotateToMoveDirection(bool onlyWhenMoving = true)
    {
        if (!_agent.pathPending && _agent.hasPath)
        {
            if (onlyWhenMoving && _agent.velocity.sqrMagnitude < 0.01f)
            {
                return;
            }

            Vector3 dir = _agent.desiredVelocity;

            if (_agent.velocity.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }

    public void RotateToTarget(IAttackable target)
    {
        if (target != null)
        {
            Vector3 dir = target.Position - transform.position;

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

    public void SetTarget(IAttackable target, bool isManualAttack)
    {
        _target = target;
        _isManualAttack = isManualAttack;
    }

    public void ClearTarget()
    {
        _target = null;
        _isManualAttack = false;
    }

    public IAttackable GetTarget()
    {
        return _target;
    }

    public bool IsEnemy(UnitController other)
    {
        if(_player.TeamType != other.Player.TeamType)
        {
            return true;
        }
        return false;
    }

    public bool IsEnemy(Building building)
    {
        if(_player.TeamType != building.Player.TeamType)
        {
            return true;
        }
        return false;
    }

    public bool IsAlly(UnitController other)
    {
        if(_player.TeamType == other.Player.TeamType)
        {
            return true;
        }
        return false;
    }

    public bool IsAlly(Building building)
    {
        if(_player.TeamType == building.Player.TeamType)
        {
            return true;
        }
        return false;
    }

    public UnitController FindTarget()
    {
        float minDist = _unitData.DetectRange;
        UnitController target = null;

        foreach (var other in UnitRegistry.Instance.AllUnits)
        {
            if (other == null || other == this || !IsEnemy(other))
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, other.transform.position);

            if (distance <= minDist && !other.IsDestroyed)
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

    public void StopAttack()
    {
        _isAttack = false;
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

    public void TakeDamage(int damage)
    {
        
        if (GameModManager.IsMultiplayer && !PhotonNetwork.IsMasterClient)
        {
            return;
        }

        _currentHP -= Mathf.Max(1, damage - _unitData.Defend);
        Debug.Log(_currentHP);

        if (GameModManager.IsMultiplayer)
        {
            photonView.RPC("RPCSyncHP", RpcTarget.All, _currentHP);
        }

        if (_currentHP <= 0)
        {
            _currentHP = 0;
            _player.DecreaseCurrentSupply(_unitData.SupplyCost);

            if (GameModManager.IsMultiplayer)
            {
                photonView.RPC("RPCSyncDie", RpcTarget.All);
            }
            else
            {
                Die();
            }
        }
    }

    //[PunRPC]
    //public void RPCTakeDamage(int damage)
    //{
    //    if (_isDie || !PhotonNetwork.IsMasterClient)
    //    {
    //        return;
    //    }

    //    _currentHP -= Mathf.Max(1, damage - _unitData.Defend);

    //    photonView.RPC("SynceHP", RpcTarget.All, _currentHP);

    //    if (_currentHP <= 0)
    //    {
    //        _currentHP = 0;
    //        photonView.RPC("SynceDie", RpcTarget.All);
    //    }
    //}

    [PunRPC]
    private void RPCSyncHP(float HP)
    {
        _currentHP = HP;
    }

    public void Die()
    {
        _isDie = true;
    }

    [PunRPC]
    private void RPCSyncDie()
    {
        _isDie = true;
    }

    public void SetResources(Resource resources)
    {
        _currentResources = resources;
        _currentResourceType = resources.Type;
    }

    public Resource GetResources()
    {
        return _currentResources;
    }

    public void StartGather()
    {
        if (_currentResources == null || !_currentResources.IsAvailable())
        {
            return;
        }

        if(_gatherCoroutine != null)
        {
            StopCoroutine(_gatherCoroutine);
        }

        PlayAnime("Gather", true);
        _currentResources.AssignWorker(this);
        _isGather = true;

        _gatherCoroutine = StartCoroutine(GatherCO());
    }

    public void StopGather()
    {
        if(_currentResources != null && _currentResources.CurrentWorker == this)
        {
            _currentResources.CurrentWorker = null;
        }

        if (_gatherCoroutine != null)
        {
            StopCoroutine(_gatherCoroutine);
            _gatherCoroutine = null;
        }

        PlayAnime("Gather", false);
        _isGather = false;
        _currentResources = null;
    }

    public bool IsReturnToBase(out Vector3 destination)
    {
        var nearDepot = Utils.FindNearestOwnedDepot(this);

        if(nearDepot != null)
        {
            destination = nearDepot.transform.position;
            SetMoveDestination(destination);
            return true;
        }
        destination = Vector3.zero;
        return false;
    }

    public void CarriedResource()
    {
        _currentCarryAmount = 0;
    }

    public bool IsCarryOver()
    {
        return _currentCarryAmount <= 0;
    }

    public bool IsWorker()
    {
        return _unitType == UnitType.Worker;
    }

    public void SetBuilding(Building building)
    {
        _building = building;
    }

    public Building GetBuilding()
    {
        return _building;
    }

    public void SetBuildData(BuildingBlueprintDataSO data)
    {
        _buildData = data;
    }

    public BuildingBlueprintDataSO GetBuildData()
    {
        return _buildData;
    }

    [PunRPC]
    public void RPCRequestBuild(Vector3 pos, string buildDataName, int playerID, int unitID)
    {
        var buildData = Resources.Load<BuildingBlueprintDataSO>("BuildingGhostData/" + buildDataName);
        var player = PlayerManager.Instance.GetPlayer(playerID);

        if(buildData == null || player == null)
        {
            return;
        }

        var unit = PhotonView.Find(unitID);
        if(unit == null)
        {
            return;
        }
        player.UseResources(buildData.ResourceCosts);
        var buildGhostObj = PhotonNetwork.Instantiate(buildData.PreviewName, pos, Quaternion.identity);
        var building = buildGhostObj.GetComponent<Building>();
        building.Init(buildData, player);
        var collider = buildGhostObj.AddComponent<MeshCollider>();

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        var worker = unit.GetComponent<UnitController>();

        worker.SetBuildData(buildData);
        worker.SetBuilding(building);

        new BuildCommand(worker, pos, building, buildData).Execute();
    }

    [PunRPC]
    public void RPCRequestResumeBuild(int buildingName, int unitID)
    {
        var buildingPhotonView = PhotonView.Find(buildingName);
        var workerPhotonView = PhotonView.Find(unitID);

        if (buildingPhotonView == null || workerPhotonView == null)
        {
            return;
        }

        var building = buildingPhotonView.GetComponent<Building>();
        var worker = workerPhotonView.GetComponent<UnitController>();

        if (!building.IsPlayerBuilding(worker.Player))
        {
            return;
        };

        worker.SetBuilding(building);
        worker.SetBuildData(building.GetBuildData());

        Vector3 buildPos = building.transform.position;
        var buildingData = building.GetBuildData();

        new BuildCommand(worker, buildPos, building, buildingData).Execute() ;
    }

    public void RequestStateChange(string stateName, Vector3 destination = new Vector3())
    {
        if(GameModManager.IsMultiplayer)
        {
            photonView.RPC("RPCRequestStateChange", RpcTarget.MasterClient, stateName, destination, photonView.ViewID);
        }

        else
        {
            UnitStateManager.SetState(Utils.GetStateByName(stateName), this, destination);
        }
    }

    [PunRPC]
    public void RPCRequestStateChange(string stateName, Vector3 destination, int unitID)
    {
        var PhtonViewunit = PhotonView.Find(unitID);
        UnitController unit = PhtonViewunit.GetComponent<UnitController>();

        unit.UnitStateManager.SetState(Utils.GetStateByName(stateName), unit, destination);

        photonView.RPC("RPCBroadcastStateChange", RpcTarget.Others, stateName, destination, unitID);
    }

    [PunRPC]
    public void RPCBroadcastStateChange(string stateName, Vector3 destination, int unitID)
    {
        var PhtonViewunit = PhotonView.Find(unitID);
        UnitController unit = PhtonViewunit.GetComponent<UnitController>();

        unit.UnitStateManager.SetState(Utils.GetStateByName(stateName), unit, destination);
    }

    private IEnumerator AttackCo()
    {
        yield return null;
        yield return new WaitUntil(() => _anime.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

        _isAttack = false;
    }

    private IEnumerator GatherCO()
    {
        while(_isGather && _currentResources != null)
        {
            if(!_isGather || _currentResources == null)
            {
                yield break;
            }

            yield return new WaitForSeconds(_gatherTickInterval);

            int gatherAmount = Mathf.Min(_gatherAmountPerTick, _currentResources.RemainAmount);
            _currentCarryAmount += gatherAmount;

            if(GameModManager.IsMultiplayer)
            {
                if(PhotonNetwork.IsMasterClient)
                {
                    _currentResources.ReduceAmount(gatherAmount);
                }
            }
            else
            {
                _currentResources.ReduceAmount(gatherAmount);
            }

            if(_currentCarryAmount >= _maxCarryAmount)
            {
                _isFull = true;
                yield break;
            }
        }
    }

    public bool IsEnemy(Player player)
    {
        throw new System.NotImplementedException();
    }
}
