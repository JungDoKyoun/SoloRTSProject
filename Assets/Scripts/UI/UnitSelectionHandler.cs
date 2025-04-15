using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private List<UnitController> _selectUnit = new List<UnitController>();
    private Camera _cam;
    private Vector2 _startPos;
    private bool _isDragging = false;
    private const int _maxUnitSelectCount = 24;

    private void Start()
    {
        _cam = Camera.main;
        _selectBox.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleSelectionInput();
        HandleCommand();
    }

    private void HandleSelectionInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 100f))
            {
                int layer = hit.collider.gameObject.layer;

                if(((1 << layer)& _unitLayer) != 0)
                {
                    DeselectAll();
                    var unit = hit.collider.GetComponent<UnitController>();

                    if(unit != null)
                    {
                        if(unit.IsPlayerUnit)
                        {
                            unit.IsSelect = true;
                            _selectUnit.Add(unit);
                            //유닛 설명 띄우기
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
            if(!unit.IsPlayerUnit)
            {
                continue;
            }

            Vector3 screenPos = _cam.WorldToScreenPoint(unit.transform.position);
            screenPos.y = Screen.height - screenPos.y;

            if(selectionRect.Contains(screenPos, true) && _selectUnit.Count <= _maxUnitSelectCount)
            {
                unit.IsSelect = true;
                _selectUnit.Add(unit);
            }
        }
    }

    private void DeselectAll()
    {
        foreach(var unit in _selectUnit)
        {
            unit.IsSelect = false;
        }
        _selectUnit.Clear();
    }

    private void HandleCommand()
    {
        if(Input.GetMouseButtonDown(1) && _selectUnit.Count > 0)
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray,out hit, 100f))
            {
                foreach(var unit in _selectUnit)
                {
                    if (!unit.IsPlayerUnit)
                    {
                        continue;
                    }

                    new MoveCommand(unit, hit.point, unit.UnitStateManager).Execute();
                }
            }
        }
    }
}
