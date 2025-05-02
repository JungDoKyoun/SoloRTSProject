using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingRegistry : MonoBehaviour
{
    private static BuildingRegistry _instance;
    private List<Building> _allBuildings = new List<Building>();

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static BuildingRegistry Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<BuildingRegistry>();
            }
            return _instance;
        }
    }
    public List<Building> AllBuildings => _allBuildings;

    public void Register(Building building)
    {
        if(!_allBuildings.Contains(building))
        {
            _allBuildings.Add(building);
        }
    }

    public void UnRegister(Building building)
    {
        if(_allBuildings.Contains(building))
        {
            _allBuildings.Remove(building);
        }
    }

    public int GetBuildingByID(int playerID, int buildingID)
    {
        return _allBuildings.Count(b => b.Player.PlayerID == playerID && b.GetBuildData().ID == buildingID);
    }

    public bool HasBuilding(int playerID, int buildingID)
    {
        return GetBuildingByID(playerID, buildingID) > 0;
    }
}
