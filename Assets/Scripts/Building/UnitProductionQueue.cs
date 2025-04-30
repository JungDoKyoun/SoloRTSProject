using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitProductionQueue : MonoBehaviour
{
    private List<UnitDataSO> _unitProductionQueue = new List<UnitDataSO>();
    private int _maxQueueSize = 6;
    private UnitDataSO _currentUnit;
    private Player _player;
    private RaceType _raceType;
    private ProductionBuilding _building;
    private Vector3 _spawnPos;
    private bool _isProduce = false;

    public void Init(Player player)
    {
        _player = player;
        _raceType = player.RaceType;
    }

    public bool AddProductionQueue(UnitDataSO unitData)
    {
        if (_unitProductionQueue.Count >= _maxQueueSize)
        {
            return false;
        }

        if (!_player.IsenoughResources(unitData.Costs))
        {
            return false;
        }

        if(!_player.IsCanProduceUnit(unitData.SupplyCost))
        {
            return false;
        }

        _player.UseResources(unitData.Costs);
        _unitProductionQueue.Add(unitData);

        if(!_isProduce)
        {
            StartCoroutine(SpawnUnit());
        }

        return true;
    }

    public void CancelProduction(int queueIndex = -1)
    {
        if(_unitProductionQueue.Count <= 0)
        {
            return;
        }

        int cancelIndex = queueIndex == -1 ? _unitProductionQueue.Count - 1 : queueIndex;

        if(cancelIndex < 0 && cancelIndex >= _unitProductionQueue.Count)
        {
            return;
        }

        var cancelUnit = _unitProductionQueue[cancelIndex];
        _player.AddResources(cancelUnit.Costs);
        _unitProductionQueue.RemoveAt(cancelIndex);
    }

    public List<UnitDataSO> GetQueue()
    {
        return _unitProductionQueue;
    }

    public Vector3 FindSpawnPos(Vector3 center, Vector3 buildSize)
    {
        float baseRadius = Mathf.Max(buildSize.x, buildSize.z);

        Vector3[] directions = new Vector3[]
        {
            Vector3.forward,
            Vector3.forward + Vector3.right,
            Vector3.right,
            Vector3.back + Vector3.right,
            Vector3.back,
            Vector3.back + Vector3.left,
            Vector3.left,
            Vector3.forward + Vector3.right
        };

        for(int r = 1; r <= 5; r++)
        {
            float radius = baseRadius + r - 1;

            foreach(var dir in directions)
            {
                Vector3 checkPos = center + dir.normalized * radius;

                if (!Physics.CheckSphere(checkPos, 0.5f, LayerMask.GetMask("Unit")))
                {
                    if(NavMesh.SamplePosition(checkPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                    {
                        return hit.position;
                    }
                    else
                    {
                        Ray ray = new Ray(checkPos + Vector3.up * 5f, Vector3.down);
                        if (Physics.Raycast(ray, out RaycastHit rayHit, 10f, LayerMask.GetMask("Ground")))
                        {
                            return rayHit.point;
                        }

                        return checkPos;
                    }
                }
            }
        }

        return center + Vector3.forward * baseRadius;
    }

    private IEnumerator SpawnUnit()
    {
        _isProduce = true;

        while(_unitProductionQueue.Count > 0)
        {
            _currentUnit = _unitProductionQueue[0];

            yield return new WaitForSeconds(_currentUnit.ProductionTime);

            while(!_player.IsCanProduceUnit(_currentUnit.SupplyCost))
            {
                yield return new WaitForSeconds(0.5f);
            }

            var buildSize = Utils.GetBuildSize(gameObject);
            var center = transform.position + transform.forward * (buildSize.z / 2f + 0.5f);
            _spawnPos = FindSpawnPos(center, buildSize);

            if(GameModManager.IsMultiplayer)
            {
                UnitPoolManager.Instance.MultiGetUnit(_raceType, _currentUnit, _player.PlayerID, _spawnPos);
            }
            else
            {
                UnitPoolManager.Instance.GetUnit(_raceType, _currentUnit, _player.PlayerID, _spawnPos);
            }

            _player.IncreaseCurrentSupply(_currentUnit.SupplyCost);
            _unitProductionQueue.RemoveAt(0);
        }

        _isProduce = false;
    }
}
