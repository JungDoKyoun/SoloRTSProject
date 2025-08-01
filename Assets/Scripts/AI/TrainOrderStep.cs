using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrainConditionType
{
    Always,
    UntilUnitCount,
    AfterSupply,
}

[System.Serializable]
public class TrainOrderStep
{
    public int UnitID;
    public TrainConditionType ConditionType;
    public int Threshold;
    public float Weight;
}
