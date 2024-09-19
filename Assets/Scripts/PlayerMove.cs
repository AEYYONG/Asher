using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float jumpForce = 5f; // 점프의 힘
    public float inputCooldown = 0.3f; // 입력 쿨다운 시간
    private bool isGrounded = true;
    private float nextInputTime = 0f; // 다음 입력을 받을 수 있는 시간
    private Rigidbody rb;

    Animator animator;
    string animState = "Move";

    // 애니메이션 상태
    enum States
    {
        idle = 0,
        right = 1,
        left = 2,
        up = 3,
        down = 4,
        jump = 5
    }

    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        UpdateState();
    }

    void FixedUpdate()
    {
        // 쿨다운이 끝나지 않았으면 입력을 무시
        if (Time.time < nextInputTime)
            return;

        // 점프 처리
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {

            isGrounded = false;
            Debug.Log("점프불가: " + isGrounded);
            // animator.SetInteger(animState, (int)States.jump);
            nextInputTime = Time.time + inputCooldown; // 쿨다운 설정
        }

        // WASD 이동 처리 (1씩 이동, 월드 좌표계 기준)
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right, Space.World); // 월드 좌표계 기준 오른쪽으로 1 단위 이동
            Debug.Log("현재 위치: " + transform.position);
            nextInputTime = Time.time + inputCooldown; // 쿨다운 설정
        }
        else if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward, Space.World); // 월드 좌표계 기준 앞으로 1 단위 이동 (Z축)
            Debug.Log("현재 위치: " + transform.position);
            nextInputTime = Time.time + inputCooldown; // 쿨다운 설정
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back, Space.World); // 월드 좌표계 기준 뒤로 1 단위 이동
            Debug.Log("현재 위치: " + transform.position);
            //animator.SetInteger(animState, (int)States.down);
            nextInputTime = Time.time + inputCooldown; // 쿨다운 설정
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left, Space.World); // 월드 좌표계 기준 왼쪽으로 1 단위 이동
            Debug.Log("현재 위치: " + transform.position);
            nextInputTime = Time.time + inputCooldown; // 쿨다운 설정
        }

        else
        {
            animator.SetInteger(animState, (int)States.idle);
        }
    }

    private void UpdateState()
    {
        if (Input.GetKey(KeyCode.D))
        {
            animator.SetInteger(animState, (int)States.right);
        }

        else if (Input.GetKey(KeyCode.A))
        {
            animator.SetInteger(animState, (int)States.left);
        }

        else if (Input.GetKey(KeyCode.W))
        {
            animator.SetInteger(animState, (int)States.up);
        }

        else if (Input.GetKey(KeyCode.S))
        {
            animator.SetInteger(animState, (int)States.down);
        }

        else if (Input.GetKey(KeyCode.Space))
        {
            animator.SetInteger(animState, (int)States.jump);
        }

    }

    void State_Idle()
    {
        animator.SetInteger(animState, (int)States.idle);
        Debug.Log("idle로");
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