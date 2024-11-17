using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Move : MonoBehaviour
{
    public NavMeshAgent agent;
    private Vector3 targetPosition;
    private bool moveInXAxis = true;
    public float detectionRange = 3f;

    // 이동 가능한 범위 설정
    public Vector3 minRange = new Vector3(0, 0, 0);  // 최소 좌표
    public Vector3 maxRange = new Vector3(12, 0, 7); // 최대 좌표

    // 애니메이션
    public Animator animator;
    private string currentAnimation;

    //추적 관련 변수
    public bool isChasing = false;
    public bool isBlocked = false;
    public bool canDetect = true; //NPC가 애셔를 감지할 수 있는지(아이템 사용 시 이용)

    // 공격, 회피 관련
    private GameObject asher;
    public float attackRange = 1.5f;     // 공격 레이캐스트 범위
    public float attackDistance = 0.5f;  // 공격 감지 거리
    public bool isAttack = false;

    // 그린존 감지 관련 // ischasing일 때로 통일해도 될 듯
    public bool goInGreenZone = false;
    public float greenZoneDistance = 1.2f;  // 그린존 감지 범위
    private bool greenZoneAttack = false;
    private bool isAnimationLocked = false;
    private bool notDizzy = true;


    // Start is called before the first frame update
    void Start()
    {
        agent.updateRotation = false;
        asher = GameObject.Find("Asher");
        SetRandomDestination();
    }

    // Update is called once per frame
    void Update()
    {
        DetectInFront();
        if (isChasing)
        {
            agent.speed = 2.5f;
            SetDestination(SnapToGrid(asher.transform.position));

        }
        if (!isChasing && HasReachedDestination())
        {
            // 목표 지점에 정확히 도달하면 새로운 랜덤 목적지를 설정
            SetRandomDestination();
        }
        
        UpdateAnimation();
        transform.rotation = Quaternion.Euler(70, 0, 0);

    }
    //헤어볼에 맞으면 잠깐 멈춤
    public void AttackedHairBall()
    {
        Debug.Log("AttackedHairBall 호출됨");
        //NPC 잠깐 멈춤 현재는 IEnumerator로 2초간 정지처리하지만 추후 애니메이션 종료 후 agent.isStopped = false;로 변경해야 함
        agent.isStopped = true;
        StartCoroutine(WaitForSecond());
    }

    private IEnumerator WaitForSecond()
    {
        Debug.Log("코루틴 시작");
        yield return new WaitForSeconds(2f);

        Debug.Log("npc 다시 움직임");
        agent.isStopped = false;

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
        Vector3 rayOrigin = transform.position + new Vector3(0, 0f, 0);

        // NPC의 현재 이동 방향에 따라 레이캐스트 방향을 설정
        Vector3 rayDirection = agent.velocity.normalized;
        // velocity가 0일 때를 대비한 기본값
        if (rayDirection == Vector3.zero)
        {
            rayDirection = transform.forward; // 기본적으로 정면으로 설정
        }

        Ray attackRay = new Ray(rayOrigin, rayDirection);
        RaycastHit attackHit;

        if (Physics.Raycast(attackRay, out attackHit, attackDistance))
        {
            // 공격 범위 내에서 Asher를 감지하면 isAttack true로 설정
            if (attackHit.collider.name == "Asher")
            {
                Debug.Log("Asher가 공격 범위 내에 있습니다! 공격 시작");
                isAttack = true;

                // 공격 애니메이션 실행 (필요시)
                ChangeAnimationState("attack");

                // NPC를 멈추게 하려면 NavMeshAgent 멈춤
                agent.isStopped = true;
            }
        }
        // 감지 레이 범위
        Ray ray = new Ray(rayOrigin, rayDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange))
        {
            // 감지된 물체의 이름이 "Asher"라면
            if (hit.collider.name == "Asher" && canDetect)
            {
                // Debug.Log("Asher 감지됨: " + hit.collider.name);

                // Asher의 위치 좌표를 그리드에 스냅
                Vector3 asherPosition = SnapToGrid(hit.transform.position);

                // 스냅된 좌표를 목표 지점으로 설정
                SetDestination(asherPosition);
                isChasing = true;
                agent.speed = 5;
                Invoke("ResetSpeed", 3f);
                //   Debug.Log("asherPosition: " + asherPosition);
                //   Debug.Log("애셔 위치: " + targetPosition);
            }
        }
        if (isChasing)
        {

            Ray downwardRay = new Ray(rayOrigin + rayDirection * greenZoneDistance, Vector3.down); // 진행 방향으로 한 칸 앞에서 아래로
            RaycastHit downwardHit;

            if (Physics.Raycast(downwardRay, out downwardHit, detectionRange))
            {
                if (downwardHit.collider.CompareTag("GreenZone"))
                {
                    Debug.Log("그린존 감지됨: " + downwardHit.collider.name);
                    goInGreenZone = true;
                    greenZoneAttack = true;
                    Vector3 backwardDirection = -rayDirection; // 이동 방향의 반대 방향
                    agent.velocity = backwardDirection * 0.3f; // 0.3 유닛만큼 후진
                    
                    Debug.Log("멈춤");
                }
                else
                {
                   // Debug.Log("타일 감지됨: " + downwardHit.collider.name);
                    
                }
            }

        }
        // 디버그 레이
        Debug.DrawRay(rayOrigin, rayDirection * detectionRange, Color.red); // 정면
        Debug.DrawRay(rayOrigin + rayDirection * greenZoneDistance, Vector3.down * greenZoneDistance, Color.green); // 아래 방향
    }


    public void Dizzy()
    {
        ChangeAnimationState("dizzy");
        notDizzy = false;
        isAnimationLocked = true;
        agent.isStopped = true;
        isChasing = false;
        greenZoneAttack = false;
        Invoke("WakeUp",3f);
    }

    
    
    void ResetSpeed()
    {
        if (isChasing)
        {
            agent.speed = 2.5f;
            Debug.Log("속도 감소: " + agent.speed);
        }
        else
            agent.speed = 1.5f;
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
            Vector3 rayOrigin = transform.position + new Vector3(0,0.1f, 0); 
            Vector3 rayDirection = agent.velocity.normalized;

            // velocity가 0일 때 대비한 기본값
            if (rayDirection == Vector3.zero)
            {
                rayDirection = transform.forward;
            }

            // 파란 레이로 1f 거리까지 시각화
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(rayOrigin, rayDirection * attackDistance);

           Vector3 rayOrigi2n = transform.position + new Vector3(0, 0, 0);
            // 3. 이동 방향으로 감지 범위만큼의 레이 캐스트 (빨간색)
          Gizmos.color = Color.red;  // 색상 다시 빨간색으로 설정
          Gizmos.DrawRay(rayOrigi2n, rayDirection * detectionRange);
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
         //   Debug.Log("다음 위치: " + targetPosition);

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
            //    Debug.Log("유효 위치 재탐색");
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
        return new Vector3(Mathf.Round(position.x), position.y, Mathf.Floor(position.z));
    }

    Vector3 SnapZToGrid(Vector3 position)
    {
        return new Vector3(position.x, position.y, Mathf.Round(position.z));
    }

 

    // 경로 상에 장애물이 있는지 확인하는 함수
    bool IsPathBlocked(Vector3 from, Vector3 to)
    {
        RaycastHit hit;
        Vector3 direction = (to - from).normalized;
        float distance = 0.4f;

       // Debug.DrawRay(from, direction * distance, Color.black, 0.1f); // 0.1초 동안 검은색 레이 표시


        if (Physics.Raycast(from, direction, out hit, distance))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                return true; // 장애물이 경로 상에 있음
            }
        }
        return false; // 장애물 없음
    }

    //ischasing 중일 때만 진행방향의 앞칸의 아래로 ray, 그린존인지 확인




    // 애니메이션 변경
    void UpdateAnimation()
    {
        if (isAnimationLocked) return;

        Vector3 velocity = agent.velocity;
        if(isAttack && notDizzy)
        {
            ChangeAnimationState("attack");
        }

        if (!isAttack && (Mathf.Abs(velocity.z) > Mathf.Abs(velocity.x)))
        {
            if (velocity.z > 0 && currentAnimation != "up_npc")
            {

                if (greenZoneAttack&&goInGreenZone)
                {
                    ChangeAnimationState("defend_down");
                    Debug.Log("어택후 아래 애니메이션");
                    goInGreenZone = false;
                    isAnimationLocked = true;
                    return;
                    //!!다음 해롱해롱으로 가기
                }

                else if (!greenZoneAttack)
                {
                    ChangeAnimationState("up_npc");
                    Debug.Log("위로 애니메이션");
                }

            }
            else if (velocity.z < 0 && currentAnimation != "down_npc")
            {
                if (greenZoneAttack&&goInGreenZone)
                {
                    ChangeAnimationState("defend_up");
                    goInGreenZone = false;
                    isAnimationLocked = true;
                    Debug.Log("어택후 위로 애니메이션");
                    return;
                    
                }
                else if (!greenZoneAttack)
                {
                    ChangeAnimationState("down_npc");
                    Debug.Log("아래로 애니메이션");
                }
            }
        }

        else
        {

            if (velocity.x > 0 && currentAnimation != "right_npc")
            {
                if (greenZoneAttack&&goInGreenZone)
                {
                    ChangeAnimationState("defend_left");
                    goInGreenZone = false;
                    isAnimationLocked = true;
                    Debug.Log("어택후 왼쪽 애니메이션");
                    return;
                }
                else if (!greenZoneAttack)
                {
                    Debug.Log("오른쪽으로 애니메이션");
                    ChangeAnimationState("right_npc");
                   
                }
            }
            else if (velocity.x < 0 && currentAnimation != "left_npc")
            {
                if (greenZoneAttack&&goInGreenZone)
                {
                    ChangeAnimationState("defend_right");
                    goInGreenZone = false;
                    isAnimationLocked = true;
                    Debug.Log("어택후 오른쪽 애니메이션");
                    return;
                }
                else if (!greenZoneAttack)
                {
                    ChangeAnimationState("left_npc");
                    Debug.Log("왼쪽으로 애니메이션");
                }
            }
        }

        
    }

    public void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;

        animator.Play(newAnimation);
        currentAnimation = newAnimation;
    }

    //공격 애니메이션 진입
   /* bool DetectForAttack()
    {
        // 레이캐스트 원점 (NPC 위치)
        Vector3 rayOrigin = transform.position + new Vector3(0, 0f, 0);

        // NPC의 현재 이동 방향에 따라 레이캐스트 방향을 설정
        Vector3 rayDirection = agent.velocity.normalized;

        if (rayDirection == Vector3.zero)
        {
            rayDirection = transform.forward; // 기본적으로 정면으로 설정
        }

        Ray ray = new Ray(rayOrigin, rayDirection);
        RaycastHit hit;

        // 1f 거리 안에 Asher가 있는지 확인
        if (Physics.Raycast(ray, out hit, attackDistance))
        {
            if (hit.collider.name == "Asher")
            {
                Debug.Log("Asher 감지됨! 공격 애니메이션 실행");
                ChangeAnimationState("attack");
                agent.isStopped = true;
                return true;
            }
        }
        return false;
    }*/


    // 회피 관련
   public void IsAttackOn()
    {
        asher.GetComponent<Player_Move>().isAttacked = true;
    }

   public void IsAttackClear()
    {
        ChangeAnimationState("up_npc");
   //     Debug.Log("실행");
        isAttack = false;
        //플레이어가 영역 안에 존재하면 죽음
        
        if (asher.GetComponent<Player_Move>().isAttacked)
        {
         //   Debug.Log("죽음");
            agent.isStopped = false;
            asher.GetComponent<Player_Move>().isAttacked = false;
        }
        else
        {
        //    Debug.Log("생존");
            asher.GetComponent<Player_Move>().isAttacked = false;
            
            Invoke("WakeUp", 3f);
        }
        
    }

    void WakeUp()
    {
        Debug.Log("깨어나다");
        isAnimationLocked = false;
        agent.isStopped = false;
        notDizzy = true;
    }
}