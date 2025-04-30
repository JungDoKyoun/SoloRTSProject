using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrainConditionType
{
    Always,
    UntilUnitCount,
    AfterTime,
    AfterSupply,
    RequireTech
}

[System.Serializable]
public class TrainOrderStep
{
    public UnitRole UnitRole;
    public TrainConditionType ConditionType;
    public int Threshold;
    public float Weight;
}
