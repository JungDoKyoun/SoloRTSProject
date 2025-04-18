using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourcesType
{
    Gold, Wood
}

[CreateAssetMenu(fileName = "ResourcesData", menuName = "SO/ResourcesDataSO", order = 2)]
public class ResourcesDataSO : ScriptableObject
{
    public string ResourcesName;
    public ResourcesType ResourcesType;
    public int MaxAmount;
}
