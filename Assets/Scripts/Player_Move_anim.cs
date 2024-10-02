using System.Collections;
using UnityEngine;

public class Player_Move_anim : MonoBehaviour
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

    public float gravity = 9.8f; // 중력 가속도 크기
    private Vector3 gravityDirection;

    public float jumpForce = 2f;
    private bool isGrounded = true;
    private bool isJump = false;
    private Rigidbody body;

    void Start()
    {
        targetPosition = SnapToGrid(transform.position);  // 시작 시 현재 위치를 목표 위치로 스냅
        startPosition = targetPosition;

        // Animator 및 Rigidbody 초기화
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();

        // 중력 방향
        gravityDirection = Quaternion.Euler(60, 0, 0) * Vector3.down;

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

        else if (Input.GetKey(KeyCode.Space) && isGrounded && body != null)
        {
            isGrounded = false;
            Debug.Log("점프 대기 중: " + isGrounded);
            ChangeAnimationState(PLAYER_JUMP);

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

                // 이동이 완료되면 idle 상태로 전환
                if (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
                {
                    ChangeAnimationState(PLAYER_IDLE);
                }
            }

        }

        if (!isGrounded && isJump)
        {
            // 중력 적용
            body.AddForce(gravityDirection * gravity, ForceMode.Acceleration);
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
        Vector3 jumpDirection = Quaternion.Euler(60, 0, 0) * Vector3.up;
        body.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);  // 60도 각도로 점프
        isJump = true;
        Debug.Log("애니메이션 이벤트로 점프");
    }

    void Idle()
    {
        ChangeAnimationState(PLAYER_IDLE);
        Debug.Log("아이들로");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // 바닥과 닿으면 점프 가능
        {
            isGrounded = true;
            isJump = false;
            body.velocity = new Vector3(0, 0, 0);

            Vector3 currentPosition = transform.position;
            currentPosition.z = Mathf.Floor(currentPosition.z) + 0.5f;
            transform.position = currentPosition;
            Debug.Log("땅에 닿음");
        }
    }
}