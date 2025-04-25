using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MainMenu, TeamSelectMenu, Loading, Playing, GameOver
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private GameState _currentState;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }
    public GameState CurrentGameState => _currentState;

    public void SetGameState(GameState state)
    {
        _currentState = state;
    }

    public bool IsMainMenu => _currentState == GameState.MainMenu;
    public bool IsTeamSelectMenu => _currentState == GameState.TeamSelectMenu;
    public bool IsLoading => _currentState == GameState.Loading;
    public bool IsPlaying => _currentState == GameState.Playing;
    public bool IsGameOver => _currentState == GameState.GameOver;
}
