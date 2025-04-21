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
                    Debug.Log(unit);

                    if(unit != null)
                    {
                        if(unit.IsPlayerUnit(_player))
                        {
                            SelectUnit(unit);
                        }
                        else
                        {
                            unit.IsSelect = true;
                            //나중에 여기에 선택한 유닛 설명 띄우기
                        }
                    }
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
        _selectedUnit.Add(unit);
        _commandPanelController.UpdateForUnits(_selectedUnit);
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
        _commandPanelController.UpdateForUnit(null);
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

                if(((1 << layer)& _enemy) != 0)
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
