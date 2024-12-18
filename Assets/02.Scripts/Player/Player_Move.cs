using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    //시작
    public bool isStart = false;

    public float moveSpeed = 5f;  // 이동 속도
    public float moveDuration = 0.3f;  // 각 방향으로 이동하는 시간 (0.3초)
    public bool isMoving = false;  // 현재 이동 중인지 여부
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float moveStartTime;  // 이동 시작 시간

    private string currentAnimaton;
    private Animator animator;

    const string PLAYER_IDLE = "idle";
    const string PLAYER_RIGHT = "right";
    const string PLAYER_LEFT = "left";
    const string PLAYER_UP = "up";
    const string PLAYER_DOWN = "down";
    const string PLAYER_JUMP = "jump";
    const string PLAYER_SLIP = "slip";

    private Rigidbody body;


    // 충돌 상태를 저장하는 변수
    private bool isColliding = false;

    //회피 관련
    public bool isAttacked = false;
    private bool isDodge = false;

    //점프 시작했는지
    public bool startJump = false;

    //혼란 함정 flag
    public bool isInverse = false;

    public bool isSlip = false;
    private Vector3 randomDirection;
    private bool slipping = false;

    // 헤어볼
    public GameObject HairBall;
    private int direction = -1;
    public bool isfire = false;

    // LineRenderer 관련
    public LineRendererAtoB lineRendererPrefab; // LineRenderer 프리팹
    private List<LineRendererAtoB> activeLines = new List<LineRendererAtoB>();
    public float lineLength = 5f; // 라인의 길이
    public LayerMask obstacleLayer; // 장애물 레이어
    public bool useBall = false;




    void Start()
    {
        // "Obstacle" 레이어를 LayerMask로 가져오기
        obstacleLayer = LayerMask.GetMask("Obstacle");


        targetPosition = SnapToGrid(transform.position);  // 시작 시 현재 위치를 목표 위치로 스냅
        startPosition = targetPosition;

        // Animator 및 Rigidbody 초기화
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();



        // Rigidbody 또는 Animator가 없을 경우 경고 로그 출력
        if (animator == null)
        {
            Debug.LogWarning("Animator 컴포넌트가 없습니다!");
        }

        if (body == null)
        {
            Debug.LogWarning("Rigidbody 컴포넌트가 없습니다!");
        }
    }

    void Update()
    {
        if (isSlip)
        {
            Slip();
        }
        else if(!isStart){
            return;
        }
        else MovePlayer();  // 이동 처리

        if (!isMoving)
        {
            HandleMovement();  // 이동 중이 아닐 때만 입력을 처리
        }

        if (useBall)
        {
            CreateDirectionalLines();
        }

        if (!useBall)
        {
            ClearLines();
        }


        SpaceInput();
   

    }

    void CreateDirectionalLines()
    {
        // 기존 라인을 모두 삭제
        ClearLines();

        // 4방향 정의
        Vector3[] directions = new Vector3[]
        {
        Vector3.forward,  
        Vector3.back,
        Vector3.left,
        Vector3.right
        };

        // 현재 위치 기준으로 라인 생성
        foreach (Vector3 direction in directions)
        {
            Vector3 from = transform.position + direction * 0.3f;
            from.z -= 0.2f; // z축 높이 조정 (플레이어의 중심에서 시작)

            Vector3 to = CalculateEndPoint(from, direction);

            // LineRenderer 프리팹 인스턴스 생성
            LineRendererAtoB line = Instantiate(lineRendererPrefab, transform);
            line.Play(from, to);

            // 활성 라인 리스트에 추가
            activeLines.Add(line);
        }
    }




    void ClearLines()
    {
        foreach (var line in activeLines)
        {
            line.Stop();
            Destroy(line.gameObject);
        }
        activeLines.Clear();
    }

    Vector3 CalculateEndPoint(Vector3 from, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(from, direction, out hit, lineLength, obstacleLayer))
        {
            Debug.Log($"Hit Detected for Direction: {direction}, Hit Point: {hit.point}");
            return hit.point; // 장애물에 닿은 경우
        }
        else
        {
            Debug.Log($"No Hit Detected for Direction: {direction}, Calculated End: {from + direction * lineLength}");
            return from + direction * lineLength; // 최대 거리까지
        }
    }



    public void StartFire(int newDirection)
    {
        direction = newDirection;
        isfire = true;
        Fire();
    }

    void Fire()
    {

        Vector3 spawnPosition = transform.position + new Vector3(0, -0.3f, -0.5f);
        GameObject HairballItem = Instantiate(HairBall, spawnPosition, transform.rotation);
        Rigidbody rigid = HairballItem.GetComponent<Rigidbody>();

        //누르고있는 방향에 따라 다르게 발사
        switch (direction)
        {
            case -1:
                return;
            case 0:
                useBall = false;
                rigid.AddForce(Vector3.forward * 1, ForceMode.Impulse);
                break;
            case 1:
                useBall = false;
                rigid.AddForce(Vector3.left * 2, ForceMode.Impulse);
                break;
            case 2:
                useBall = false;
                rigid.AddForce(Vector3.back * 1, ForceMode.Impulse);
                break;
            case 3:
                useBall = false;
                rigid.AddForce(Vector3.right * 2, ForceMode.Impulse);
                break;
            
        }
        /*if (direction == 0)
        {//위
            rigid.AddForce(Vector3.forward);
        }*/
       
    }

    public void StartSlip()
    {
        int randomDirectionIndex = Random.Range(0, 4);  // 0에서 3 사이의 정수 생성
        switch (randomDirectionIndex)
        {
            case 0:
                randomDirection = Vector3.left;
                Debug.Log("미끄러짐 시작! 방향: 왼쪽");
                break;
            case 1:
                randomDirection = Vector3.right;
                Debug.Log("미끄러짐 시작! 방향: 오른쪽");
                break;
            case 2:
                randomDirection = Vector3.forward;
                Debug.Log("미끄러짐 시작! 방향: 위");
                break;
            case 3:
                randomDirection = Vector3.back;
                Debug.Log("미끄러짐 시작! 방향: 아래");
                break;
        }
        isSlip = true; 
        isMoving = false;
        slipping = true;
    }

    void Slip()
    {
        // 미끄러짐 이동 처리
        transform.position += randomDirection * moveSpeed * Time.deltaTime;
        // 애니메이션 slip으로
        ChangeAnimationState(PLAYER_SLIP);

        // 장애물이 감지되면 미끄러짐 중지
        if (IsObstacleInSlip(randomDirection))
        {
            isSlip = false;  // isSlip을 false로 설정하여 미끄러짐 중지
            isMoving = true;
            Debug.Log("미끄러짐 종료 - 장애물에 부딪힘");

            // 목표 위치 설정 및 애니메이션 업데이트
            targetPosition = transform.position;
            targetPosition = SnapToGrid(transform.position);
            ChangeAnimationState(PLAYER_IDLE);
        }
    }



    void SpaceInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("flase 스타트 점프 : " + startJump);
            Debug.Log(" flase 어택당했는지 : " + isAttacked);
        }


    }

    bool IsObstacleInSlip(Vector3 localDirection)
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position; // 레이의 시작점
        rayOrigin.y -= 0.2f;
        rayOrigin.z -= 0.3f;
        float rayDistance = 0.5f; // 레이 거리
        Vector3 rayDirection = transform.TransformDirection(localDirection);

        // 레이를 쏴서 해당 방향에 장애물이 있는지 확인
        if (Physics.Raycast(rayOrigin, localDirection, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("NPC"))
            {
                Debug.Log("장애물 감지: " + hit.collider.name);
                isColliding = true;

                return true; // 장애물이 있을 때
            }
        }
        isColliding = false;
        return false; // 장애물이 없을 때
    }
    bool IsObstacleInDirection(Vector3 localDirection)
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position; // 레이의 시작점
        rayOrigin.y -= 0.2f;
        rayOrigin.z -= 0.5f;
        float rayDistance = 0.9f; // 레이 거리
        Vector3 rayDirection = transform.TransformDirection(localDirection);

        // 레이를 쏴서 해당 방향에 장애물이 있는지 확인
        if (Physics.Raycast(rayOrigin, localDirection, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("NPC"))
            {
                Debug.Log("장애물 감지: " + hit.collider.name);
                isColliding = true;

                return true; // 장애물이 있을 때
            }
        }
        isColliding = false;
        return false; // 장애물이 없을 때
    }

    void HandleMovement()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.D))
        {
            if (!isInverse)
            {
                if (!IsObstacleInDirection(Vector3.right))
                {
                    movement = Vector3.right;

                }
                ChangeAnimationState(PLAYER_RIGHT);
            }
            else
            {
                if (!IsObstacleInDirection(Vector3.left))
                {
                    movement = Vector3.left;
                }
                ChangeAnimationState(PLAYER_LEFT);
            }
        }
        else if (Input.GetKey(KeyCode.A))
        {
            if (!isInverse)
            {
                if (!IsObstacleInDirection(Vector3.left))
                {
                    movement = Vector3.left;
                }
                ChangeAnimationState(PLAYER_LEFT);
            }
            else
            {
                if (!IsObstacleInDirection(Vector3.right))
                {
                    movement = Vector3.right;

                }
                ChangeAnimationState(PLAYER_RIGHT);
            }
        }
        else if (Input.GetKey(KeyCode.W))
        {
            if (!isInverse)
            {
                if (!IsObstacleInDirection(Vector3.forward))
                {
                    movement = Vector3.forward;

                }
                ChangeAnimationState(PLAYER_UP);
            }
            else
            {
                if (!IsObstacleInDirection(Vector3.back))
                {
                    movement = Vector3.back; // 음의 Z축으로 이동
                }
                ChangeAnimationState(PLAYER_DOWN);
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (!isInverse)
            {
                if (!IsObstacleInDirection(Vector3.back))
                {
                    movement = Vector3.back; // 음의 Z축으로 이동
                }
                ChangeAnimationState(PLAYER_DOWN);
            }
            else
            {
                if (!IsObstacleInDirection(Vector3.forward))
                {
                    movement = Vector3.forward;

                }
                ChangeAnimationState(PLAYER_UP);
            }
        }
        else if (Input.GetKey(KeyCode.Space) && !isAttacked)
        {
            startJump = true;
            ChangeAnimationState(PLAYER_JUMP);  // 점프 애니메이션 실행
        }


        else if ((Input.GetKey(KeyCode.RightShift)||Input.GetKey(KeyCode.LeftShift)))
        {
            
            if (isAttacked && !isDodge)
            {
                isAttacked = false;
                ChangeAnimationState(PLAYER_JUMP); // 임시로 점프 애니메이션 실행
                Debug.Log("회피 성공!");
                Invoke("Dodge", 2f);
            }
            else if (!isDodge)
            {
                ChangeAnimationState(PLAYER_JUMP); // 임시로 점프 애니메이션 실행
                Debug.Log("회피만!");
                Invoke("Dodge", 2f);
            }
            isDodge = true;

        }
        else if (!Input.anyKey && !startJump)
        {
            ChangeAnimationState(PLAYER_IDLE);
        }

        if (movement != Vector3.zero)
        {
            // 현재 위치와 목표 위치 사이의 거리가 일정 이상일 때만 이동 시작
            startPosition = transform.position;  // 현재 위치 저장
            targetPosition = SnapToGrid(transform.position + movement);  // 목표 위치 계산
            moveStartTime = Time.time;  // 이동 시작 시간 기록
            isMoving = true;  // 이동 중 상태로 변경
        }
    }

    void Dodge()
    {
        isDodge = false;
        Debug.Log("회피 쿨타임 종료!");
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // 레이 시작점과 방향
            Vector3 rayOrigin = transform.position;

            // 오른쪽으로 레이 그리기
            DrawRay(rayOrigin, Vector3.right, 0.9f);

            // 왼쪽으로 레이 그리기
            DrawRay(rayOrigin, Vector3.left, 0.9f);

            // 앞쪽으로 레이 그리기 (위쪽)
            DrawRay(rayOrigin, Vector3.forward, 0.9f);

            // 뒤쪽으로 레이 그리기 (아래쪽)
            DrawRay(rayOrigin, Vector3.back, 0.9f);
        }
    }

    // 주어진 방향으로 레이를 그리는 함수
    void DrawRay(Vector3 origin, Vector3 direction, float distance)
    {
        Gizmos.color = Color.red; // 레이 색상을 빨간색으로 설정
        Gizmos.DrawRay(origin, direction * distance); // 레이를 그립니다
    }
    void MovePlayer()
    {
        if (isMoving)
        {
            // 이동 진행 비율 계산 (0.3초 동안 이동)
            float t = (Time.time - moveStartTime) / moveDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // 이동이 완료되었을 때 처리
            if (t >= 1f)
            {
                isMoving = false;  // 이동 종료
                transform.position = targetPosition;  // 최종 위치 설정

                // 이동이 완료되면 idle 상태로 전환
                if (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
                {
                    ChangeAnimationState(PLAYER_IDLE);
                }
            }

        }


    }

    // 목표 좌표를 정수 좌표로 스냅하면서 Z축을 항상 .5로 맞추는 함수
    Vector3 SnapToGrid(Vector3 position)
    {
        // X축은 반올림, Z축은 정수로 만들고 + 0.5f로 항상 .5 유지
        return new Vector3(Mathf.Round(position.x), position.y, Mathf.Floor(position.z) + 0.5f);
    }

    void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimaton == newAnimation) return;

        animator.Play(newAnimation, -1, 0f);
        currentAnimaton = newAnimation;
    }

    // 애니메이션 이벤트에 의해 호출되는 함수
    public void Jump()
    {
        Debug.Log("점프 끝");
        startJump = false;
    }

    public void Idle()
    {
        ChangeAnimationState(PLAYER_IDLE);
        Debug.Log("아이들로");
    }

    //점프 중력 관련 보류
    void OnCollisionEnter(Collision collision)
    //점프 가능 판정(땅에 닿음) 어떻게 짤건지? -> 땅에 닿으면 startJump false로 변경해야 함(애니메이션 제어용)
    {
        /*   if (collision.gameObject.CompareTag("Ground")) // 바닥과 닿으면 점프 가능
           {
               startJump = false;
               body.velocity = new Vector3(0, 0, 0);
               Vector3 currentPosition = transform.position;
               currentPosition.z = Mathf.Floor(currentPosition.z) + 0.5f;
               transform.position = currentPosition;
           }*/


    }

}