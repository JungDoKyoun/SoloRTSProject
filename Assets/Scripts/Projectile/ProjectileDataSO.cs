using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Projectile/ProjectileDataSO")]
public class ProjectileDataSO : ScriptableObject
{
    public string ProjectileName;
    public GameObject ProjectilePrefab;
    public float Damage;
    public float Speed;
}
