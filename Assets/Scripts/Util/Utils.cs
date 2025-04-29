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

    public static Resource FindNearestAvailableResource(Vector3 workerPos, float radius, ResourcesType resType)
    {
        Collider[] hits = Physics.OverlapSphere(workerPos, radius, LayerMask.GetMask("Resources"));
        Resource nearResources = null;
        float minDistance = float.MaxValue;

        foreach(var hit in hits)
        {
            Resource resources = hit.GetComponent<Resource>();

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

    public static Building FindNearestOwnedDepot(UnitController unit)
    {
        var depots = ResourceDepotManager.Instance.GetDepots();
        Building nearDepot = null;
        float minDis = float.MaxValue;

        foreach(var depot in depots)
        {
            if(depot is Building building && building.IsPlayerBuilding(unit.Player))
            {
                float distance = Vector3.Distance(unit.transform.position, building.transform.position);

                if(minDis > distance)
                {
                    minDis = distance;
                    nearDepot = building;
                }
            }
        }
        return nearDepot;
    }

    public static IUnitState GetStateByName(string name)
    {
        switch(name)
        {
            case "IdleState":
                return new IdleState();
            case "MoveState":
                return new MoveState();
            case "ChaseState":
                return new ChaseState();
            case "AttackState":
                return new AttackState();
            case "MoveAttackState":
                return new MoveAttackState();
            case "MoveToGatherState":
                return new MoveToGatherState();
            case "GatherState":
                return new GatherState();
            case "ReturnToBaseState":
                return new ReturnToBaseState();
            case "MoveToBuildstate":
                return new MoveToBuildstate();
            case "BuildState":
                return new BuildState();
            case "DieState":
                return new DieState();
            default:
                return new IdleState();
        }
    }

    public static Vector3 GetBuildSize(GameObject obj)
    {
        Collider collider = obj.GetComponent<Collider>();
        if (collider != null)
        {
            return collider.bounds.size;
        }
        return new Vector3(2f, 2f, 2f);
    }
}
