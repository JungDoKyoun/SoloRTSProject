using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Unit/UnitDataSO")]
public class UnitDataSO : ScriptableObject
{
    [Header("기본 정보")]
    public string Name;
    public int ID;
    public GameObject Prefab;

    [Header("스텟")]
    public int MaxHp;
    public int AttackRange;
    public int DetectRange;
    public float MoveSpeed;
    public float Damage;
    public float Defend;
    public double AttackCoolTime;

    //[Header("기타 항목")]
}
