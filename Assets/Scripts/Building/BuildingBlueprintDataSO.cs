using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "SO/BuildingBlueprintDataSO", order = 4)]
public class BuildingBlueprintDataSO : ScriptableObject
{
    public string Name;
    public GameObject BuildingPrefab;
    public GameObject PreviewPrefab;
    public float buildTime;
    public Sprite Icon;

    [Header("ºñ¿ë")]
    public List<ResourceCost> ResourceCosts;
}
