using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPanelController : MonoBehaviour
{
    [SerializeField] private CommadButton[] _buttons;
    [SerializeField] private Sprite _attackIcon, _buildIcon;
    [SerializeField] private BuildPanelController _buildPanelController;

    public void UpdateForUnit(UnitController unit)
    {
        foreach(var button in _buttons)
        {
            button.Clear();
        }

        if(unit == null)
        {
            return;
        }

        if (unit.UnitType == UnitType.Normal)
        {

        }
        else if (unit.IsWorker())
        {
            _buttons[8].SetCommandButton(_buildIcon, () => CommandUIManager.Instance.ShowBuildPanel());
            _buildPanelController.Setup(unit.UnitData);
        }
    }

    public void UpdateForUnits(List<UnitController> units)
    {
        foreach (var button in _buttons)
        {
            button.Clear();
        }

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
}
