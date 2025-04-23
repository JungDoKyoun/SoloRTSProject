using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "SO/BuildingDataSO", order = 3)]
public class BuildingDataSO : ScriptableObject
{
    public string Name;
    public int MaxHP;
    public int Defend;
    public float Sight;
    public bool IsCanProduceUnits;
    public List<UnitDataSO> TrainableUnits;
    public int SupplyProvided;
}
