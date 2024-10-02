using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;  // 이동 속도
    public float moveDuration = 0.3f;  // 각 방향으로 이동하는 시간 (0.3초)
    private bool isMoving = false;  // 현재 이동 중인지 여부
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

    public float jumpForce = 5f;
    private bool isGrounded = true;
    private Rigidbody body;

    void Start()
    {
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
        if (!isMoving)
        {
            HandleMovement();  // 이동 중이 아닐 때만 입력을 처리
        }
        MovePlayer();  // 이동 처리
    }

    void HandleMovement()
    {
        Vector3 movement = Vector3.zero;
        //해야할 것 : https://unialgames.tistory.com/entry/Unity-State-Machine-Behaviour 링크를 보고 idle 상태 이동 애니메이션 로직 다시 짜기
        // 60,0,0 방향으로 중력 생성해서 자체 중력으로 움직임
        // npc 움직임 좀 더 랜덤으로 지금 너무 좌우로만 움직이는 것 같음 속도 낮춰서 다음 타겟이 어떻게 생서되는지 확인 필요
        // WASD 입력에 따라 목표 위치 설정 (각 방향으로 1만큼 이동)
        if (Input.GetKey(KeyCode.D))
        {
            movement = Vector3.right;
            ChangeAnimationState(PLAYER_RIGHT);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movement = Vector3.left;
            ChangeAnimationState(PLAYER_LEFT);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            movement = Vector3.forward;
            ChangeAnimationState(PLAYER_UP);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movement = Vector3.back; // 음의 Z축으로 이동
            ChangeAnimationState(PLAYER_DOWN);
        }

        // X방향으로 60도 각도로 점프
        else if (Input.GetKey(KeyCode.Space) && isGrounded && body != null)
        {
            isGrounded = false;
            Debug.Log("점프: " + isGrounded);
            ChangeAnimationState(PLAYER_JUMP);

            // 60도 각도로 점프하는 벡터 계산
            Vector3 jumpDirection = Quaternion.Euler(60, 0, 0) * Vector3.up;
            body.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);  // 60도 각도로 점프

            // 점프 후 idle로 전환하기 위한 코루틴 시작
            StartCoroutine(JumpToIdleCoroutine());
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

        animator.Play(newAnimation);
        currentAnimaton = newAnimation;
    }

    // 코루틴: 점프 후 0.2초 기다렸다가 idle로 전환
    IEnumerator JumpToIdleCoroutine()
    {
        // 애니메이션 재생이 끝날 때까지 대기 (만약 애니메이션이 1.0초라면 1.0초 대기)
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 점프 애니메이션이 끝날 때까지 대기
        while (stateInfo.IsName(PLAYER_JUMP) && stateInfo.normalizedTime < 1.0f)
        {
            yield return null;  // 애니메이션이 끝날 때까지 기다림
        }

        yield return new WaitForSeconds(0.2f);  // 0.2초 대기

        ChangeAnimationState(PLAYER_IDLE);  // idle 상태로 전환
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // 바닥과 닿으면 점프 가능
        {
            isGrounded = true;
            Debug.Log("IsGrounded: " + isGrounded);
        }
    }
}
