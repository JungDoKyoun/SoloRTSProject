using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackStrategy
{
    void ExecuteAttack(UnitController attacker, IAttackable target);
}

public class MeleeAttack : IAttackStrategy
{
    public void ExecuteAttack(UnitController attacker, IAttackable target)
    {
        if(GameModManager.IsMultiplayer)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (target == null || !((MonoBehaviour)target).TryGetComponent<PhotonView>(out var view))
            {
                return;
            }

            target.TakeDamage(attacker.UnitData.Damage);
        }
        else
        {
            target.TakeDamage(attacker.UnitData.Damage);
        }
    }
}

public class RangedAttack : IAttackStrategy
{
    public void ExecuteAttack(UnitController attacker, IAttackable target)
    {
        if(GameModManager.IsMultiplayer)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (target == null || !((MonoBehaviour)target).TryGetComponent<PhotonView>(out var view))
            {
                return;
            }

            ProjectileSpawner.Instance.LaunchMultiplayer(attacker.UnitData.ProjectileData, attacker.FirePoin, target);
        }
        else
        {
            ProjectileSpawner.Instance.LaunchLocal(attacker.UnitData.ProjectileData, attacker.FirePoin, target);
        }
    }
}
