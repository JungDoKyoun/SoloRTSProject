using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    [HideInInspector] public UnitStateManager UnitStateManager { get; private set; }
    [HideInInspector] public UnitController UnitController { get; private set; }
    [HideInInspector] public NavMeshAgent NavMeshAgent { get; private set; }
    [HideInInspector] public Renderer Renderer { get; private set; }

    private void Awake()
    {
        UnitStateManager = GetComponent<UnitStateManager>();
        UnitController = GetComponent<UnitController>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Renderer = GetComponent<Renderer>();
        UnitController.Init(this);
    }
}
