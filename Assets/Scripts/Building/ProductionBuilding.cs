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
        _productionQueue.AddProductionQueue(unitData);
    }

    public void CancelProduction(int queueIndex)
    {
        _productionQueue.CancelProduction(queueIndex);
    }
    
}
