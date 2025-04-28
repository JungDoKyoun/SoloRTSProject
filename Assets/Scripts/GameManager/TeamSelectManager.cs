using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelectManager : MonoBehaviour
{
    [SerializeField] private List<PlayerSlot> _team1Slots;
    [SerializeField] private List<PlayerSlot> _team2Slots;

    private void Start()
    {
        AutoAssignLocalPlayer();
    }

    public void OnStartGameClick()
    {
        List<SlotData> slotDatas = new List<SlotData>();
        int playerID = 1;

        foreach (var slot in _team1Slots)
        {
            if (!slot.IsOpen())
            {
                SlotData slotData = slot.GetSlotData();
                slotData.PlayerID = playerID;
                playerID++;
                slotDatas.Add(slotData);
            }
        }

        foreach (var slot in _team2Slots)
        {
            if (!slot.IsOpen())
            {
                SlotData slotData = slot.GetSlotData();
                slotData.PlayerID = playerID;
                playerID++;
                slotDatas.Add(slotData);
            }
        }
        PlayerManager.Instance.InitializePlayersFromSlots(slotDatas);
        SceneLoader.Instance.LoadScene("GameScene");
    }

    public void TeamChange(PlayerSlot slot, TeamType newteam)
    {
        List<PlayerSlot> targetSlots = (newteam == TeamType.Team1) ? _team1Slots : _team2Slots;
        var data = slot.GetSlotData();

        PlayerSlot targetSlot = targetSlots.Find(s => s.IsOpen());
        if(targetSlot != null)
        {
            string nicName = data.NickName;
            RaceType race = data.RaceType;

            if(data.IsAI)
            {
                targetSlot.AssignAI(race);
            }
            else
            {
                targetSlot.AssignPlayer(newteam, nicName, race);
            }

            slot.ClearSlot();
        }
        else
        {
            slot.ResetTeamDropdown();
        }
    }

    public void AutoAssignLocalPlayer()
    {
        Debug.Log("½ÇÇà");
        PlayerSlot slot = _team1Slots.Find(s => s.IsOpen());

        if(slot != null)
        {
            slot.AssignPlayer(TeamType.Team1);
            PlayerManager.Instance.SetLocalPlayerSlot(slot);
        }
        else
        {
            PlayerSlot slot2 = _team2Slots.Find(s => s.IsOpen());
            if(slot2 != null)
            {
                slot2.AssignPlayer(TeamType.Team2);
                PlayerManager.Instance.SetLocalPlayerSlot(slot2);
            }
        }
    }
}
