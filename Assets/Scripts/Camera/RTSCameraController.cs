using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    [Header("맵 이동 속도")]
    [SerializeField] private float _mapMoveSpeed;

    [Header("맵 밖으로 나가지 않게 최대 최소 X,Z값 제한")]
    [SerializeField] private Vector2 _mapLimitX;
    [SerializeField] private Vector2 _mapLimitZ;

    [Header("가장자리 지정할 비율")]
    [Range(0, 0.2f)][SerializeField] private float edgePer;

    [Header("마우스 휠 속도")]
    [SerializeField] private float _mouseScrollSpeed;

    [Header("마우스 휠 최소, 최대")]
    [SerializeField] private float _minScroll;
    [SerializeField] private float _maxScroll;

    private void Update()
    {
        CamMove();
        CamZoom();
    }

    private void CamMove()
    {
        Vector3 dir = Vector3.zero;

        float edgeX = Screen.width * edgePer;
        float edgeZ = Screen.height * edgePer;
        Vector3 mousePos = Input.mousePosition;
        Vector3 pos = transform.position;

        if (mousePos.x < edgeX)
        {
            dir.x -= 1;
        }
        if (mousePos.x > Screen.width - edgeX)
        {
            dir.x += 1;
        }
        if(mousePos.y < edgeZ)
        {
            dir.z -= 1;
        }
        if(mousePos.y > Screen.height - edgeZ)
        {
            dir.z += 1;
        }

        Vector3 finalPos = pos + dir * _mapMoveSpeed * Time.deltaTime;

        finalPos.x = Mathf.Clamp(finalPos.x, _mapLimitX.x, _mapLimitX.y);
        finalPos.z = Mathf.Clamp(finalPos.z, _mapLimitZ.x, _mapLimitZ.y);

        transform.position = finalPos;
    }

    private void CamZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 pos = transform.position;
        pos.y -= scroll * _mouseScrollSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, _minScroll, _maxScroll);
        transform.position = pos;
    }
}
