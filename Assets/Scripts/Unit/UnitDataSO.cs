using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public enum AttackType
{
    Melee, Ranged
}

public enum UnitType
{
    Normal,Worker
}

[CreateAssetMenu(fileName = "UnitData", menuName = "SO/UnitDataSO", order = 0)]
public class UnitDataSO : ScriptableObject
{
    [Header("기본 정보")]
    public string Name;
    public int ID;
    public GameObject Prefab;

    [Header("스텟")]
    public int MaxHp;
    public int Damage;
    public int Defend;
    public float AttackRange;
    public float DetectRange;
    public float MoveSpeed;
    public double AttackCoolTime;

    [Header("기타 항목")]
    public UnitType UnitType;
    public AttackType AttackType;
    public ProjectileDataSO ProjectileData;

    [Header("일꾼 관련")]
    //public bool Gather;
    public int MaxCarryAmount;
    public int GatherAmountPerTick;
    public float GatherTickInterval;
    public float GatherSearchRadius;
    public float BuildDistance;
    public List<BuildingBlueprintDataSO> BuildingBlueprintDatas;
}
