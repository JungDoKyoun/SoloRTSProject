using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlot : MonoBehaviour
{
    private SlotData _currentSlotData;
    private bool _isOpen;

    public bool IsOpen()
    {
        return _isOpen;
    }

    public SlotData GetSlotData()
    {
        return _currentSlotData;
    }
}
