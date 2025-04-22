using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RaceType
{
    Human
}

public class Player
{
    private Dictionary<ResourcesType, int> _resources = new Dictionary<ResourcesType, int>();
    private int _playerID;
    private string _nicName;
    private TeamType _teamType;
    private RaceType _raceType;
    private bool _isAI;

    public Player(int playerID, string nicName, TeamType teamType, RaceType raceType, bool isAI)
    {
        _playerID = playerID;
        _nicName = nicName;
        _teamType = teamType;
        _raceType = raceType;
        _isAI = isAI;

        _resources[ResourcesType.Gold] = 50;
        _resources[ResourcesType.Wood] = 0;
    }

    public Dictionary<ResourcesType, int> Resources { get { return _resources; } }
    public int PlayerID { get { return _playerID; } }
    public string NicName { get { return _nicName; } }
    public TeamType TeamType { get { return _teamType; } }
    public RaceType RaceType { get { return _raceType; } }
    public bool IsAI { get { return _isAI; } }

    public bool IsAlly(Player player)
    {
        if(_teamType == player.TeamType)
        {
            return true;
        }
        return false;
    }

    public bool IsEnemy(Player player)
    {
        if(_teamType != player.TeamType)
        {
            return true;
        }
        return false;
    }

    public void AddResources(ResourcesType type, int amount)
    {
        int currentResources = _resources[type] + amount;

        if(GameModManager.IsMultiplayer)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            PlayerManager.Instance.SyncPlayerResource(this, type, currentResources);
        }
        else
        {
            SetResource(type, currentResources);
        }
    }

    public void AddResources(List<ResourceCost> costs)
    {
        foreach(var cost in costs)
        {
            ResourcesType type = cost.ResourcesType;
            int currentResources = _resources[type] + cost.Amount;

            if (GameModManager.IsMultiplayer)
            {
                if (!PhotonNetwork.IsMasterClient)
                {
                    return;
                }

                PlayerManager.Instance.SyncPlayerResource(this, type, currentResources);
            }
            else
            {
                SetResource(type, currentResources);
            }
        }
    }

    public bool IsenoughResources(List<ResourceCost> costs)
    {
        foreach(var cost in costs)
        {
            if (!_resources.ContainsKey(cost.ResourcesType) || _resources[cost.ResourcesType] < cost.Amount)
            {
                return false;
            }
        }
        return true;
    }

    public void UseResources(List<ResourceCost> costs)
    {
        foreach (var cost in costs)
        {
            ResourcesType type = cost.ResourcesType;
            int currentResources = _resources[type] - cost.Amount;

            if (GameModManager.IsMultiplayer)
            {
                if (!PhotonNetwork.IsMasterClient)
                {
                    return;
                }

                PlayerManager.Instance.SyncPlayerResource(this, type, currentResources);
            }
            else
            {
                SetResource(type, currentResources);
            }
        }
    }

    public void SetResource(ResourcesType type, int newAmount)
    {
        _resources[type] = newAmount;
    }
}
