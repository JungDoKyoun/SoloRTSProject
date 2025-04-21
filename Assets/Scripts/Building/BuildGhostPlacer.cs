using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildGhostPlacer : MonoBehaviour
{
    private static BuildGhostPlacer _instance;
    [SerializeField] private Material _validMat, _invalidMat;
    [SerializeField] private LayerMask groundLayer;
    private GameObject _ghost;
    private Renderer[] _renderers;
    private BuildingBlueprintDataSO _data;
    private UnitController _unit;
    private Player _player;

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

    public void StartPlacing(BuildingBlueprintDataSO data)
    {
        _data = data;

        if(_ghost != null)
        {
            Destroy(_ghost);
        }

        _unit = UnitSelectionHandler.Instance.SelectedUnit.Find(u => u.IsWorker());
        if(_unit == null)
        {
            return;
        }

        _player = _unit.Player;
        if(!_player.IsenoughResources(_data.ResourceCosts))
        {
            return;
        }

        _ghost = Instantiate(data.PreviewPrefab);
        _renderers = _ghost.GetComponentsInChildren<Renderer>();
    }

    public bool IsValidPosition(Vector3 pos)
    {
        Vector3 half = _data.BuildSize * 0.5f;

        if(Physics.OverlapBox(pos, half, Quaternion.identity, LayerMask.GetMask("Unit", "Building", "Resources", "Enemy")).Length > 0)
        {
            return false;
        }

        if(!Physics.Raycast(pos + Vector3.up * 5f, Vector3.down, 10f, groundLayer))
        {
            return false;
        }

        if(Physics.OverlapBox(pos, half, Quaternion.identity, groundLayer).Length < 1)
        {
            return false;
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
        Destroy(_ghost);
        _player.UseResources(_data.ResourceCosts);
        new BuildCommand(_unit, des, _data).Execute();
    }

    public void UpdateGhost()
    {
        if(_ghost == null)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            bool isValid = IsValidPosition(hit.point);

            if(!isValid)
            {
                SetGhostColor(_invalidMat);
                return;
            }

            SetGhostColor(_validMat);

            if(Input.GetMouseButtonDown(0))
            {
                StartBuild(hit.point);
            }
        }
    }
}
