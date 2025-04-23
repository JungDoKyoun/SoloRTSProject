using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class DepositTrigger : MonoBehaviour
{
    Building _building;

    private void Start()
    {
        _building = GetComponentInParent<Building>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<UnitController>(out UnitController unit))
        {
            if(_building.IsPlayerBuilding(unit.Player))
            {
                if(_building is IResourceDepot depot)
                {
                    depot.ReceiveResource(unit.CurrentResourceType, unit.CurrentCarryAmount, unit.Player);
                    unit.CarriedResource();
                }
            }
        }
    }
}
