using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerRegistry : MonoBehaviour
{
    private static AIPlayerRegistry _instance;
    private Dictionary<int, AIPlayer> _allAIPlayer = new Dictionary<int, AIPlayer>();

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static AIPlayerRegistry Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<AIPlayerRegistry>();
            }
            return _instance;
        }
    }

    public void RegisterAIPlayer(int playerID ,AIPlayer aIPlayer)
    {
        if(!_allAIPlayer.ContainsKey(playerID))
        {
            _allAIPlayer.Add(playerID, aIPlayer);
        }
    }

    public AIPlayer GetAIPlayer(int playerID)
    {
        _allAIPlayer.TryGetValue(playerID, out AIPlayer aIPlayer);
        return aIPlayer;
    }
}
