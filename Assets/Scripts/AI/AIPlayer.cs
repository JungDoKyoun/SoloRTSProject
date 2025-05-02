using System.Collections;
using System.Collections.Generic;
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

    private void Update()
    {
        if (_aiStateManager != null)
        {
            _aiStateManager.Update();
        }
        CheckPhaseTransition();
    }

    public int PlayerID => _playerID;

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

        switch(race)
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
        if(_phaseIndex < _aiStrategyData.StrategyPhases.Count)
        {
            float elapseTime = Time.time - _startTime;

            if (elapseTime >= _aiStrategyData.StrategyPhases[_phaseIndex].TransitionTime)
            {
                _phaseIndex++;
                _currentPhase = _aiStrategyData.StrategyPhases[_phaseIndex];
            }
        }
    }
}
