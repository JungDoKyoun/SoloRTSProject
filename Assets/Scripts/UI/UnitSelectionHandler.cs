using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class UnitSelectionHandler : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] RectTransform _selectBox;

    [Header("레이어")]
    [SerializeField] private LayerMask _groundLayer;

    private static UnitSelectionHandler _instance;
    private List<UnitController> _selectedUnit = new List<UnitController>();
    private Camera _cam;
    private Player _player;
    private Building _selectedBuilding;
    [SerializeField] private CommandPanelController _commandPanelController;
    private Vector2 _startPos;
    private const int _maxUnitSelectCount = 24;
    private bool _isDragging = false;
    private bool _isAttackMove = false;

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

    private void Start()
    {
        _cam = Camera.main;
        _player = PlayerManager.Instance.LocalPlayer;
        _selectBox.gameObject.SetActive(false);
    }

    private void Update()
    {
        PushButton();
        HandleSelectionInput();
        HandleCommand();
    }

    public static UnitSelectionHandler Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<UnitSelectionHandler>();
            }
            return _instance;
        }
    }

    public List<UnitController> SelectedUnit { get { return _selectedUnit; } }

    private void PushButton()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            _isAttackMove = true;
        }
    }

    private void HandleSelectionInput()
    {
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 100f))
            {
                var targetUnit = hit.collider.GetComponent<UnitController>();
                var targetBuilding = hit.collider.GetComponent<Building>();
                var targetResource = hit.collider.GetComponent<Resource>();

                if (_isAttackMove && targetUnit != null && _selectedUnit[0].IsEnemy(targetUnit) && _selectedUnit.Count > 0)
                {
                    foreach (var unit in _selectedUnit)
                    {
                        if (unit.IsPlayerUnit(_player))
                        {
                            new AttackCommand(unit, targetUnit).Execute();
                        }
                    }
                    _isAttackMove = false;
                    return;
                }

                if(_isAttackMove && targetBuilding != null && _selectedUnit.Count > 0 && _selectedUnit[0].IsEnemy(targetBuilding))
                {
                    foreach(var unit in _selectedUnit)
                    {
                        if(unit.IsPlayerUnit(_player))
                        {
                            new AttackCommand(unit, targetBuilding);
                        }
                    }
                }

                if(_isAttackMove && ((1 << hit.collider.gameObject.layer)& _groundLayer) != 0)
                {
                    Vector3 detination = hit.point;

                    foreach(var unit in _selectedUnit)
                    {
                        if (unit.IsPlayerUnit(_player))
                        {
                            new MoveAttackCommand(unit, detination).Execute();
                        }
                    }
                    _isAttackMove = false;
                    return;
                }

                if (targetUnit != null)
                {
                    SelectUnit(targetUnit);
                    return;
                }

                if (targetBuilding != null)
                {
                    SelectBuilding(targetBuilding);

                    if(targetBuilding.IsPlayerBuilding(_player))
                    return;
                }

                _isDragging = true;
                _startPos = Input.mousePosition;
                _selectBox.gameObject.SetActive(true);
            }
        }

        if(Input.GetMouseButton(0) && _isDragging)
        {
            UpdateSelectBox(_startPos, Input.mousePosition);
        }

        if(Input.GetMouseButtonUp(0) && _isDragging)
        {
            _isDragging = false;
            _selectBox.gameObject.SetActive(false);
            SelectUnitInBox(_startPos, Input.mousePosition);
        }
    }

    private void SelectUnit(UnitController unit)
    {
        DeselectAll();

        unit.IsSelect = true;

        if(unit.IsPlayerUnit(_player))
        {
            _selectedUnit.Add(unit);
            _commandPanelController.ShowUnitUI(unit);
        }
        else
        {
            _commandPanelController.ClearAll();
        }

        //중앙패널 유닛 정보
    }

    private void UpdateSelectBox(Vector2 startPos, Vector2 mousePos)
    {
        Vector2 size = mousePos - startPos;
        _selectBox.pivot = new Vector2
            (size.x >= 0 ? 0 : 1,
            size.y >= 0 ? 0 : 1);
        _selectBox.anchoredPosition = startPos;
        _selectBox.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
    }

    private void SelectUnitInBox(Vector2 startPos, Vector2 mousePos)
    {
        DeselectAll();

        Rect selectionRect = Utils.GetScreenRect(startPos, mousePos);

        List<UnitController> playerUnits = new List<UnitController>();
        List<UnitController> otherUnits = new List<UnitController>();

        foreach(var unit in UnitRegistry.Instance.AllUnits)
        {
            Vector3 screenPos = _cam.WorldToScreenPoint(unit.transform.position);
            screenPos.y = Screen.height - screenPos.y;

            if(selectionRect.Contains(screenPos, true))
            {
                if(unit.IsPlayerUnit(_player))
                {
                    playerUnits.Add(unit);
                }
                else
                {
                    otherUnits.Add(unit);
                }
            }
        }

        if(playerUnits.Count == 1)
        {
            SelectUnit(playerUnits[0]);
            return;
        }

        if(playerUnits.Count > 1)
        {
            foreach(var unit in playerUnits)
            {
                if(_selectedUnit.Count <= _maxUnitSelectCount)
                {
                    unit.IsSelect = true;
                    _selectedUnit.Add(unit);
                }
            }
            _commandPanelController.ShowUnitsUI(playerUnits);
            //센터 패널
            return;
        }
        if(playerUnits.Count == 0 && otherUnits.Count > 0)
        {
            SelectUnit(otherUnits[0]);
            return;
        }

        if(_selectedUnit.Count == 0)
        {
            foreach(var building in BuildingRegistry.Instance.AllBuildings)
            {
                if(!building.IsPlayerBuilding(_player))
                {
                    continue;
                }

                Vector3 screenPos = _cam.WorldToScreenPoint(building.transform.position);
                screenPos.y = Screen.height - screenPos.y;

                if(selectionRect.Contains(screenPos, true))
                {
                    SelectBuilding(building);
                    break;
                }
            }
        }
        else
        {
            _commandPanelController.ShowUnitsUI(_selectedUnit);
            //중앙패널 유닛정보
        }
    }

    private void DeselectAll()
    {
        foreach(var unit in _selectedUnit)
        {
            unit.IsSelect = false;
        }

        _selectedUnit.Clear();

        if(_selectedBuilding != null)
        {
            _selectedBuilding.IsSelected = false;
            _selectedBuilding = null;
        }

        _commandPanelController.ClearAll();

        //센터패널도 초기화
    }

    private void SelectBuilding(Building building)
    {
        DeselectAll();

        _selectedBuilding = building;
        building.IsSelected = true;

        if(building.IsPlayerBuilding(_player))
        {
            if (building.IsComplete)
            {
                _commandPanelController.ShowCompleteBuildingUI(building);
            }
            else if (!building.IsComplete)
            {
                _commandPanelController.ShowIncompleteBuildingUI(building);
            }
        }
        //센터패널 
    }

    private void HandleCommand()
    {
        if(Input.GetMouseButtonDown(1) && _selectedUnit.Count > 0)
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray,out hit, 100f))
            {
                var targetUnit = hit.collider.GetComponent<UnitController>();
                var targetBuilding = hit.collider.GetComponent<Building>();
                var targetResource = hit.collider.GetComponent<Resource>();

                if (targetUnit != null && _selectedUnit.Count > 0 && _selectedUnit[0].IsEnemy(targetUnit))
                {
                    foreach(var unit in _selectedUnit)
                    {
                        if(unit.IsPlayerUnit(_player))
                        {
                            new AttackCommand(unit, targetUnit).Execute();
                        }
                    }
                    return;
                }

                if(targetBuilding != null && _selectedUnit.Count > 0&& _selectedUnit[0].IsEnemy(targetBuilding))
                {
                    foreach(var unit in _selectedUnit)
                    {
                        if(unit.IsPlayerUnit(_player))
                        {
                            new AttackCommand(unit, targetBuilding).Execute();
                        }
                    }
                    return;
                }

                if(targetResource != null)
                {
                    Vector3 destination = targetResource.transform.position;

                    foreach (var unit in _selectedUnit)
                    {
                        if(unit.IsPlayerUnit(_player) && unit.IsWorker())
                        {
                            new GatherCommand(unit, targetResource, destination).Execute();
                        }
                    }
                    return;
                }

                if(targetUnit != null && _selectedUnit.Count > 0 && targetBuilding.IsPlayerBuilding(_player))
                {
                    var unit = _selectedUnit[0];
                    var data = targetBuilding.GetBuildData();
                    if(unit.IsPlayerUnit(_player) && unit.IsWorker())
                    {
                        if (GameModManager.IsMultiplayer)
                        {
                            if(PhotonNetwork.IsMasterClient)
                            {
                                new BuildCommand(unit, targetBuilding.gameObject.transform.position, targetBuilding, data).Execute();
                            }
                            else
                            {
                                unit.photonView.RPC("RPCRequestResumeBuild", RpcTarget.MasterClient, targetBuilding.photonView.ViewID, unit.photonView.ViewID);
                            }
                        }
                        else
                        {
                            new BuildCommand(unit, targetBuilding.gameObject.transform.position, targetBuilding, data).Execute();
                        }
                    }
                    return;
                }

                foreach (var unit in _selectedUnit)
                {
                    if (unit.IsPlayerUnit(_player))
                    {
                        new MoveCommand(unit, hit.point).Execute();
                    }
                }
            }
        }
    }
}
