using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    private AIStateManager _aiStateManager;
    private Player _player;
    private int _playerID;
    private RaceType _raceType;
    private TeamType _teamType;

    private void Update()
    {
        if (_aiStateManager != null)
        {
            _aiStateManager.Update();
        }
    }

    public void Init(Player player)
    {
        _player = player;
        _playerID = player.PlayerID;
        _raceType = player.RaceType;
        _teamType = player.TeamType;
        _aiStateManager = new AIStateManager();
        _aiStateManager.SetState(new AIStartState(), this, _aiStateManager);
    }
}
