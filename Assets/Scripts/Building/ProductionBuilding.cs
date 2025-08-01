using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building, IUnitProducer
{
    protected UnitProductionQueue _productionQueue;

    private void Awake()
    {
        _productionQueue = GetComponent<UnitProductionQueue>();
    }

    private void Start()
    {
        _productionQueue.Init(_player);
    }

    public void ProduceUnit(UnitDataSO unitData)
    {
        if (GameModManager.IsMultiplayer)
        {
            photonView.RPC("RPCProduceUnit", RpcTarget.All, unitData.ID, _player.PlayerID);
        }
        else
        {
            _productionQueue.AddProductionQueue(unitData);
        }
    }
    public UnitDataSO GetUnitDataByID(int unitID)
    {
        if (_data != null && _data.TrainableUnits != null)
        {
            return _data.TrainableUnits.Find(u => u.ID == unitID);
        }
        return null;
    }

    [PunRPC]
    public void RPCProduceUnit(int unitID, int playerID)
    {
        var unitData = GetUnitDataByID(unitID);
        if (unitData != null)
        {
            _productionQueue.AddProductionQueue(unitData);
        }
    }

    public void CancelProduction(int queueIndex)
    {
        _productionQueue.CancelProduction(queueIndex);
    }

    public bool CanProduce(int unitID)
    {
        if (_data == null || !_data.IsCanProduceUnits)
            return false;

        return _data.TrainableUnits.Exists(u => u.ID == unitID);
    }
}
