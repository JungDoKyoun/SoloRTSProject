using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIState
{
    public void Enter(AIPlayer aIPlayer, AIStateManager aIStateManager);
    public void Exit();
    public void Update();
}

public class AIStartState : IAIState
{
    private AIPlayer _aiPlayer;
    private AIStateManager _aiStateManager;
    private float _startTime;
    private float _waitDuration = 1f;

    public void Enter(AIPlayer aiPlayer, AIStateManager aiStateManager)
    {
        _aiPlayer = aiPlayer;
        _aiStateManager = aiStateManager;
        _startTime = Time.time;

        AIWorkerManager.AssignInitialWorkers(aiPlayer);
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        if(Time.time - _startTime >= _waitDuration)
        {
            _aiStateManager.SetState(new AIMainState(), _aiPlayer, _aiStateManager);
        }
    }
}

public class AIMainState : IAIState
{
    private AIPlayer _aiPlayer;
    private AIStateManager _aiStateManager;
    private float _lastBuildCheck;
    private float _lastTrainCheck;
    private float _lastWorkerCheck;
    private float _buildInterval = 5f;
    private float _trainInterval = 3f;
    private float _workerInterval = 2f;

    public void Enter(AIPlayer aiPlayer, AIStateManager aiStateManager)
    {
        _aiPlayer = aiPlayer;
        _aiStateManager = aiStateManager;

        _lastBuildCheck = Time.time;
        _lastTrainCheck = Time.time;
        _lastWorkerCheck = Time.time;
    }

    public void Exit()
    {

    }

    public void Update()
    {
        if (Time.time - _lastBuildCheck >= _buildInterval)
        {
            _aiPlayer.TryBuild();
            _lastBuildCheck = Time.time;
        }

        if (Time.time - _lastTrainCheck >= _trainInterval)
        {
            _aiPlayer.TryTrainUnit();
            _lastTrainCheck = Time.time;
        }

        if (Time.time - _lastWorkerCheck >= _workerInterval)
        {
            _aiPlayer.CheckNewWorkers();
            _lastWorkerCheck = Time.time;
        }
    }
}
