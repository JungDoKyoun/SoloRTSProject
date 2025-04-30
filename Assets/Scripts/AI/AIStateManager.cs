using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateManager
{
    private IAIState _currentState;

    public void SetState(IAIState newState, AIPlayer aiPlayer, AIStateManager aiStateManager)
    {
        _currentState = newState;
        _currentState.Enter(aiPlayer, aiStateManager);
    }

    public void Update()
    {
        _currentState.Update();
    }
}
