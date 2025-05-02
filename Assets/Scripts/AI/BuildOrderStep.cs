using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildConditionType
{
    Always,
    MaintainBuildingCount,
    RequireSupplyBuffer,
    RequireTech
}

[System.Serializable]
public class BuildOrderStep
{
    public int UnitID;
    public BuildConditionType BuildConditionType;
    public int Threshold;
    public float Weight;
    public int RequiredTechID;
}
