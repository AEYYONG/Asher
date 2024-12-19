using System.Collections;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    public float moveSpeed = 5f;  // 이동 속도
    public float moveDuration = 0.2f;  // 각 방향으로 이동하는 시간 (0.2초)
    private bool isMoving = false;  // 현재 이동 중인지 여부
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float moveStartTime;  // 이동 시작 시간

    private string currentAnimaton;
    private Animator animator;

    const string PLAYER_IDLE = "idle";
    const string PLAYER_RIGHT = "right";
    const string PLAYER_LEFT = "left";
    const string PLAYER_UP = "up_ready";
    const string PLAYER_DOWN = "down";
    const string PLAYER_JUMP = "jump";
    const string PLAYER_UP_GO = "up_go";  // up_go 애니메이션 상수 추가

    private bool isGo = false;

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
    }

    void Update()
    {
        if (!isMoving)
        {
            HandleMovement();  // 이동 중이 아닐 때만 입력을 처리
        }

        // up_go 애니메이션이 재생 중일 때만 이동
        if (IsPlayingAnimation(PLAYER_UP_GO))
        {
            MovePlayer();  // 이동 처리
        }
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
            ChangeAnimationState(PLAYER_UP);  // up_ready 애니메이션 실행
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movement = Vector3.back; // 음의 Z축으로 이동
            ChangeAnimationState(PLAYER_DOWN);
        }
        else if (Input.GetKey(KeyCode.Space) && isGrounded && body != null)
        {
            isGrounded = false;
            ChangeAnimationState(PLAYER_JUMP);
        }

        if (movement != Vector3.zero)
        {
            startPosition = transform.position;  // 현재 위치 저장
            targetPosition = SnapToGrid(transform.position + movement);  // 목표 위치 계산
            moveStartTime = Time.time;  // 이동 시작 시간 기록
            isMoving = true;  // 이동 중 상태로 변경
        }
    }

    void Go()
    {
        isGo = true;
        ChangeAnimationState(PLAYER_UP_GO);  // up_go 애니메이션으로 변경
    }

    void Stop()
    {
        if (!Input.GetKey(KeyCode.W))
        {
            isGo = false;
            ChangeAnimationState(PLAYER_IDLE);  // idle 애니메이션으로 변경
        }
    }

    void MovePlayer()
    {
        if (isMoving)
        {
            float t = (Time.time - moveStartTime) / moveDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            if (t >= 1f)
            {
                isMoving = false;  // 이동 종료
                transform.position = targetPosition;  // 최종 위치 설정
            }
        }
    }

    // 애니메이션이 재생 중인지 확인하는 함수
    bool IsPlayingAnimation(string animationName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName);
    }

    Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), position.y, Mathf.Floor(position.z) + 0.5f);
    }

    void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimaton == newAnimation) return;

        animator.Play(newAnimation, -1, 0f);
        currentAnimaton = newAnimation;
    }

    public void Jump()
    {
        Vector3 jumpDirection = Quaternion.Euler(60, 0, 0) * Vector3.up;
        body.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
        isJump = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJump = false;
            body.velocity = Vector3.zero;

            Vector3 currentPosition = transform.position;
            currentPosition.z = Mathf.Floor(currentPosition.z) + 0.5f;
            transform.position = currentPosition;
        }
    }
}