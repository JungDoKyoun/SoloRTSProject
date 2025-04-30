using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingRole
{
    None,
    Base,
    Barracks,
    Archery,
    Stables,
    Temple,
    SupplyDepot,
    Tech1,
    Tech2
}

[CreateAssetMenu(fileName = "BuildingData", menuName = "SO/BuildingDataSO", order = 3)]
public class BuildingDataSO : ScriptableObject
{
    [Header("건물 기본정보")]
    public string Name;
    public int MaxHP;
    public int Defend;
    public float Sight;

    [Header("건물 인구관련")]
    public int SupplyProvided;

    [Header("건물 유닛 생성")]
    public bool IsCanProduceUnits;
    public List<UnitDataSO> TrainableUnits;

    [Header("AI전략 관련")]
    public BuildingRole BuildingRole;
}
