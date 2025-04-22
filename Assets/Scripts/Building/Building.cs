using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviourPunCallbacks
{
    protected BuildingDataSO _data;
    protected Player _player;
    protected BuildingBlueprintDataSO _buildData;
    protected int _currentHP;
    protected float _elapseTime;
    protected bool _isComplete;
    protected bool _isSelected = false;

    public Player Player { get { return _player; } }
    public int CurrentHP { get { return _currentHP; } }
    public bool IsComplete { get { return _isComplete; } }
    public bool IsSelected { get { return _isSelected; } set { _isSelected = value; } }

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

    public void Construct(float delta)
    {
        _elapseTime += delta;

        if(_elapseTime >= _buildData.BuildTime)
        {
            _isComplete = true;
        }
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

    public bool IsMyBuilding(Player player)
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
}
