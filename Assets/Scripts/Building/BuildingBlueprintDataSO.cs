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
    public BuildingDataSO buildingData;

    [Header("비용")]
    public List<ResourceCost> ResourceCosts;

    [Header("건물 크기")]
    public Vector3 BuildSize = new Vector3(0f, 0f, 0f);
}
