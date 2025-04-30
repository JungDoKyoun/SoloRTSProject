using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameInitializer : MonoBehaviour
{
    [Header("건물 데이터")]
    [SerializeField] BuildingBlueprintDataSO _humanBuildData;

    [Header("건물 프리펩")]
    [SerializeField] GameObject _humanBuildPrefab;

    [Header("유닛 데이터")]
    [SerializeField] UnitDataSO _humanUnitData;

    [Header("유닛 프리펩")]
    [SerializeField] GameObject _humanUnitPrefab;

    [Header("시작 위치")]
    [SerializeField] List<Transform> _startPos;

    [Header("AI프리펩")]
    [SerializeField] GameObject _aiPlayerPrefab;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        var players = PlayerManager.Instance.GetAllPlayer();
        List<Transform> startList = new List<Transform>(_startPos);
        BuildingBlueprintDataSO buildData = null;

        foreach (var player in players)
        {
            if(player.IsAI)
            {
                var aiObj = Instantiate(_aiPlayerPrefab);
                var ai = aiObj.GetComponent<AIPlayer>();
                ai.Init(player);
                AIPlayerRegistry.Instance.RegisterAIPlayer(player.PlayerID, ai);
            }

            if(player.RaceType == RaceType.Human)
            {
                buildData = _humanBuildData;
            }

            int index = Random.Range(0, startList.Count);
            Transform pos = startList[index];
            startList.RemoveAt(index);
            var buildingObj = Instantiate(_humanBuildPrefab, pos.position, _humanBuildPrefab.transform.rotation);
            Building building = buildingObj.GetComponent<Building>();
            building.Init(buildData, player.PlayerID, buildData.MaxHP);
            buildingObj.AddComponent<NavMeshObstacle>();
            var nav = buildingObj.GetComponent<NavMeshObstacle>();
            nav.carving = true;
            MeshRenderer meshRenderer = building.gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                BoxCollider boxCollider = building.gameObject.AddComponent<BoxCollider>();
                boxCollider.center = building.transform.InverseTransformPoint(meshRenderer.bounds.center);
                boxCollider.size = meshRenderer.bounds.size;
            }

            for (int i = 0; i < 4; i++)
            {
                Vector3 sponPos = pos.position + new Vector3(1 * i, 0, -4);
                UnitController unit = UnitPoolManager.Instance.GetUnit(player.RaceType, _humanUnitData, player.PlayerID, sponPos);
                UnitManager unitManager = unit.GetComponent<UnitManager>(); 
                unit.Init(unitManager, player.PlayerID);
            }
        }
    }
}
