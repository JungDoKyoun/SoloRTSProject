using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviourPunCallbacks, IAttackable
{
    protected BuildingDataSO _data;
    protected Player _player;
    protected BuildingBlueprintDataSO _buildData;
    protected int _currentHP;
    protected float _elapseTime;
    protected bool _isComplete;
    protected bool _isSelected = false;
    protected bool _isDestroy = false;

    public Player Player { get { return _player; } }
    public int CurrentHP { get { return _currentHP; } }
    public bool IsComplete { get { return _isComplete; } set { _isComplete = value; } }
    public bool IsSelected { get { return _isSelected; } set { _isSelected = value; } }
    public bool IsDestroyed { get { return _isDestroy; } set { _isDestroy = value; } }
    public Vector3 Position => transform.position;

    private void Start()
    {
        BuildingRegistry.Instance.Register(this);
    }

    private new void OnDisable()
    {
        BuildingRegistry.Instance.UnRegister(this);
    }

    public void Init(BuildingBlueprintDataSO data, Player player)
    {
        _buildData = data;
        _data = data.buildingData;
        _player = player;
        _currentHP = _data.MaxHP;
        _isComplete = false;
    }

    public void Init(BuildingBlueprintDataSO data, Player player, int hp)
    {
        _buildData = data;
        _data = data.buildingData;
        _player = player;
        _currentHP = hp;
        _isComplete = true;
    }

    public void CompleteConstruction(Vector3 pos)
    {
        if (GameModManager.IsMultiplayer)
        {
            photonView.RPC("RPCCompleteConstruction", RpcTarget.All, pos, _buildData.Name, _player.PlayerID);
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            var buildObj = Instantiate(_buildData.BuildingPrefab, pos, _buildData.BuildingPrefab.transform.rotation);
            var building = buildObj.GetComponent<Building>();
            building.Init(_buildData ,_player, _currentHP);
            Destroy(gameObject);
        }
    }

    [PunRPC]
    public void RPCCompleteConstruction(Vector3 pos, string dataName, int playerID)
    {
        var buildData = Resources.Load<BuildingBlueprintDataSO>("BuildingGhostData/" + dataName);
        var player = PlayerManager.Instance.GetPlayer(playerID);

        if(player == null || buildData == null)
        {
            return;
        }

        var buildObj = PhotonNetwork.Instantiate(buildData.BuildingName, pos, buildData.BuildingPrefab.transform.rotation);
        var building = buildObj.GetComponent<Building>();
        building.Init(buildData, player, _currentHP);
    }

    public bool IsPlayerBuilding(Player player)
    {
        return player == _player;
    }
    
    public BuildingDataSO GetBuildingData()
    {
        return _data;
    }

    public BuildingBlueprintDataSO GetBuildData()
    {
        return _buildData;
    }

    public void CancelConstruct()
    {
        if (GameModManager.IsMultiplayer)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            _player.AddResources(_buildData.ResourceCosts);
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            _player.AddResources(_buildData.ResourceCosts);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        if(GameModManager.IsMultiplayer && !PhotonNetwork.IsMasterClient)
        {
            return;
        }

        _currentHP -= Mathf.Max(1, damage - _data.Defend);

        if(GameModManager.IsMultiplayer)
        {
            photonView.RPC("RPCSyncHP", RpcTarget.All, _currentHP);
        }

        if(_currentHP <= 0)
        {
            _currentHP = 0;

            if(GameModManager.IsMultiplayer)
            {
                photonView.RPC("RPCDestroyBuilding", RpcTarget.All);
            }
            else
            {
                DestroyBuilding();
            }
        }
    }

    [PunRPC]
    public void RPCSyncHP(int hp)
    {
        _currentHP = hp;
    }

    public void DestroyBuilding()
    {
        _isDestroy = true;
        Destroy(gameObject);
    }

    [PunRPC]
    public void RPCDestroyBuilding()
    {
        _isDestroy = true;
        PhotonNetwork.Destroy(gameObject);
    }

    public bool IsEnemy(Player player)
    {
        if(_player.TeamType != player.TeamType)
        {
            return true;
        }
        return false;
    }
}
