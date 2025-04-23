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
    private const int _maxSupplyCapacity = 200;
    private int _maxSupply;
    private int _currentSupply;

    public Player(int playerID, string nicName, TeamType teamType, RaceType raceType, bool isAI)
    {
        _playerID = playerID;
        _nicName = nicName;
        _teamType = teamType;
        _raceType = raceType;
        _isAI = isAI;

        _resources[ResourcesType.Gold] = 50;
        _resources[ResourcesType.Wood] = 0;
        _maxSupply = 0;
        _currentSupply = 0;
    }

    public Dictionary<ResourcesType, int> Resources => _resources;
    public int PlayerID => _playerID;
    public string NicName => _nicName;
    public TeamType TeamType => _teamType;
    public RaceType RaceType => _raceType;
    public bool IsAI => _isAI;
    public int MaxSupply => _maxSupply;
    public int CurrentSupply => _currentSupply;

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

    public void SetSupply(int newMaxSupply, int newCurrentSupply)
    {
        _maxSupply = newMaxSupply;
        _currentSupply = newCurrentSupply;
    }

    public bool IsCanProduceUnit(int amount)
    {
        return _currentSupply + amount <= _maxSupply;
    }

    public void IncreaseMaxSupply(int amount)
    {
        if(GameModManager.IsMultiplayer)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            _maxSupply = Mathf.Min(_maxSupply + amount, _maxSupplyCapacity);
            PlayerManager.Instance.SyncPlayerSupply(this, _maxSupply, _currentSupply);
        }
        else
        {
            _maxSupply = Mathf.Min(_maxSupply + amount, _maxSupplyCapacity);
        }
    }

    public void DecreaseMaxSupply(int amount)
    {
        if(GameModManager.IsMultiplayer)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            _maxSupply -= amount;
            PlayerManager.Instance.SyncPlayerSupply(this, _maxSupply, _currentSupply);
        }
        else
        {
            _maxSupply -= amount;
        }
    }

    public void IncreaseCurrentSupply(int amount)
    {
        if(GameModManager.IsMultiplayer)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            _currentSupply += amount;
            PlayerManager.Instance.SyncPlayerSupply(this, _maxSupply, _currentSupply);
        }
        else
        {
            _currentSupply += amount;
        }
    }

    public void DecreaseCurrentSupply(int amount)
    {
        if(GameModManager.IsMultiplayer)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            _currentSupply = Mathf.Max(0, _currentSupply - amount);
            PlayerManager.Instance.SyncPlayerSupply(this, _maxSupply, _currentSupply);
        }
        else
        {
            _currentSupply = Mathf.Max(0, _currentSupply - amount);
        }
    }
}
