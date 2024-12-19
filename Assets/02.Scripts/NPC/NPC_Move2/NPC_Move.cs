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
    public bool AttackAnim = false;
    public bool isAsher = false;
    private bool isAttackAnimationPlaying = false;
    public bool safe = false;

    // 그린존 감지 관련 // ischasing일 때로 통일해도 될 듯
    public bool goInGreenZone = false;
    public float greenZoneDistance = 1.2f;  // 그린존 감지 범위
    public bool greenZoneAttack = false;
    public bool isAnimationLocked = false;
    private bool notDizzy = true;

    // MotionTrail 관련
    private MotionTrail motionTrail; // MotionTrail 스크립트
    private bool isTrailActive = false;


    // Start is called before the first frame update
    void Start()
    {
        agent.updateRotation = false;
        asher = GameObject.Find("Asher");
        motionTrail = GetComponent<MotionTrail>();
        SetRandomDestination();
    }

    // Update is called once per frame
    void Update()
    {
        DetectInFront();
        if (isChasing)
        {
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
        if (!notDizzy)
        {
            return;
        }
        else
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
                 //   Debug.Log("Asher가 공격 범위 내에 있습니다! 공격 시작");
                    isAttack = true;
                    asher.GetComponent<Player_Move>().isAttacked = true;
                    isAsher = true;
                    // 공격 애니메이션 실행 (필요시)
                    if (!AttackAnim)
                    {
                        Debug.Log("공격 시작");
                        AttackAnim = true;

                        
                    }

                    
                }
                else
                {
                    Debug.Log("Asher가 아님");
                    isAsher = false;
                    
                }

            }
            else
            {
                // 레이캐스트가 아무것도 감지하지 못했을 경우
                isAttack = false;
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
                    if (!isChasing)
                    {
                        StartCoroutine(ActivateTrailForDuration(5f));
                    }
                    isChasing = true;
                    agent.speed = 4;
                    
                  
                   // Invoke("ResetSpeed", 3f);
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
    }


    // MotionTrail을 일정 시간 동안 활성화
    private IEnumerator ActivateTrailForDuration(float duration)
    {
        agent.speed = 5;
        Debug.Log("스피드 5");
        if (motionTrail != null && !isTrailActive)
        {
            motionTrail.StartTrail(!greenZoneAttack);
            isTrailActive = true;
        }

        yield return new WaitForSeconds(duration);

       
        
        if (motionTrail != null && isTrailActive)
        {
            motionTrail.StopTrail();
            isTrailActive = false;
            ResetSpeed();
            Debug.Log("스피드 2.5");
        }
    }


    public void Dizzy()
    {

        if (currentAnimation == "defend_up")
        {
            ChangeAnimationState("up_dizzy");
        }
        else if (currentAnimation == "defend_left")
        {
            ChangeAnimationState("left_dizzy");
        }
        else if (currentAnimation == "defend_right")
        {
            ChangeAnimationState("right_dizzy");
        }
        else if (currentAnimation == "defend_down")
        {
            ChangeAnimationState("dizzy");
        }
        else
        {
            ChangeAnimationState("dizzy"); // 기본 상태
        }
        notDizzy = false;
        isAnimationLocked = true;
        agent.isStopped = true;
        isChasing = false;
        ResetSpeed();
        greenZoneAttack = false;
        Invoke("WakeUp",3f);
    }

    public void Attack()
    {
        
        if(currentAnimation == "up_npc")
        {
            Debug.Log("어택 위");
            ChangeAnimationState("attack_up");
            agent.isStopped = true;
        }
        else if (currentAnimation == "down_npc")
        {
            Debug.Log("어택아래");

            ChangeAnimationState("attack_down");
            agent.isStopped = true;
        }
        else if (currentAnimation == "left_npc")
        {
            Debug.Log("어택 왼쪽");
            
            ChangeAnimationState("attack_left");
            agent.isStopped = true;

        }
        else if (currentAnimation == "right_npc")
        {
            Debug.Log("어택 오른쪽");
            
            ChangeAnimationState("attack_right");
            agent.isStopped = true;

        }
    }

    // 잡은 경우에 애니메이션 중지 넣어야 할 것


    
    void ResetSpeed()
    {
        if (isChasing)
        {
           // agent.speed = 2.5f;
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
        Collider[] hitColliders = Physics.OverlapSphere(position, 0.001f);

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

    //ischasing 중일 때만 진행방향의 앞칸의 아래로 ray, 그린존인지 확인

    Vector3 previousVelocity;


    // 애니메이션 변경
    void UpdateAnimation()
    {
        if (isAttack)
        {
            Debug.Log($"공격 상태로 애니메이션 유지 - 현재 애니메이션: {currentAnimation}");
            Attack();
            return;
        }

        if (isAnimationLocked) return;

        Vector3 velocity = agent.velocity;
        if (Vector3.Distance(previousVelocity, velocity) < 0.1f)
        {
            velocity = previousVelocity; // 갑작스러운 변화 무시
        }
        previousVelocity = velocity;


        if (!isAttack && (Mathf.Abs(velocity.z) >= Mathf.Abs(velocity.x)))
        {
            if (velocity.z > 0 && currentAnimation != "up_npc")
            {

                if (greenZoneAttack&&goInGreenZone)
                {
                    ChangeAnimationState("defend_down");
                  //  Debug.Log("어택후 아래 애니메이션");
                    goInGreenZone = false;
                    isAnimationLocked = true;
                    return;
                    //!!다음 해롱해롱으로 가기
                }

                else if (!greenZoneAttack)
                {
                    ChangeAnimationState("up_npc");
                 //   Debug.Log("위로 애니메이션");
                }

            }
            else if (velocity.z <= 0 && currentAnimation != "down_npc")
            {
                if (greenZoneAttack&&goInGreenZone)
                {
                    ChangeAnimationState("defend_up");
                    goInGreenZone = false;
                    isAnimationLocked = true;
                  //  Debug.Log("어택후 위로 애니메이션");
                    return;
                    
                }
                else if (!greenZoneAttack)
                {
                    ChangeAnimationState("down_npc");
                 //   Debug.Log("아래로 애니메이션");
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
                  //  Debug.Log("어택후 왼쪽 애니메이션");
                    return;
                }
                else if (!greenZoneAttack)
                {
                    //Debug.Log("오른쪽으로 애니메이션");
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
                  //  Debug.Log("어택후 오른쪽 애니메이션");
                    return;
                }
                else if (!greenZoneAttack)
                {
                    ChangeAnimationState("left_npc");
                  //  Debug.Log("왼쪽으로 애니메이션");
                }
            }
        }
        if (isAttack && notDizzy)
        {
            Debug.Log($"공격 실행 조건 - 현재 애니메이션: {currentAnimation}");
            Attack();
            
        }


    }

    public void ChangeAnimationState(string newAnimation)
    {
        // 현재 애니메이션 상태 정보 가져오기
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // "attack_"으로 시작하는 애니메이션은 재생 중에는 변경 불가
        if (stateInfo.IsName(currentAnimation) && currentAnimation.StartsWith("attack_"))
        {
            if (newAnimation.StartsWith("dizzy2"))
            {
                Debug.Log($"애니메이션 변경: {currentAnimation} -> {newAnimation}");
                animator.Play(newAnimation);
                currentAnimation = newAnimation;
                return;
            }
            else if (stateInfo.normalizedTime < 1.0f)
            {
            //    Debug.Log($"현재 애니메이션: {currentAnimation}, 변경 요청: {newAnimation} -> 변경 차단");
                return; // 애니메이션이 끝나지 않았으므로 상태 변경 차단
            }
        }

        // 애니메이션 변경
        Debug.Log($"애니메이션 변경: {currentAnimation} -> {newAnimation}");
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
        if (!agent.isStopped)
        {
           //
           //agent.isStopped = true;
            Debug.Log("공격 애니메이션 진입");
        }
        
    }

   public void IsAttackSuccess()
    {
        if (asher.GetComponent<Player_Move>().isAttacked && !StageManager.Instance.isGameOver)
        {
            if (!isAsher)
            {// 공격 거리에서 피함
                Debug.Log("피해서 생존");
                
                asher.GetComponent<Player_Move>().isAttacked = false;
                isAttack = false;
            }
            else
            {
                Debug.Log("죽음");
                
                asher.GetComponent<Player_Move>().isAttacked = false;
                isAttack = false;
                StartCoroutine(StageManager.Instance.GameOver());
            }
        }

        else
        {

            Debug.Log("생존");
            asher.GetComponent<Player_Move>().isAttacked = false;
            isAttack = false;

            // 만약 회피 눌렀으면 3초 정지, 아니면 바로 움직임
            ChangeAnimationState("dizzy2");
            isAnimationLocked = true; // 애니메이션 변경 잠금
            safe = true;
            Invoke("WakeUp", 2f);
        }
    }

   public void IsAttackClear()
    {
        Debug.Log("멈추면 안되는데");
        //     Debug.Log("실행");

        //플레이어가 회피하지 못한 경우에만 움직임
        if (!safe)
        {
            agent.isStopped = false;
        }

    }

    void WakeUp()
    {
        Debug.Log("깨어나다");
        isAnimationLocked = false;
        agent.isStopped = false;
        notDizzy = true;
        AttackAnim = false;
        safe = false;
    }
}