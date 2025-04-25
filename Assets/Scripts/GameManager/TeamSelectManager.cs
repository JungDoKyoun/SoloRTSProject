using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelectManager : MonoBehaviour
{
    [SerializeField] private List<PlayerSlot> _team1Slots;
    [SerializeField] private List<PlayerSlot> _team2Slots;

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
        Debug.Log("asd");
        PlayerManager.Instance.InitializePlayersFromSlots(slotDatas);
        Debug.Log(SceneLoader.Instance == null ? "SceneLoader is null" : "SceneLoader is not null");
        SceneLoader.Instance.LoadScene("GameScene");
    }
}
