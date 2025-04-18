using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] BuildingDataSO _data;
    private Player _ownPlayer;

    public virtual void SetPlayer(Player player)
    {
        _ownPlayer = player;
    }

    public bool IsMyBuilding(Player unit)
    {
        return unit == _ownPlayer;
    }
    
    public BuildingDataSO GetBuildingData()
    {
        return _data;
    }
}
