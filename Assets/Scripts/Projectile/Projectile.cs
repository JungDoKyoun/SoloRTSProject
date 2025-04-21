using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviourPunCallbacks
{
    private ProjectileDataSO _data;
    private UnitController _target;
    private string _id;

    public ProjectileDataSO Data { get { return _data; } }

    public void Init(ProjectileDataSO data, UnitController target, string id)
    {
        _data = data;
        _target = target;
        _id = id;
    }

    private void Update()
    {
        if(_target == null || _target.IsDie)
        {
            Release();
            return;
        }

        Shoot();
    }

    private void Shoot()
    {
        Vector3 targetPos = _target.transform.position;
        targetPos.y = transform.position.y;
        Vector3 dir = (targetPos - transform.position).normalized;
        transform.position += dir * _data.Speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir);

        if(Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            Hit();
        }
    }

    private void Hit()
    {
        if(GameModManager.IsMultiplayer)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                var targetID = _target.GetComponent<PhotonView>().ViewID;
                photonView.RPC("RPCTakeDamage", RpcTarget.MasterClient, _data.Damage, targetID, _id);
            }
        }
        else
        {
            if(_target != null && !_target.IsDie)
            {
                _target.TakeDamage(_data.Damage);
            }
            Release();
        }
    }

    [PunRPC]
    private void RPCTakeDamage(int damage, int targetID, string id)
    {
        var target = PhotonView.Find(targetID).GetComponent<UnitController>();

        if(target != null && !target.IsDie)
        {
            target.TakeDamage(damage);
        }

        Release();
    }

    private void Release()
    {
        if (GameModManager.IsMultiplayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPCRelease", RpcTarget.All, _id);
            }
        }
        else
        {
            ProjectileSpawner.Instance.ReleaseLocal(_id);
        }
    }

    [PunRPC]
    private void RPCRelease(string id)
    {
        ProjectileSpawner.Instance.ReleaseMultiplayer(id);
    }
}
