using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "SO/BuildingBlueprintDataSO", order = 4)]
public class BuildingBlueprintDataSO : ScriptableObject
{
    public string Name;
    public GameObject PreviewPrefab;
    public string PreviewName;
    public GameObject BuildingPrefab;
    public string BuildingName;
    public float BuildTime;
    public Sprite Icon;
    public int MaxHP;
    public int Defend;
    public BuildingDataSO buildingData;

    [Header("비용")]
    public List<ResourceCost> ResourceCosts;

    [Header("AI 관련")]
    public int ID;
}
