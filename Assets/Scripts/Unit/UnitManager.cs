using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private UnitDataSO _unitDataSo;
    [HideInInspector] public UnitStateManager UnitStateManager { get; private set; }
    [HideInInspector] public UnitController UnitController { get; private set; }
    [HideInInspector] public NavMeshAgent NavMeshAgent { get; private set; }
    [HideInInspector] public Renderer Renderer { get; private set; }
    [HideInInspector] public Animator Anime { get; private set; }

    private void Awake()
    {
        UnitStateManager = GetComponent<UnitStateManager>();
        UnitController = GetComponent<UnitController>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Renderer = GetComponent<Renderer>();
        Anime = GetComponent<Animator>();
    }

    public UnitDataSO UnitDataSO { get { return _unitDataSo; } }
}
