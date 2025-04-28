using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if(player.RaceType == RaceType.Human)
            {
                buildData = _humanBuildData;
            }

            int index = Random.Range(0, startList.Count);
            Transform pos = startList[index];
            startList.RemoveAt(index);
            Building building = Instantiate(_humanBuildPrefab, pos.position, _humanBuildPrefab.transform.rotation).GetComponent<Building>();
            building.Init(buildData, player.PlayerID, buildData.MaxHP);
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
                UnitController unit = Instantiate(_humanUnitPrefab, sponPos, Quaternion.identity).GetComponent<UnitController>();
                UnitManager unitManager = unit.GetComponent<UnitManager>(); 
                unit.Init(unitManager, player.PlayerID);
            }
        }
    }
}
