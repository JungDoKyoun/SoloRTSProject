using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviourPunCallbacks
{
    [SerializeField] protected BuildingDataSO _data;
    protected Player _player;
    protected BuildingBlueprintDataSO _buildData;
    protected int _currentHP;
    protected float _elapseTime;
    protected bool _isComplet = false;

    public Player Player { get { return _player; } }
    public int CurrentHP { get { return _currentHP; } }
    public bool IsComplet { get { return _isComplet; } }

    public void Init(BuildingBlueprintDataSO data, Player player)
    {
        _buildData = data;
        _player = player;
        _currentHP = _data.MaxHP;
    }

    public void Init(Player player, int hp)
    {
        _player = player;
        _currentHP = hp;
    }

    public void Construct(float delta)
    {
        _elapseTime += delta;

        if(_elapseTime >= _buildData.BuildTime)
        {
            _isComplet = true;
        }
    }

    public bool IsMyBuilding(Player player)
    {
        return player == _player;
    }
    
    public BuildingDataSO GetBuildingData()
    {
        return _data;
    }
}
