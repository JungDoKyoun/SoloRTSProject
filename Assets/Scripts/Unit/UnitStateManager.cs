using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateManager : MonoBehaviour
{
    private IUnitState _state;

    private void Start()
    {
        SetState(new IdleState());
    }

    private void Update()
    {
        _state?.Update();
    }

    private void FixedUpdate()
    {
        _state?.FixedUpdate();
    }

    public void SetState(IUnitState state)
    {
        if(_state != null)
        {
            _state.Exit();
        }
        _state = state;
        _state.Enter();
    }
}
