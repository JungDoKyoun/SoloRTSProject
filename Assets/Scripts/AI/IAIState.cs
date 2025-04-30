using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIState
{
    public void Enter(AIPlayer aIPlayer, AIStateManager aIStateManager);
    public void Update();
}

public class AIStartState : IAIState
{
    private AIPlayer _aiPlayer;
    private AIStateManager _aiStateManager;
    private float _startTime;

    public void Enter(AIPlayer aiPlayer, AIStateManager aiStateManager)
    {
        _aiPlayer = aiPlayer;
        _aiStateManager = aiStateManager;
        _startTime = Time.time;

        Debug.Log("초기 전략 들어옴");
    }

    public void Update()
    {
        Debug.Log("초기전략 진행중");
    }
}
