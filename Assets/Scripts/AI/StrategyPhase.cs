using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StrategyPhase
{
    public string PhaseName;
    public float TransitionTime;

    [Header("유닛 훈련 및 건설 전략")]
    public List<TrainOrderStep> TrainOrderStep = new List<TrainOrderStep>();
    public List<BuildOrderStep> BuildOrderStep = new List<BuildOrderStep>();

    [Header("일꾼 자원 배치 목표 수")]
    public int TargetGoldWorkerCount;
    public int TargetWoodWorkerCount;
}
