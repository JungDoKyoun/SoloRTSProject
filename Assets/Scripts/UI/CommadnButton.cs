using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CommadButton : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;

    public void SetCommandButton(Sprite icon, UnityAction callback)
    {
        Clear();
        _icon.sprite = icon;
        _icon.enabled = true;
        _button.interactable = true;
        _button.onClick.AddListener(callback);
    }

    public void Clear()
    {
        _icon.enabled = false;
        _button.interactable = false;
        _button.onClick.RemoveAllListeners();
    }
}
