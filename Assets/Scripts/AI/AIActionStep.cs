using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum BuildingType
{
    None = 0,
    ResourceDepot = 1 << 0,
    UnitProducer = 1 << 1,
    SupplyProvider = 1 << 2,
    ResearchCenter = 1 << 3
}

public enum AIActionType
{
    Build,
    Train,
    WaitForSupply,
    WaitForResource,
    WaitForTime
}

[System.Serializable]
public class AIActionStep
{
    public AIActionType ActionType;
    public BuildingBlueprintDataSO buildingBlueprintData;
    public UnitDataSO unitData;
    public int SupplyNeeded;
    public float DelayTime;
    public int ResourceRequired;
}
