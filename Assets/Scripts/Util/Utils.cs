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

    public static Resources FindNearestAvailableResource(Vector3 workerPos, float radius, ResourcesType resType)
    {
        Collider[] hits = Physics.OverlapSphere(workerPos, radius, LayerMask.GetMask("Resources"));
        Resources nearResources = null;
        float minDistance = float.MaxValue;

        foreach(var hit in hits)
        {
            Resources resources = hit.GetComponent<Resources>();

            if(resources != null && resources.IsAvailable() && resources.Type == resType)
            {
                float distance = Vector3.Distance(workerPos, resources.transform.position);

                if(minDistance > distance)
                {
                    minDistance = distance;
                    nearResources = resources;  
                }
            }
        }
        return nearResources;
    }
}
