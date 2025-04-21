using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandUIManager : MonoBehaviour
{
    private static CommandUIManager _instance;
    [SerializeField] GameObject _commandPanel;
    [SerializeField] GameObject _buildPanel;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(_instance);
        }
    }

    public static CommandUIManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<CommandUIManager>();
            }
            return _instance;
        }
    }

    public void ShowCommandPanel()
    {
        _commandPanel.SetActive(true);
        _buildPanel.SetActive(false);
    }

    public void ShowBuildPanel()
    {
        Debug.Log("µé¾î¿È");
        _commandPanel.SetActive(false);
        _buildPanel.SetActive(true);
    }
}
