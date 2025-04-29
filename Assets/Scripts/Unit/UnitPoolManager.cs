using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class UnitPoolManager : MonoBehaviourPunCallbacks
{
    private static UnitPoolManager _instance;
    private Dictionary<RaceType, List<UnitDataSO>> _allUnitData = new Dictionary<RaceType, List<UnitDataSO>>();
    private Dictionary<RaceType, Dictionary<string, ObjectPool<UnitController>>> _raceUnitPools = new Dictionary<RaceType, Dictionary<string, ObjectPool<UnitController>>>();
    private Dictionary<string, UnitController> _activeUnit = new Dictionary<string, UnitController>();

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            LoadAllUnitData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static UnitPoolManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<UnitPoolManager>();
            }
            return _instance;
        }
    }

    private void LoadAllUnitData()
    {
        foreach(RaceType race in System.Enum.GetValues(typeof(RaceType)))
        {
            var dataList = new List<UnitDataSO>(Resources.LoadAll<UnitDataSO>($"UnitData/{race}"));
            _allUnitData[race] = dataList;
        }
    }

    private ObjectPool<UnitController> CreatePool(RaceType race, UnitDataSO unitData)
    {
        if(!_raceUnitPools.ContainsKey(race))
        {
            _raceUnitPools[race] = new Dictionary<string, ObjectPool<UnitController>>();
        }

        var raceUnitPools = _raceUnitPools[race];
        if (raceUnitPools.ContainsKey(unitData.Name))
        {
            return raceUnitPools[unitData.Name];
        }

        var pool = new ObjectPool<UnitController>(() =>
        {
            var obj = Instantiate(unitData.Prefab).GetComponent<UnitController>();
            obj.gameObject.SetActive(false);
            return obj;
        },
        obj => obj.gameObject.SetActive(true),
        obj => obj.gameObject.SetActive(false),
        obj => Destroy(obj.gameObject),
        true);

        raceUnitPools[unitData.Name] = pool;
        return pool;
    }

    public UnitController GetUnit(RaceType race, UnitDataSO unitData, int playerID, Vector3 pos, string unitInstanceID = null)
    {
        var pool = CreatePool(race, unitData);
        var unit = pool.Get();

        var manger = unit.GetComponent<UnitManager>();
        unit.Init(manger, playerID, unitInstanceID);

        unit.transform.position = pos;

        _activeUnit[unit.UnitInstanceID] = unit;

        return unit;
    }

    public void MultiGetUnit(RaceType race, UnitDataSO unitData, int playerID, Vector3 pos)
    {
        if (GameModManager.IsMultiplayer)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            string unitInstanceID = System.Guid.NewGuid().ToString();
            photonView.RPC("RPCGetUnit", RpcTarget.All, (int)race, unitData.Name, playerID, pos, unitInstanceID);
        }
    }

    [PunRPC]
    private void RPCGetUnit(int raceInt, string dataName, int playerID, Vector3 pos, string unitInstanceID)
    {
        RaceType race = (RaceType)raceInt;
        var dataList = _allUnitData[race];
        var data = dataList.Find(u => u.Name == dataName);

        GetUnit(race, data, playerID, pos, unitInstanceID);
    }

    public void ReleaseUnit(UnitController unit)
    {
        if (_activeUnit.TryGetValue(unit.UnitInstanceID, out var targetUnit))
        {
            var pool = CreatePool(targetUnit.Player.RaceType, targetUnit.UnitData);
            pool.Release(targetUnit);
            _activeUnit.Remove(unit.UnitInstanceID);
        }
    }

    public void MultiReleaseUnit(UnitController unit)
    {
        if(GameModManager.IsMultiplayer)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            string unitInstanceID = unit.UnitInstanceID;
            photonView.RPC("RPCReleaseUnit", RpcTarget.All, unitInstanceID);
        }
    }

    [PunRPC]
    private void RPCReleaseUnit(string unitID)
    {
        if (_activeUnit.TryGetValue(unitID, out var unit))
        {
            ReleaseUnit(unit);
        }
    }
}
