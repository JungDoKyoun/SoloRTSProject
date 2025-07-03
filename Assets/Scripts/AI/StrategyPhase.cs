using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StrategyPhase
{
    public string PhaseName;
    public float TransitionTime;

    [Header("À¯´Ö ÈÆ·Ã ¹× °Ç¼³ Àü·«")]
    public List<TrainOrderStep> TrainOrderStep = new List<TrainOrderStep>();
    public List<BuildOrderStep> BuildOrderStep = new List<BuildOrderStep>();

    [Header("ÀÏ²Û °ü·Ã")]
    public List<ResourcesType> GatherPattern = new List<ResourcesType>();
    public int TargetGoldWorkerCount;
    public int TargetWoodWorkerCount;
}
