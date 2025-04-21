using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    private Dictionary<int, Player> _players = new Dictionary<int, Player>();
    private Player _localPlayer;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static PlayerManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<PlayerManager>();
            }
            return _instance;
        }
    }
    public Player LocalPlayer { get { return _localPlayer; } }

    public void PlayerRegistry(Player player, bool isLocal = false)
    {
        if(!_players.ContainsKey(player.PlayerID))
        {
            _players.Add(player.PlayerID, player);
        }

        if(isLocal)
        {
            _localPlayer = player;
        }
    }

    public void PlayerRemove(Player player)
    {
        if (_players.ContainsKey(player.PlayerID))
        {
            _players.Remove(player.PlayerID);
        }
    }

    public Player GetPlayer(int id)
    {
        if(_players.TryGetValue(id, out Player player))
        {
            return player;
        }
        return null;
    }

    public List<Player> GetAllPlayer()
    {
        return new List<Player>(_players.Values);
    }
}
