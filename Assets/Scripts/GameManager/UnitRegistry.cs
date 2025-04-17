using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRegistry : MonoBehaviour
{
    private static UnitRegistry _instance;
    private List<UnitController> _allUnits = new List<UnitController>();

    public static UnitRegistry Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<UnitRegistry>();
            }
            return _instance;
        }
    }
    public List<UnitController> AllUnits { get { return _allUnits; } }

    public void Register(UnitController unit)
    {
        if(!_allUnits.Contains(unit))
        {
            _allUnits.Add(unit);
        }
    }

    public void UnRegister(UnitController unit)
    {
        if(_allUnits.Contains(unit))
        {
            _allUnits.Remove(unit);
        }
    }
}
