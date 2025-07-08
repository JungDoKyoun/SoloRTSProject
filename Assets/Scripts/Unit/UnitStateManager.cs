using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateManager : MonoBehaviour
{
    private IUnitState _state;

    public IUnitState State => _state;

    private void Update()
    {
        _state?.Update();
    }

    private void FixedUpdate()
    {
        _state?.FixedUpdate();
    }

    public void SetState(IUnitState state, UnitController unitController, Vector3 destination = default)
    {
        if (_state != null)
        {
            _state.Exit();
        }
        _state = state;
        _state.Enter(unitController, destination);
    }
}
