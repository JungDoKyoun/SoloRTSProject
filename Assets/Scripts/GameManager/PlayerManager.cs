using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
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

    public void InitializePlayersFromSlots(List<SlotData> slots)
    {
        foreach(var slot in slots)
        {
            PlayerRegistry(slot.PlayerID, slot.NickName, slot.TeamType, slot.RaceType, slot.IsAI);
        }
    }

    public void PlayerRegistry(int playerID, string nickName, TeamType team, RaceType raceType ,bool isAI)
    {
        if(!_players.ContainsKey(playerID))
        {
            Player newplayer = new Player(playerID, nickName, team, raceType ,isAI);
            _players.Add(playerID, newplayer);

            if(GameModManager.IsMultiplayer)
            {
                if (playerID == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    _localPlayer = newplayer;
                }
            }
            else
            {
                _localPlayer = newplayer;
            }
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
        if(_players.TryGetValue(id, out var player))
        {
            return player;
        }
        return null;
    }

    public List<Player> GetAllPlayer()
    {
        return new List<Player>(_players.Values);
    }

    public void SyncPlayerResource(Player player, ResourcesType type, int newAmount)
    {
        photonView.RPC("RPCSyncPlayerResource", RpcTarget.All, player.PlayerID, type, newAmount);
    }

    [PunRPC]
    public void RPCSyncPlayerResource(int playerID, ResourcesType type, int newAmount)
    {
        var player = GetPlayer(playerID);
        if(player != null)
        {
            player.SetResource(type, newAmount);
        }
    }

    public void SyncPlayerSupply(Player player, int maxSupply, int currentSupply)
    {
        photonView.RPC("RPCSyncPlayerSupply", RpcTarget.All, player.PlayerID, maxSupply, currentSupply);
    }

    [PunRPC]
    public void RPCSyncPlayerSupply(int playerID, int maxSupply, int currentSupply)
    {
        var player = GetPlayer(playerID);

        if (player != null)
        {
            player.SetSupply(maxSupply, currentSupply);
        }
    }
}
