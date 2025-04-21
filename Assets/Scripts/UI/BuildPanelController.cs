using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPanelController : MonoBehaviour
{
    [SerializeField] private BuildButton[] _buildButtons;

    public void Setup(UnitDataSO unitData)
    {
        var buildings = unitData.BuildingBlueprintDatas;

        for(int i = 0; i < _buildButtons.Length; i++)
        {
            if(i < buildings.Count)
            {
                _buildButtons[i].SetUp(buildings[i]);
            }
            else
            {
                _buildButtons[i].Clear();
            }
        }
    }
}
