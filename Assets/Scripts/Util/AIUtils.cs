using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.AI;

public static class AIUtils
{
    public static bool IsCanBuild(Vector3 buildPos, Player player, float radius)
    {
        Ray ray = new Ray(Vector3.up * 10f, Vector3.down);
        if (!Physics.Raycast(ray, out RaycastHit hit, 20f))
            return false;

        if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
            return false;

        if (!NavMesh.SamplePosition(buildPos, out _, 1f, NavMesh.AllAreas))
            return false;

        if (Physics.OverlapSphere(buildPos, radius).Length > 0)
            return false;

        Collider[] resHits = Physics.OverlapSphere(buildPos, 7f, LayerMask.GetMask("Resources"));
        foreach(var reshit in resHits)
        {
            Resource res = reshit.GetComponent<Resource>();

            if (res != null && res.IsAvailable())
                return false;
        }

        return true;
    }

    public static bool FindBuildPos(Player player, BuildingBlueprintDataSO data, float searchRadius, out Vector3 buildPos)
    {
        buildPos = Vector3.zero;
        List<IResourceDepot> depots = ResourceDepotManager.Instance.GetDepots();

        foreach(var depot in depots)
        {
            if (depot.GetPlayer() != player)
                continue;

            Vector3 center = depot.GetPos();

            for(int i = 0; i < 100; i++)
            {
                Vector2 random = Random.insideUnitCircle * searchRadius;
                Vector3 randomPos = center + new Vector3(random.x, 0, random.y);

                if(IsCanBuild(randomPos, player, data.BuildingRadius))
                {
                    buildPos = randomPos;
                    return true;
                }
            }
        }

        return false;
    }
}
