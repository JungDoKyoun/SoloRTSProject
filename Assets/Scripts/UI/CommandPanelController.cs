using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPanelController : MonoBehaviour
{
    [SerializeField] private CommadButton[] _buttons;
    [SerializeField] private Sprite _attackIcon, _buildIcon;
    [SerializeField] private BuildPanelController _buildPanelController;

    public void ClearAll()
    {
        foreach (var button in _buttons)
        {
            button.Clear();
        }
    }

    public void UpdateForUnit(UnitController unit)
    {
        ClearAll();

        if(unit == null)
        {
            return;
        }

        if (unit.UnitType == UnitType.Normal)
        {

        }
        else if (unit.IsWorker())
        {
            _buttons[8].SetCommandButton(_buildIcon, () => ShowBuild(unit));
        }
    }

    public void UpdateForUnits(List<UnitController> units)
    {
        ClearAll();

        if (units == null || units.Count == 0)
        {
            return;
        }

        if (units.Count == 1)
        {
            UpdateForUnit(units[0]);
            return;
        }

        //여기에 공용 UI업데이트
    }

    public void ShowBuild(UnitController unit)
    {
        ClearAll();

        if (unit == null)
        {
            return;
        }

        var buildList = unit.UnitData.BuildingBlueprintDatas;

        for(int i = 0; i < _buttons.Length; i++)
        {
            if(i < buildList.Count)
            {
                int index = i;
                var data = buildList[index];
                _buttons[i].SetCommandButton(buildList[i].Icon, () => BuildGhostPlacer.Instance.StartPlacing(data, unit));
            }

            else
            {
                _buttons[i].Clear();
            }
        }
    }
}
