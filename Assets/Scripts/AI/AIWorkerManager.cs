using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
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

    public static void AssignNewWorker(UnitController unit, AIPlayer aiPlayer)
    {
        var pattern = aiPlayer.CurrentPhase.GatherPattern;
        int goldTarget = aiPlayer.CurrentPhase.TargetGoldWorkerCount;
        int woodTarget = aiPlayer.CurrentPhase.TargetWoodWorkerCount;
        int totalTarget = goldTarget + woodTarget;

        var allWorkers = UnitRegistry.Instance.GetAllUnits(aiPlayer.PlayerID, 0);
        int currentGold = 0;
        int currentWood = 0;

        foreach(var worker in allWorkers)
        {
            if (worker.CurrentResourceType == ResourcesType.Gold)
                currentGold++;

            else if (worker.CurrentResourceType == ResourcesType.Wood)
                currentWood++;
        }

        int totalAssigned = currentGold + currentWood;

        if (totalAssigned >= totalTarget)
            return;

        Resource res = null;

        if(currentGold < goldTarget / 2)
        {
            res = Utils.FindNearestAvailableResource(unit.transform.position , unit.GatherSearchRadius, ResourcesType.Gold);
        }
        else if(currentWood < woodTarget / 2)
        {
            res = Utils.FindNearestAvailableResource(unit.transform.position, unit.GatherSearchRadius, ResourcesType.Wood);
        }

        if(res == null)
        {
            int index = aiPlayer.WorkerAssignIndex % pattern.Count;
            ResourcesType resType = pattern[index];
            aiPlayer.WorkerAssignIndex++;

            if (resType == ResourcesType.Gold && goldTarget > currentGold)
            {
                res = Utils.FindNearestAvailableResource(unit.transform.position, unit.GatherSearchRadius, resType);
            }
            else if (resType == ResourcesType.Wood && woodTarget > currentWood)
            {
                res = Utils.FindNearestAvailableResource(unit.transform.position, unit.GatherSearchRadius, resType);
            }
            else if (currentGold < goldTarget)
            {
                res = Utils.FindNearestAvailableResource(unit.transform.position, unit.GatherSearchRadius, resType);
            }
            else if (currentWood < woodTarget)
            {
                res = Utils.FindNearestAvailableResource(unit.transform.position, unit.GatherSearchRadius, resType);
            }
            else
            {
                res = Utils.FindNearestAvailableResource(unit.transform.position, unit.GatherSearchRadius, ResourcesType.Gold);
            }
        }

        if(res != null)
        {
            var commad = new GatherCommand(unit, res, res.transform.position);
            commad.Execute();
        }
    }
}
