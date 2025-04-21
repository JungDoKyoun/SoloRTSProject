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

        if (unit.IsWorker())
        {
            _buttons[8].SetCommandButton(_buildIcon, () => CommandUIManager.Instance.ShowBuildPanel());
            _buildPanelController.Setup(unit.UnitData);
        }
    }
}
