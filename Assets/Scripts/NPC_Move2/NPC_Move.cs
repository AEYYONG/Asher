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
    public Vector3 maxRange = new Vector3(12, 0, 7); // 최대 좌표

    // 애니메이션
    public Animator animator;
    private string currentAnimation;

    //추적 관련 변수
    public bool isChasing = false;
    public bool isBlocked = false;


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
        if (isChasing)
        {
            GameObject asher = GameObject.Find("Asher");
            if (!isBlocked)
            {
                SetDestination(SnapToGrid(asher.transform.position));
            }
        }
        if (!isChasing && HasReachedDestination())
        {
            // 목표 지점에 정확히 도달하면 새로운 랜덤 목적지를 설정
            SetRandomDestination();
        }
        else
        {
            MoveInGrid();
        }

        UpdateAnimation();
        transform.rotation = Quaternion.Euler(70, 0, 0);

    }

    // 목표 지점에 정확히 도달했는지 확인하는 함수
    bool HasReachedDestination()
    {
        Vector3 currentPosition = transform.position;

        // X축과 Z축 좌표가 목표 지점과 일정 범위 내에 있는지 확인
        float threshold = 0.1f;  // 허용 오차 범위

        bool isXCloseEnough = Mathf.Abs(currentPosition.x - targetPosition.x) < threshold;
        bool isZCloseEnough = Mathf.Abs(currentPosition.z - targetPosition.z) < threshold;

        return isXCloseEnough && isZCloseEnough;
    }

    void DetectInFront()
    {
        Vector3 rayOrigin = transform.position + new Vector3(0, 0f, 0);  // NPC의 아래쪽에서 레이캐스트 시작

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
                // Debug.Log("Asher 감지됨: " + hit.collider.name);

                // Asher의 위치 좌표를 그리드에 스냅
                Vector3 asherPosition = SnapToGrid(hit.transform.position);

                // 스냅된 좌표를 목표 지점으로 설정
                SetDestination(asherPosition);
                isChasing = true;
                //   Debug.Log("asherPosition: " + asherPosition);
                //   Debug.Log("애셔 위치: " + targetPosition);
            }
        }
    }

    void SetDestination(Vector3 destination)
    {
        NavMeshHit hit;

        // Z축이 0인 경우를 특별히 처리
        if (Mathf.Approximately(destination.z, 0))
        {
            destination.z = 0.1f; // Z축이 0일 때 약간의 값을 추가해 문제를 피함
        }

        // NavMesh 상에서 유효한 좌표를 찾기
        if (NavMesh.SamplePosition(destination, out hit, 1.0f, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
            agent.SetDestination(targetPosition);
            //  Debug.Log("목표 지점 설정: " + targetPosition);
        }
        else
        {
            Debug.LogWarning("NavMesh 상에서 유효하지 않은 위치입니다.");
        }
    }

    // OnDrawGizmos를 사용하여 레이캐스트 경로를 Scene에 시각적으로 표시
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // 1. 현재 위치에서 목표 위치로 레이 캐스트 (빨간색)
            Vector3 from = transform.position;
            Vector3 to = targetPosition;

            Vector3 direction = (to - from).normalized;
            float distance = Vector3.Distance(from, to);

            // 목표 위치를 향한 빨간색 레이
            Gizmos.color = Color.red;
            Gizmos.DrawRay(from, direction * distance);

            // 2. NPC의 이동 방향으로 1f 거리의 검은색 레이 추가
            Vector3 rayOrigin = transform.position + new Vector3(0, -0.3f, 0);  // NPC의 아래에서 레이 시작
            Vector3 rayDirection = agent.velocity.normalized;

            // velocity가 0일 때 대비한 기본값
            if (rayDirection == Vector3.zero)
            {
                rayDirection = transform.forward;
            }

            // 검은색 레이로 1f 거리까지 시각화
            Gizmos.color = Color.black;
            float rayDistance = 1f;  // 1f 거리 설정
            Gizmos.DrawRay(rayOrigin, rayDirection * rayDistance);

            // 3. 이동 방향으로 감지 범위만큼의 레이 캐스트 (빨간색)
            Gizmos.color = Color.red;  // 색상 다시 빨간색으로 설정
            Gizmos.DrawRay(rayOrigin, rayDirection * detectionRange);
        }
    }

    // 랜덤한 목적지를 설정하고 스냅하는 함수
    void SetRandomDestination()
    {
        bool validPositionFound = false;

        while (!validPositionFound)
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

            NavMeshHit hit;

            if (!IsPositionTaggedAsObstacle(targetPosition))
            {
                // 유효한 위치를 찾으면 목표 지점 설정
                SetDestination(targetPosition);
                validPositionFound = true;
            }
            else
            {
                SetRandomDestination();
                Debug.Log("유효 위치 재탐색");
            }
        }
    }

    bool IsPositionTaggedAsObstacle(Vector3 position)
    {
        // 반경 0.1의 구 모양으로 해당 위치의 충돌체를 감지
        Collider[] hitColliders = Physics.OverlapSphere(position, 0.1f);

        foreach (var collider in hitColliders)
        {
            // 태그가 "Obstacle"인 콜라이더가 있는지 확인
            if (collider.CompareTag("Obstacle"))
            {
                return true; // 장애물이 있음
            }
        }

        return false; // 장애물이 없음
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
            // X축 방향으로 이동 시도
            Vector3 nextPosition = new Vector3(targetPosition.x, currentPosition.y, currentPosition.z);

            //z축 스냅
            currentPosition.z = Mathf.Round(currentPosition.z);
            transform.position = currentPosition;

            // X축 이동 시 장애물 확인
            if (IsPathBlocked(currentPosition, nextPosition))
            {
                Debug.Log("X축에 장애물 감지");
                // X축 경로가 막혀있으면 Z축을 먼저 이동
                if (isChasing)
                {
                    isBlocked = true;
                }
                GameObject asher = GameObject.Find("Asher");
                Vector3 randomPosition = FindRandomPositionInSemiCircle();
                Debug.Log("플레이어 위치" + SnapToGrid(asher.transform.position));
                Debug.Log("랜덤 위치" + randomPosition);
                SetDestination(SnapToGrid(randomPosition));
                Debug.Log("재정의된 위치: " + SnapToGrid(randomPosition));
                moveInXAxis = false;

            }
            else
            {
                agent.SetDestination(nextPosition);
                if (Mathf.Abs(currentPosition.x - targetPosition.x) < 0.1f)
                {
                    moveInXAxis = false; // X축 이동 완료 후 Z축 이동으로 전환
                    isBlocked = false;
                }
            }
        }
        else
        {
            // Z축 방향으로 이동 시도
            Vector3 nextPosition = new Vector3(currentPosition.x, currentPosition.y, targetPosition.z);
            // X축 좌표를 정수로 스냅
            currentPosition.x = Mathf.Round(currentPosition.x);
            transform.position = currentPosition;
            // Z축 이동 시 장애물 확인
            if (IsPathBlocked(currentPosition, nextPosition))
            {
                Debug.Log("Z축에 장애물 감지");
                if (isChasing)
                {
                    isBlocked = true;
                }

                Vector3 randomPosition = FindRandomPositionInSemiCircle();
                Debug.Log("플레이어 위치" + randomPosition);
                Debug.Log("랜덤 위치" + randomPosition);
                SetDestination(SnapToGrid(randomPosition));
                Debug.Log("재정의된 위치" + randomPosition);
                // Z축 경로가 막혀있으면 다시 X축을 시도하거나 다른 처리

                moveInXAxis = true;

            }
            else
            {
                agent.SetDestination(nextPosition);
                if (Mathf.Abs(currentPosition.z - targetPosition.z) < 0.1f)
                {
                    moveInXAxis = true; // Z축 이동 완료 후 다시 X축 이동으로 전환
                    isBlocked = false;
                }
            }
        }
    }
    Vector3 FindRandomPositionInSemiCircle()
    {
        Vector3 currentPosition = transform.position;
        Vector3 direction = (targetPosition - currentPosition).normalized;

        // 반원 형태로 장애물이 없는 랜덤한 위치 찾기
        float radius = 2.0f;  // 탐색할 반원의 반지름
        List<Vector3> possiblePositions = new List<Vector3>();

        for (int angle = -90; angle <= 90; angle += 10)
        {
            float radian = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * radius;
            Vector3 candidatePosition = currentPosition + direction + offset;

            if (!IsPositionTaggedAsObstacle(candidatePosition))
            {
                possiblePositions.Add(candidatePosition);
            }
        }

        if (possiblePositions.Count > 0)
        {
            return possiblePositions[Random.Range(0, possiblePositions.Count)];
        }

        return currentPosition;  // 만약 유효한 위치가 없으면 현재 위치 반환
    }


    // 경로 상에 장애물이 있는지 확인하는 함수
    bool IsPathBlocked(Vector3 from, Vector3 to)
    {
        RaycastHit hit;
        Vector3 direction = (to - from).normalized;
        float distance = 0.4f;

        Debug.DrawRay(from, direction * distance, Color.black, 0.1f); // 0.1초 동안 검은색 레이 표시


        if (Physics.Raycast(from, direction, out hit, distance))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                return true; // 장애물이 경로 상에 있음
            }
        }
        return false; // 장애물 없음
    }



    // 애니메이션 변경
    void UpdateAnimation()
    {
        Vector3 velocity = agent.velocity;
        if (Mathf.Abs(velocity.z) > Mathf.Abs(velocity.x))
        {
            if (velocity.z > 0 && currentAnimation != "up_npc")
            {

                ChangeAnimationState("up_npc");
            }
            else if (velocity.z < 0 && currentAnimation != "down_npc")
            {
                ChangeAnimationState("down_npc");
            }
        }

        else
        {
            if (velocity.x > 0 && currentAnimation != "right_npc")
            {

                ChangeAnimationState("right_npc");
            }
            else if (velocity.x < 0 && currentAnimation != "left_npc")
            {
                ;
                ChangeAnimationState("left_npc");
            }
        }
    }

    void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;

        animator.Play(newAnimation);
        currentAnimation = newAnimation;
    }

}