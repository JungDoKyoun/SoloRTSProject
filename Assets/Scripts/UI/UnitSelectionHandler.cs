using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelectionHandler : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] RectTransform _selectBox;

    [Header("레이어")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _unitLayer;
    [SerializeField] private LayerMask _buildingLayer;
    [SerializeField] private LayerMask _resources;
    [SerializeField] private LayerMask _enemy;

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
                int layer = hit.collider.gameObject.layer;

                if(_isAttackMove && ((1 << layer)& _enemy) != 0)
                {
                    var target = hit.collider.GetComponent<UnitController>();
                    if(target != null)
                    {
                        foreach(var unit in _selectedUnit)
                        {
                            if(unit.IsPlayerUnit(_player))
                            {
                                new AttackCommand(unit, target).Execute();
                            }
                        }
                        _isAttackMove = false;
                        return;
                    }
                }

                if(_isAttackMove && ((1 << layer)& _groundLayer) != 0)
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

                if(((1 << layer)& _unitLayer) != 0)
                {
                    DeselectAll();
                    var unit = hit.collider.GetComponent<UnitController>();

                    if(unit != null)
                    {
                        SelectUnit(unit);
                    }
                    return;
                }

                if(((1 << layer)& _buildingLayer) != 0)
                {
                    DeselectAll();
                    var building = hit.collider.GetComponent<Building>();

                    if(building == null)
                    {
                        return;
                    }

                    SelectBuilding(building);

                    return;
                }

                DeselectAll();
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
        unit.IsSelect = true;

        if(unit.IsPlayerUnit(_player))
        {
            _selectedUnit.Add(unit);
            _commandPanelController.UpdateForUnits(_selectedUnit);
        }

        //센터패널
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

        foreach(var unit in UnitRegistry.Instance.AllUnits)
        {
            if(!unit.IsPlayerUnit(_player))
            {
                continue;
            }

            Vector3 screenPos = _cam.WorldToScreenPoint(unit.transform.position);
            screenPos.y = Screen.height - screenPos.y;

            if(selectionRect.Contains(screenPos, true) && _selectedUnit.Count <= _maxUnitSelectCount)
            {
                SelectUnit(unit);
            }
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

        _commandPanelController.UpdateForUnit(null);

        //센터패널도 초기화
    }

    private void SelectBuilding(Building building)
    {
        _selectedBuilding = building;
        building.IsSelected = true;

        if(building.IsMyBuilding(_player))
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
                int layer = hit.collider.gameObject.layer;
                Debug.Log("클릭한 레이어: " + layer);

                if (((1 << layer)& _enemy) != 0)
                {
                    var target = hit.collider.GetComponent<UnitController>();

                    foreach(var unit in _selectedUnit)
                    {
                        if(unit.IsPlayerUnit(_player))
                        {
                            new AttackCommand(unit, target).Execute();
                        }
                    }
                    return;
                }

                if(((1 << layer)& _resources) != 0)
                {
                    var target = hit.collider.GetComponent<Resource>();

                    foreach(var unit in _selectedUnit)
                    {
                        if(unit.IsPlayerUnit(_player) && unit.IsWorker())
                        {
                            new GatherCommand(unit, target).Execute();
                        }
                    }
                }

                if(((1 <<layer)& _buildingLayer) != 0)
                {
                    Debug.Log("건물 클릭 감지!");
                    var building = hit.collider.GetComponent<Building>();

                    if(!building.IsMyBuilding(_player))
                    {
                        Debug.Log("내 건물이 아님!");
                        return;
                    }

                    var unit = _selectedUnit[0];
                    var data = building.GetBuildData();
                    if(unit.IsPlayerUnit(_player) && unit.IsWorker())
                    {
                        Debug.Log("내 일꾼이 클릭됨, 건물 이어서 지어야 함!");
                        if (GameModManager.IsMultiplayer)
                        {
                            if(PhotonNetwork.IsMasterClient)
                            {
                                Debug.Log("마스터 클라에서 BuildCommand 실행");
                                new BuildCommand(unit, building.gameObject.transform.position, building, data).Execute();
                            }
                            else
                            {
                                Debug.Log("RPC로 마스터에게 요청 보냄");
                                unit.photonView.RPC("RPCRequestResumeBuild", RpcTarget.MasterClient, building.photonView.ViewID, unit.photonView.ViewID);
                            }
                        }
                        else
                        {
                            Debug.Log("싱글플레이에서 BuildCommand 실행");
                            new BuildCommand(unit, building.gameObject.transform.position, building, data).Execute();
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
