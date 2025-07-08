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
            _aiStateManager.SetState(new AIBuildState(), _aiPlayer, _aiStateManager);
        }
    }
}

public class AIBuildState : IAIState
{
    private AIPlayer _aiPlayer;
    private AIStateManager _aiStateManager;
    private float _startTime;
    private float _interval;
    private float _elapsedTime;

    public void Enter(AIPlayer aiPlayer, AIStateManager aiStateManager)
    {
        _aiPlayer = aiPlayer;
        _aiStateManager = aiStateManager;
        _startTime = Time.time;
        _interval = 5f;
        _elapsedTime = 0f;
    }

    public void Exit()
    {

    }

    public void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime < _interval)
            return;

        _aiPlayer.TryBuild();
        _elapsedTime = 0f;
    }
}
