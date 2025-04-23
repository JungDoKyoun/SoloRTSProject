using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable 
{
    void TakeDamage(int damage);
    bool IsEnemy(Player player);
    bool IsDestroyed { get; }
    Vector3 Position { get; }
}
