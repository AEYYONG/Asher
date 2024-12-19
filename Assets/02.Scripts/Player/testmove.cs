using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testmove : MonoBehaviour
{
    public float moveSpeed = 5f;  // 이동 속도
    public float moveDuration = 0.3f;  // 각 방향으로 이동하는 시간 (0.3초)
    private bool isMoving = false;  // 현재 이동 중인지 여부
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float moveStartTime;  // 이동 시작 시간

    private string currentAnimation;
    private Animator animator;
    private bool isReadyPlaying = false;  // 준비 애니메이션이 재생 중인지 여부
    private bool isGoPlaying = false;  // Go 애니메이션이 재생 중인지 여부

    const string PLAYER_IDLE = "idle";
    const string PLAYER_UP_READY = "test_up_ready";
    const string PLAYER_UP_GO = "test_up_go";
    const string PLAYER_UP_STOP = "test_up_stop";

    void Start()
    {
        targetPosition = SnapToGrid(transform.position);  // 시작 시 현재 위치를 목표 위치로 스냅
        startPosition = targetPosition;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isMoving)
        {
            HandleMovement();  // 이동 중이 아닐 때만 입력을 처리
        }

        // W 키를 떼었을 때 Stop 애니메이션 -> Idle 애니메이션
        if (isGoPlaying && Input.GetKeyUp(KeyCode.W))
        {
            StartCoroutine(PlayStopAndIdle());  // Stop 애니메이션 재생 후 Idle 전환
        }

        if (isMoving)
        {
            MovePlayer();  // 이동 처리
        }
    }

    void HandleMovement()
    {
        Vector3 movement = Vector3.zero;

        // W 키를 처음 눌렀을 때
        if (Input.GetKeyDown(KeyCode.W) && !isReadyPlaying && !isGoPlaying)
        {
            movement = Vector3.forward;
            StartCoroutine(PlayReadyAndGo(movement));  // Ready -> Go 애니메이션 재생 후 이동 시작
        }
        else if (Input.GetKey(KeyCode.D))
        {
            movement = Vector3.right;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movement = Vector3.left;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movement = Vector3.back;
        }
        else if (!isMoving && !Input.GetKey(KeyCode.W))
        {
            ChangeAnimationState(PLAYER_IDLE);  // W키 입력이 없으면 Idle 애니메이션 재생
        }

        if (movement != Vector3.zero && !isReadyPlaying && !isMoving)
        {
            startPosition = transform.position;  // 현재 위치 저장
            targetPosition = SnapToGrid(transform.position + movement);  // 목표 위치 계산
        }
    }

    void MovePlayer()
    {
        if (isMoving)
        {
            // 이동 진행 비율 계산 (0.3초 동안 이동)
            float t = (Time.time - moveStartTime) / moveDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // 이동 완료 시 처리
            if (t >= 1f)
            {
                isMoving = false;
                transform.position = targetPosition;
            }
        }
    }

    // 목표 좌표를 정수 좌표로 스냅하면서 Z축을 항상 .5로 맞추는 함수
    Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), position.y, Mathf.Floor(position.z) + 0.5f);
    }

    // Ready -> Go 애니메이션 재생 후 이동 처리
    IEnumerator PlayReadyAndGo(Vector3 movement)
    {
        isReadyPlaying = true;

        // Ready 애니메이션 재생
        ChangeAnimationState(PLAYER_UP_READY);

        // Ready 애니메이션이 끝날 때까지 대기 (0.5초)
        yield return new WaitForSeconds(0.5f);

        // Go 애니메이션 재생 및 이동 시작
        ChangeAnimationState(PLAYER_UP_GO);
        moveStartTime = Time.time;
        isMoving = true;
        isGoPlaying = true;
        isReadyPlaying = false;
    }

    // Stop 애니메이션 재생 후 Idle 상태로 전환
    IEnumerator PlayStopAndIdle()
    {
        isGoPlaying = false;

        // Stop 애니메이션 재생
        ChangeAnimationState(PLAYER_UP_STOP);

        // Stop 애니메이션이 끝날 때까지 대기 (0.5초)
        yield return new WaitForSeconds(0.5f);

        // Idle 애니메이션으로 전환
        ChangeAnimationState(PLAYER_IDLE);
    }

    // 애니메이션 상태 변경 함수
    void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;

        animator.Play(newAnimation);
        currentAnimation = newAnimation;
    }
}
