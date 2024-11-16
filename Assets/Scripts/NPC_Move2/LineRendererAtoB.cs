using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererAtoB : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector3 from;
    private Vector3 to;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true; // 월드 좌표계를 사용
        lineRenderer.enabled = false;
    }

    public void Play(Vector3 startPoint, Vector3 endPoint)
    {
        from = startPoint;
        to = endPoint;

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
    }

    public void UpdatePositions(Vector3 startPoint, Vector3 endPoint)
    {
        from = startPoint;
        to = endPoint;
    }

    private void Update()
    {
        // 매 프레임 시작점과 끝점을 갱신
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
    }

    public void Stop()
    {
        lineRenderer.enabled = false;
    }
}
