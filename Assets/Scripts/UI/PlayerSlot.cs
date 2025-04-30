using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _nicName;
    [SerializeField] private TMP_Dropdown _raceType;
    [SerializeField] private TMP_Dropdown _team;
    [SerializeField] private TeamSelectManager _teamSelectManager;
    [SerializeField] TeamType _teamType;
    private SlotData _currentSlotData;
    private bool _isOpen;

    private void Awake()
    {
        _nicName.onValueChanged.AddListener(OnStatusChanged);
        _raceType.onValueChanged.AddListener(OnRaceChanged);
        _team.onValueChanged.AddListener(OnTeamChanged);

        Init();
        ClearSlot();
    }

    private void Init()
    {
        _nicName.ClearOptions();
        _nicName.AddOptions(new List<string> { "¿­¸²", "AI" });

        _raceType.ClearOptions();
        _raceType.AddOptions(new List<string> { "ÈÞ¸Õ"});

        _team.ClearOptions();
        _team.AddOptions(new List<string> { "ÆÀ 1", "ÆÀ 2" });
    }

    public void ClearSlot()
    {
        _isOpen = true;
        _currentSlotData = null;
        _nicName.ClearOptions();
        _nicName.AddOptions(new List<string> { "¿­¸²", "AI" });
        _nicName.value = 0;
        _raceType.value = 0;
        _raceType.interactable = false;
        _team.value = (_teamType == TeamType.Team1) ? 0 : 1;
        _team.interactable = false;
    }

    public void AssignPlayer(TeamType teamType, string nickName = "Player", RaceType race = RaceType.Human)
    {
        _isOpen = false;
        _currentSlotData = new SlotData
        {
            PlayerID = 0,
            NickName = nickName,
            RaceType = race,
            TeamType = teamType,
            IsAI = false
        };

        _nicName.ClearOptions();
        _nicName.AddOptions(new List<string> { nickName });
        _nicName.value = 0;
        _nicName.interactable = false;

        _raceType.interactable = true;
        _raceType.value = (int)race;

        _team.interactable = true;
        _team.value = (int)teamType;
    }

    public void AssignAI(RaceType race = RaceType.Human)
    {
        _isOpen = false;
        _currentSlotData = new SlotData
        {
            PlayerID = 0,
            NickName = "AI",
            RaceType = (RaceType) _raceType.value,
            TeamType = (TeamType)_team.value,
            IsAI = true
        };
        _nicName.value = 1;
        _raceType.value = (int)race;
        _raceType.interactable = true;
        _team.interactable = true;
    }

    public bool IsOpen()
    {
        return _isOpen;
    }

    public SlotData GetSlotData()
    {
        return _currentSlotData;
    }

    public void ResetTeamDropdown()
    {
        _team.value = (_teamType == TeamType.Team1) ? 0 : 1;
    }

    private void OnStatusChanged(int value)
    {
        if(value == 0)
        {
            ClearSlot();
        }
        if(value == 1)
        {
            AssignAI();
        }
    }

    private void OnRaceChanged(int value)
    {
        if(!_isOpen && _currentSlotData != null)
        {
            _currentSlotData.RaceType = (RaceType)_raceType.value;
        }
    }

    private void OnTeamChanged(int value)
    {
        if(!_isOpen && _currentSlotData != null)
        {
            _teamSelectManager.TeamChange(this, (TeamType)value);
        }
    }
}
