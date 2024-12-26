using System.Collections.Generic;
using UnityEngine;

public class NPC_Detection : MonoBehaviour
{
    public float detectionRange = 3f; // 감지 거리
    public float detectionAngle = 30f; // 시야각
    public Material lineMaterial; // LineRenderer에 사용할 재질
    private List<LineRenderer> lineRenderers = new List<LineRenderer>(); // 여러 LineRenderer 저장

    private int segments = 50; // 부채꼴 세그먼트 개수

    void Start()
    {
        CreateLineRenderers();
    }

    void Update()
    {
        DrawFan();
    }

    private void CreateLineRenderers()
    {
        for (int i = 0; i <= segments; i++)
        {
            LineRenderer lr = new GameObject("LineSegment").AddComponent<LineRenderer>();
            lr.transform.parent = this.transform; // NPC 객체에 부모화
            lr.material = lineMaterial;
            lr.widthMultiplier = 0.05f; // 선의 두께
            lr.useWorldSpace = false; // 로컬 좌표 사용
            lr.positionCount = 2; // 시작점과 끝점
            lineRenderers.Add(lr);
        }
    }

    private void DrawFan()
    {
        float stepAngle = detectionAngle / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = -detectionAngle / 2 + stepAngle * i;

            Vector3 start = Vector3.zero; // 부채꼴 중심
            Vector3 end = Quaternion.Euler(0, angle, 0) * Vector3.forward * detectionRange; // 부채꼴 끝점

            // LineRenderer로 선 그리기
            lineRenderers[i].SetPosition(0, start); // 시작점
            lineRenderers[i].SetPosition(1, end);   // 끝점
        }
    }
}
