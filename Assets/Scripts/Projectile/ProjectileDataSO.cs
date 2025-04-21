using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "SO/ProjectileDataSO", order = 1)]
public class ProjectileDataSO : ScriptableObject
{
    public string ProjectileName;
    public GameObject ProjectilePrefab;
    public int Damage;
    public float Speed;
}
