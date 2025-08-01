using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitProducer
{
    void ProduceUnit(UnitDataSO unitData);
    void CancelProduction(int queueIndex);
    bool CanProduce(int unitID);
    UnitDataSO GetUnitDataByID(int unitID);
    void RPCProduceUnit(int unitID, int playerID);
}
