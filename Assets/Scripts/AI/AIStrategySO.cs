using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="AIStrategyData", menuName = "SO/AIStrategySO",order = 5)]
public class AIStrategySO : ScriptableObject
{
    public List<StrategyPhase> StrategyPhases;
}
