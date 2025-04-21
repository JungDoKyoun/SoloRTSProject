using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;
    private BuildingBlueprintDataSO _data;

    public void SetUp(BuildingBlueprintDataSO data)
    {
        _data = data;
        _icon.sprite = data.Icon;
        _icon.enabled = true;
        _button.interactable = true;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() =>
        {
            BuildGhostPlacer.Instance.StartPlacing(data);
            CommandUIManager.Instance.ShowCommandPanel();
        });
    }

    public void Clear()
    {
        _icon.enabled = false;
        _button.interactable = false;
        _button.onClick.RemoveAllListeners();
    }
}
