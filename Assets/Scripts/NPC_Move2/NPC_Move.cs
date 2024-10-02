using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Move : MonoBehaviour
{
    public NavMeshAgent agent;
    private Vector3 targetPosition;
    private bool moveInXAxis = true;
    public float detectionRange = 2.0f;

    // 이동 가능한 범위 설정
    public Vector3 minRange = new Vector3(0, 0, 0);  // 최소 좌표
    public Vector3 maxRange = new Vector3(12, 0, 8); // 최대 좌표

    // Start is called before the first frame update
    void Start()
    {
        agent.updateRotation = false;
        SetRandomDestination();
    }

    // Update is called once per frame
    void Update()
    {
        DetectInFront();
        if (HasReachedDestination())
        {
            // 목표 지점에 정확히 도달하면 새로운 랜덤 목적지를 설정
            SetRandomDestination();
        }
        else
        {
            MoveInGrid();
        }
    }

    // 목표 지점에 정확히 도달했는지 확인하는 함수
    bool HasReachedDestination()
    {
        Vector3 currentPosition = transform.position;

        // X축과 Z축 좌표가 목표 지점과 정확히 일치하는지 확인
        return Mathf.Approximately(currentPosition.x, targetPosition.x) &&
               Mathf.Approximately(currentPosition.z, targetPosition.z);
    }

    void DetectInFront()
    {
        Vector3 rayOrigin = transform.position + new Vector3(0, -0.3f, 0);  // NPC의 아래쪽에서 레이캐스트 시작

        // NPC의 현재 이동 방향에 따라 레이캐스트 방향을 설정
        Vector3 rayDirection = agent.velocity.normalized;

        // velocity가 0일 때를 대비한 기본값
        if (rayDirection == Vector3.zero)
        {
            rayDirection = transform.forward; // 기본적으로 정면으로 설정
        }

        Ray ray = new Ray(rayOrigin, rayDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange))
        {
            // 감지된 물체의 이름이 "Asher"라면
            if (hit.collider.name == "Asher")
            {
                Debug.Log("Asher 감지됨: " + hit.collider.name);

                // Asher의 위치 좌표를 그리드에 스냅
                Vector3 asherPosition = SnapToGrid(hit.transform.position);

                // 스냅된 좌표를 목표 지점으로 설정
                SetDestination(asherPosition);
            }
        }
    }

    void SetDestination(Vector3 destination)
    {
        targetPosition = destination;
        agent.SetDestination(targetPosition);
        Debug.Log("Asher의 위치로 이동: " + targetPosition);
    }

    // OnDrawGizmos를 사용하여 레이캐스트 경로를 Scene에 시각적으로 표시
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // 레이의 색상을 빨간색으로 설정

        Vector3 rayOrigin = transform.position + new Vector3(0, -0.3f, 0);

        // NPC의 현재 이동 방향에 따라 레이캐스트 방향을 설정
        Vector3 rayDirection = agent.velocity.normalized;

        // velocity가 0일 때를 대비한 기본값
        if (rayDirection == Vector3.zero)
        {
            rayDirection = transform.forward; // 기본적으로 정면으로 설정
        }

        // 레이캐스트의 방향으로 감지 범위만큼 Gizmos로 레이 그리기
        Gizmos.DrawRay(rayOrigin, rayDirection * detectionRange);
    }

    // 랜덤한 목적지를 설정하고 스냅하는 함수
    void SetRandomDestination()
    {
        // minRange와 maxRange 사이에서 랜덤한 위치 선택
        Vector3 randomPosition = new Vector3(
            Random.Range(minRange.x, maxRange.x),
            0,  // Y는 항상 0
            Random.Range(minRange.z, maxRange.z)
        );

        // 랜덤 좌표를 그리드에 스냅
        targetPosition = SnapToGrid(randomPosition);
        Debug.Log("다음 위치: " + targetPosition);

        // 목적지 설정
        SetDestination(targetPosition);
    }

    // 정수 좌표로 스냅하는 함수
    Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), position.y, Mathf.Round(position.z));
    }

    Vector3 SnapZToGrid(Vector3 position)
    {
        return new Vector3(position.x, position.y, Mathf.Round(position.z));
    }

    // 그리드 상에서 X축과 Z축으로만 이동하는 함수
    void MoveInGrid()
    {
        Vector3 currentPosition = transform.position;

        if (moveInXAxis)
        {
            // X축 방향으로 이동
            Vector3 nextPosition = new Vector3(targetPosition.x, currentPosition.y, currentPosition.z);
            agent.SetDestination(nextPosition);

            // X축 이동이 완료되었는지 확인 (목표 위치와 일치)
            if (Mathf.Approximately(currentPosition.x, targetPosition.x))
            {
                moveInXAxis = false; // Z축 이동으로 전환
            }
        }
        else
        {
            // Z축 방향으로 이동
            Vector3 nextPosition = SnapZToGrid(new Vector3(currentPosition.x, currentPosition.y, targetPosition.z));
            agent.SetDestination(nextPosition);

            // Z축 이동이 완료되었는지 확인 (목표 위치와 일치)
            if (Mathf.Approximately(currentPosition.z, targetPosition.z))
            {
                moveInXAxis = true; // X축 이동으로 다시 전환
            }
        }
    }
}
