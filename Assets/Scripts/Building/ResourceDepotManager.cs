using System.Collections.Generic;
using UnityEngine;

public class ResourceDepotManager : MonoBehaviour
{
    private static ResourceDepotManager _instance;
    private List<IResourceDepot> _depots = new List<IResourceDepot>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static ResourceDepotManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ResourceDepotManager>();
            }
            return _instance;
        }
    }
    public List<IResourceDepot> Depots { get { return _depots; } }

    public void RegisterDepot(IResourceDepot depot)
    {
        if(!_depots.Contains(depot))
        {
            _depots.Add(depot);
        }
    }

    public void UnregisterDepot(IResourceDepot depot)
    {
        if(_depots.Contains(depot))
        {
            _depots.Remove(depot);
        }
    }

    public List<IResourceDepot> GetDepots()
    {
        return _depots;
    }
}
