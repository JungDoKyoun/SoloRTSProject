using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BuildGhostPlacer : MonoBehaviour
{
    private static BuildGhostPlacer _instance;
    [SerializeField] private Material _validMat, _invalidMat;
    [SerializeField] private LayerMask _groundLayer;
    private GameObject _ghost;
    private Renderer[] _renderers;
    private BuildingBlueprintDataSO _data;
    private UnitController _unit;
    private Player _player;
    private Building _building;

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

    private void Update()
    {
        UpdateGhost();
    }

    public static BuildGhostPlacer Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<BuildGhostPlacer>();
            }
            return _instance;
        }
    }

    public void StartPlacing(BuildingBlueprintDataSO data, UnitController unit)
    {
        _data = data;

        if(_ghost != null)
        {
            Destroy(_ghost);
        }

        _unit = unit;
        if(_unit == null)
        {
            return;
        }

        _player = _unit.Player;

        //if(!_player.IsenoughResources(_data.ResourceCosts))
        //{
        //    return;
        //}

        _ghost = Instantiate(data.PreviewPrefab);
        _renderers = _ghost.GetComponentsInChildren<Renderer>();
    }

    public bool IsValidPosition(Vector3 pos)
    {
        if(_ghost == null)
        {
            return false;
        }

        _building = _ghost.GetComponent<Building>();
        var buildSize = Utils.GetBuildSize(_ghost);
        Vector3 half = buildSize * 0.5f;

        if(Physics.OverlapBox(pos, half, Quaternion.identity, LayerMask.GetMask("Unit", "Building", "Resources")).Length > 0)
        {
            return false;
        }

        if(!Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, 1f, _groundLayer))
        {
            return false;
        }

        Vector3[] checkPoints = new Vector3[]
        {
            pos + new Vector3(half.x, 0, half.z),
            pos + new Vector3(half.x, 0, -half.z),
            pos + new Vector3(-half.x, 0, half.z),
            pos + new Vector3(-half.x, 0, -half.z)
        };

        foreach (var point in checkPoints)
        {
            if (Physics.Raycast(point + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 1f, _groundLayer))
            {
                if (Mathf.Abs(point.y - hit.point.y) > 0.2)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    private void SetGhostColor(Material mat)
    {
        foreach(var r in _renderers)
        {
            r.material = mat;
        }
    }

    private void StartBuild(Vector3 des)
    {
        //_player.UseResources(_data.ResourceCosts);
        new BuildCommand(_unit, des, _building, _data).Execute();
    }

    public void UpdateGhost()
    {
        if(_ghost == null)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            _ghost.transform.position = hit.point;
            bool isValid = IsValidPosition(hit.point);

            if(!isValid)
            {
                SetGhostColor(_invalidMat);
                return;
            }

            SetGhostColor(_validMat);

            if(Input.GetMouseButtonDown(0))
            {
                if(GameModManager.IsMultiplayer)
                {
                    if(!_unit.IsPlayerUnit(_player))
                    {
                        return;
                    }

                    _unit.photonView.RPC("RPCRequestBuild", Photon.Pun.RpcTarget.All, hit.point, _data.Name, _player.PlayerID, _unit.photonView.ViewID);
                    _ghost = null;
                }
                else
                {
                    if(_building == null)
                    {
                        _building = _ghost.GetComponent<Building>();
                    }
                    
                    _building.Init(_data, _player.PlayerID);
                    MeshRenderer meshRenderer = _building.GetComponent<MeshRenderer>();
                    if(meshRenderer != null)
                    {
                        BoxCollider boxCollider = _building.AddComponent<BoxCollider>();
                        boxCollider.center = meshRenderer.bounds.center - _building.transform.position;
                        boxCollider.size = meshRenderer.bounds.size;
                    }
                    _building.AddComponent<NavMeshObstacle>();
                    var nav = _building.GetComponent<NavMeshObstacle>();
                    nav.carving = true;
                    StartBuild(hit.point);
                    _ghost = null;
                }
            }
        }
    }
}
