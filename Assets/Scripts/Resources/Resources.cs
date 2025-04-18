using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Resources : MonoBehaviourPunCallbacks
{
    [SerializeField] private ResourcesDataSO _data;
    private UnitController _currentWorker;
    private ResourcesType _type;
    private int _remainAmount;

    private void Awake()
    {
        _remainAmount = _data.MaxAmount;
        _type = _data.ResourcesType;
    }

    public UnitController CurrentWorker { get { return _currentWorker; } set { _currentWorker = value; } }
    public ResourcesType Type { get { return _type; } }
    public int RemainAmount { get { return _remainAmount; } }

    public bool IsAvailable()
    {
        return _remainAmount > 0 && _currentWorker == null;
    }

    public void AssignWorker(UnitController worker)
    {
        if(GameModManager.IsMultiplayer)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if(worker.TryGetComponent<PhotonView>(out var view))
            {
                photonView.RPC("RPCAssignWorker", RpcTarget.All, view.ViewID);
            }
        }

        else
        {
            _currentWorker = worker;
        }
    }

    [PunRPC]
    private void RPCAssignWorker(int workerID)
    {
        var view = PhotonView.Find(workerID);
        if (view != null && view.TryGetComponent(out UnitController worker))
        {
            _currentWorker = worker;
        }
    }

    public void ReduceAmount(int amount)
    {
        if(GameModManager.IsMultiplayer)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            _remainAmount -= amount;
            photonView.RPC("RPCUpdateAmount", RpcTarget.All, _remainAmount);
        }
        else
        {
            _remainAmount -= amount;
        }

        if (_remainAmount <= 0)
        {
            DestroyResources();
        }
    }

    private void DestroyResources()
    {
        if(GameModManager.IsMultiplayer)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            photonView.RPC("RPCDestroyResources", RpcTarget.All);
        }

        else
        {
            CleanWorker();
            Destroy(gameObject);
        }
    }

    [PunRPC]
    private void RPCDestroyResources()
    {
        CleanWorker();
        Destroy(gameObject);
    }

    [PunRPC]
    private void RPCUpdateAmount(int newAmount)
    {
        _remainAmount = newAmount;
    }

    private void CleanWorker()
    {
        _currentWorker = null;
    }
}
