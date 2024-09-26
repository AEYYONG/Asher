using System.Collections;
using System.Collections.Generic;
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
    const string PLAYER_RIGJHT = "right";
    const string PLAYER_LEFT = "left";
    const string PLAYER_UP = "up";
    const string PLAYER_DOWN = "down";
   // const string PLAYER_JUMP = "jump";

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
        MovePlayer();  // 이동 처리
    }

    void HandleMovement()
    {
        Vector3 movement = Vector3.zero;

        // WASD 입력에 따라 목표 위치 설정 (각 방향으로 1만큼 이동)
        if (Input.GetKey(KeyCode.D))
        {
            movement = Vector3.right;
            ChangeAnimationState(PLAYER_RIGJHT);
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

        else
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
}
