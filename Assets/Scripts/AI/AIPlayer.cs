using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    private AIStrategySO _aiStrategyData;
    private AIMapping _aiMapping;
    private AIStateManager _aiStateManager;
    private StrategyPhase _currentPhase;
    private Player _player;
    private int _playerID;
    private RaceType _raceType;
    private TeamType _teamType;
    private float _startTime;
    private int _phaseIndex = 0;
    private int _workerAssignIndex;

    [SerializeField] private float _searchRadius;

    private void Update()
    {
        if (_aiStateManager != null)
        {
            _aiStateManager.Update();
        }
        CheckPhaseTransition();
    }

    public StrategyPhase CurrentPhase => _currentPhase;
    public int PlayerID => _playerID;
    public int WorkerAssignIndex { get { return _workerAssignIndex; } set { _workerAssignIndex = value; } }

    public void Init(Player player)
    {
        _player = player;
        _playerID = player.PlayerID;
        _raceType = player.RaceType;
        _teamType = player.TeamType;
        _startTime = Time.time;

        _aiStrategyData = LoadStrategyForRace(_raceType);
        _aiMapping = new AIMapping();
        _aiMapping.Init(_raceType);
        _currentPhase = _aiStrategyData.StrategyPhases[_phaseIndex];

        _aiStateManager = new AIStateManager();
        _aiStateManager.SetState(new AIStartState(), this, _aiStateManager);
    }

    private AIStrategySO LoadStrategyForRace(RaceType race)
    {
        string[] paths = null;

        switch (race)
        {
            case RaceType.Human:
                paths = new string[]
                {
                    "Human/AIStrategyData/EarlyAttack"
                };
                break;
        }

        int index = Random.Range(0, paths.Length);
        return Resources.Load<AIStrategySO>(paths[index]);
    }

    private void CheckPhaseTransition()
    {
        if (_phaseIndex < _aiStrategyData.StrategyPhases.Count - 1)
        {
            float elapseTime = Time.time - _startTime;

            if (elapseTime >= _aiStrategyData.StrategyPhases[_phaseIndex].TransitionTime)
            {
                _phaseIndex++;
                _currentPhase = _aiStrategyData.StrategyPhases[_phaseIndex];
            }
        }
    }

    public void TryBuild()
    {
        if (_currentPhase == null || _currentPhase.BuildOrderStep == null || _currentPhase.BuildOrderStep.Count == 0)
            return;

        List<BuildOrderStep> buildOrderSteps = _currentPhase.BuildOrderStep;

        foreach (var buildOrderStep in buildOrderSteps)
        {
            if (BuildingRegistry.Instance.HasBuilding(_playerID, buildOrderStep.BuildingID))
                continue;

            var buildingData = _aiMapping.GetBuildingByID(buildOrderStep.BuildingID);

            float supplyRatio = (float)_player.CurrentSupply / _player.MaxSupply;

            if (supplyRatio >= 0.8f && _player.MaxSupply < 200)
            {
                var houseData = _aiMapping.GetBuildingByID(1);
                if (houseData != null && houseData.buildingData.CanSupply)
                {
                    buildingData = houseData;
                }
            }

            if (!_player.IsEnoughResources(buildingData.ResourceCosts))
                continue;

            if (AIUtils.FindBuildPos(_player, buildingData, _searchRadius, out Vector3 buildPos))
            {
                var workers = UnitRegistry.Instance.AllUnits.FindAll(u => u.Player == _player && u.UnitType == UnitType.Worker);

                UnitController selectworker = workers.FirstOrDefault(u => u.IsIdle());

                if (selectworker == null)
                {
                    selectworker = workers[0];
                }

                if (GameModManager.IsMultiplayer)
                {
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        return;
                    }

                    selectworker.photonView.RPC("RPCRequestBuild", RpcTarget.All, buildPos, buildingData.Name, _playerID, selectworker.UnitInstanceID);
                }

                else
                {
                    selectworker.RequestBuild(buildPos, buildingData.Name, _playerID, selectworker);
                }

                break;
            }
        }
    }

    public void TryTrainUnit()
    {
        if (_currentPhase == null || _currentPhase.TrainOrderStep == null || _currentPhase.TrainOrderStep.Count == 0)
            return;

        if (GameModManager.IsMultiplayer && !PhotonNetwork.IsMasterClient)
            return;

        List<Building> buildings = BuildingRegistry.Instance.GetBuildings(_playerID);

        List<TrainOrderStep> trainOrders = new List<TrainOrderStep>();
        foreach (var unit in _currentPhase.TrainOrderStep)
        {
            if (CheckCondition(unit))
            {
                trainOrders.Add(unit);
            }
        }

        trainOrders.Sort((a, b) => b.Weight.CompareTo(a.Weight));

        if (trainOrders.Count > 0)
        {
            int unitID = trainOrders[0].UnitID;
            var unitData = _aiMapping.GetUnitByID(unitID);

            foreach (var building in buildings)
            {
                if (building is IUnitProducer producer && building.IsUsable() && producer.CanProduce(unitID))
                {
                    if (_player.IsCanProduceUnit(unitData.SupplyCost))
                    {
                        producer.ProduceUnit(unitData);
                        return;
                    }
                }
            }
        }
    }

    private bool CheckCondition(TrainOrderStep step)
    {
        switch (step.ConditionType)
        {
            case TrainConditionType.Always:
                return true;

            case TrainConditionType.UntilUnitCount:
                {
                    int count = 0;
                    foreach (var u in UnitRegistry.Instance.AllUnits)
                    {
                        if (u.Player == _player && u.UnitData.ID == step.UnitID && !u.IsDestroyed)
                            count++;
                    }

                    return count < step.Threshold;
                }

            case TrainConditionType.AfterSupply:
                var unitData = _aiMapping.GetUnitByID(step.UnitID);
                return _player.IsCanProduceUnit(unitData.SupplyCost);

            default:
                return false;
        }
    }

    public void CheckNewWorkers()
    {
        var allWorkers = UnitRegistry.Instance.GetAllUnits(_playerID, 0);

        foreach (var worker in allWorkers)
        {
            if (worker.IsIdle())
            {
                AIWorkerManager.AssignNewWorker(worker, this);
            }
        }
    }
}
