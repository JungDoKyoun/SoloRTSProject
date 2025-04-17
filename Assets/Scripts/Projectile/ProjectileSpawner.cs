using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<ProjectileDataSO> _data = new List<ProjectileDataSO>();
    private static ProjectileSpawner _instance;
    private Dictionary<string, ObjectPool<Projectile>> _projectilePool = new Dictionary<string, ObjectPool<Projectile>>();
    private Dictionary<string, Projectile> _activeProjectile = new Dictionary<string, Projectile>();

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static ProjectileSpawner Instance { get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<ProjectileSpawner>();
            }
            return _instance;
        }
    }

    private ObjectPool<Projectile> CreatePool(ProjectileDataSO data)
    {
        if(_projectilePool.TryGetValue(data.ProjectileName, out var pool))
        {
            return pool;
        }

        ObjectPool<Projectile> newPool = new ObjectPool<Projectile>(
            () => Instantiate(data.ProjectilePrefab).GetComponent<Projectile>(),
            obj => obj.gameObject.SetActive(true),
            obj => obj.gameObject.SetActive(false),
            obj => Destroy(obj.gameObject),
            defaultCapacity : 20);

        _projectilePool.Add(data.ProjectileName, newPool);
        return newPool;
    }

    public void LaunchLocal(ProjectileDataSO data, Transform firePoin, UnitController target)
    {
        string id = Guid.NewGuid().ToString();
        var pool = CreatePool(data);
        var obj = pool.Get();

        obj.transform.position = firePoin.position;
        obj.transform.rotation = Quaternion.LookRotation(target.transform.position - firePoin.position);
        obj.Init(data, target, id);

        _activeProjectile.Add(id, obj);
    }

    public void LaunchMultiplayer(ProjectileDataSO data, Transform firePoin, UnitController target)
    {
        string id = Guid.NewGuid().ToString();
        LaunchLocal(data, firePoin, target);

        if(target.TryGetComponent<PhotonView>(out var view))
        {
            photonView.RPC("RPCLaunch", RpcTarget.Others, data.ProjectileName, firePoin.position, view.ViewID, id);
        }
    }

    [PunRPC]
    private void RPCLaunch(string dataName, Vector3 firePoin, int targetId, string id)
    {
        var data = _data.Find(p => p.ProjectileName == dataName);
        if(data == null)
        {
            return;
        }
        var target = PhotonView.Find(targetId)?.GetComponent<UnitController>();
        if(target == null)
        {
            return;
        }

        var pool = CreatePool(data);
        var obj = pool.Get();

        obj.transform.position = firePoin;
        obj.transform.rotation = Quaternion.LookRotation(target.transform.position - firePoin);
        obj.Init(data, target, id);

        _activeProjectile.Add(id, obj);
    }

    public void ReleaseLocal(string id)
    {
        if(_activeProjectile.TryGetValue(id, out var obj))
        {
            string name = obj.Data.ProjectileName;

            if (_projectilePool.TryGetValue(name, out var pool))
            {
                pool.Release(obj);
            }

            _activeProjectile.Remove(id);
        }
    }

    public void ReleaseMultiplayer(string id)
    {
        ReleaseLocal(id);

        photonView.RPC("RPCRelease", RpcTarget.Others, id);
    }

    [PunRPC]
    private void RPCRelease(string id)
    {
        ReleaseLocal(id);
    }
}
