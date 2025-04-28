using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CommandPanelController : MonoBehaviour
{
    [SerializeField] private CommadButton[] _buttons;
    [SerializeField] private Sprite _attackIcon, _buildIcon, _cancleIcon;

    public void ClearAll()
    {
        foreach (var button in _buttons)
        {
            button.Clear();
        }
    }

    public void ShowUnitUI(UnitController unit)
    {
        ClearAll();
        Debug.Log((unit.IsWorker()));
        if(unit == null)
        {
            return;
        }

        if (unit.IsWorker())
        {
            Debug.Log("bb");
            _buttons[8].SetCommandButton(_buildIcon, () => ShowBuildUI(unit));
        }

        //공용
    }

    public void ShowUnitsUI(List<UnitController> units)
    {
        ClearAll();

        if (units == null || units.Count == 0)
        {
            return;
        }

        if (units.Count == 1)
        {
            ShowUnitUI(units[0]);
            return;
        }

        //여기에 공용 UI업데이트
    }

    public void ShowBuildUI(UnitController unit)
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

    public void ShowIncompleteBuildingUI(Building building)
    {
        ClearAll();

        _buttons[11].SetCommandButton(_cancleIcon, () => building.CancelConstruct());
    }

    public void ShowCompleteBuildingUI(Building building)
    {
        ClearAll();

        if(building is IUnitProducer)
        {
            ShowProductionUI(building);
        }
    }

    public void ShowProductionUI(Building building)
    {
        ClearAll();

        if(building == null)
        {
            return;
        }

        var unitProductionList = building.GetBuildingData().TrainableUnits;

        for(int i = 0; i < _buttons.Length; i++)
        {
            if(i < unitProductionList.Count)
            {
                int index = i;
                var data = unitProductionList[index];
                _buttons[i].SetCommandButton(data.UnitIcon, () =>
                {
                    if(building is IUnitProducer producer)
                    {
                        producer.ProduceUnit(data);
                    }
                });
            }
        }
    }
}
