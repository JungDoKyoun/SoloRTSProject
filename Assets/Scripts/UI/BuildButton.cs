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
            //건물 형체 넣는 함수 넣어라
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
