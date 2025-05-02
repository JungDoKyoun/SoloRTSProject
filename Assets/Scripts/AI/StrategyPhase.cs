using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StrategyPhase
{
    public string PhaseName;
    public float TransitionTime;
    public List<TrainOrderStep> TrainOrderStep = new List<TrainOrderStep>();
    public List<BuildOrderStep> BuildOrderStep = new List<BuildOrderStep>();
}
