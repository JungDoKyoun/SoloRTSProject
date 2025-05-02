using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AIWorkerManager
{
    public static void AssignInitialWorkers(AIPlayer aIPlayer)
    {
        var worker = UnitRegistry.Instance.GetAllUnits(aIPlayer.PlayerID, 0);

        for(int i = 0; i < worker.Count; i++)
        {
            Resource res;
            if(i < 3)
            {
                res = Utils.FindNearestAvailableResource(worker[i].transform.position, worker[i].UnitData.GatherSearchRadius, ResourcesType.Gold);
            }
            else
            {
                res = Utils.FindNearestAvailableResource(worker[i].transform.position, worker[i].UnitData.GatherSearchRadius, ResourcesType.Wood);
            }

            if(res != null)
            {
                var command = new GatherCommand(worker[i], res, res.transform.position);
                command.Execute();
            }
        }
    }
}
