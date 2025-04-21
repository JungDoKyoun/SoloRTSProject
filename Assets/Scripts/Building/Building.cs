using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviourPunCallbacks
{
    [SerializeField] protected BuildingDataSO _data;
    protected Player _player;

    public virtual void SetPlayer(Player player)
    {
        _player = player;
    }

    public bool IsMyBuilding(Player unit)
    {
        return unit == _player;
    }
    
    public BuildingDataSO GetBuildingData()
    {
        return _data;
    }
}
