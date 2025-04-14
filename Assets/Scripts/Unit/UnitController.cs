using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    private NavMeshAgent _agent;

    public void Init(UnitManager unitManager)
    {
        _agent = unitManager.NavMeshAgent;
    }
}
