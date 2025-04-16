using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Rect GetScreenRect(Vector2 startPos, Vector2 endPos)
    {
        startPos.y = Screen.height - startPos.y;
        endPos.y = Screen.height - endPos.y;

        Vector2 leftTop = Vector2.Min(startPos, endPos);
        Vector2 rightBottom = Vector2.Max(startPos, endPos);

        return Rect.MinMaxRect(leftTop.x, leftTop.y, rightBottom.x, rightBottom.y);
    }

    public static double GetTime()
    {
        return GameModManager.IsMultiplayer
            ? Photon.Pun.PhotonNetwork.Time
            : Time.time;
    }
}
