using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float jumpForce = 5f; // ������ ��
    public float inputCooldown = 0.3f; // �Է� ��ٿ� �ð�
    private bool isGrounded = true;
    private float nextInputTime = 0f; // ���� �Է��� ���� �� �ִ� �ð�
    private Rigidbody rb;

    Animator animator;
    string animState = "Move";

    // �ִϸ��̼� ����
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
        // ��ٿ��� ������ �ʾ����� �Է��� ����
        if (Time.time < nextInputTime)
            return;

        // ���� ó��
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {

            isGrounded = false;
            Debug.Log("�����Ұ�: " + isGrounded);
            // animator.SetInteger(animState, (int)States.jump);
            nextInputTime = Time.time + inputCooldown; // ��ٿ� ����
        }

        // WASD �̵� ó�� (1�� �̵�, ���� ��ǥ�� ����)
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right, Space.World); // ���� ��ǥ�� ���� ���������� 1 ���� �̵�
            Debug.Log("���� ��ġ: " + transform.position);
            nextInputTime = Time.time + inputCooldown; // ��ٿ� ����
        }
        else if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward, Space.World); // ���� ��ǥ�� ���� ������ 1 ���� �̵� (Z��)
            Debug.Log("���� ��ġ: " + transform.position);
            nextInputTime = Time.time + inputCooldown; // ��ٿ� ����
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back, Space.World); // ���� ��ǥ�� ���� �ڷ� 1 ���� �̵�
            Debug.Log("���� ��ġ: " + transform.position);
            //animator.SetInteger(animState, (int)States.down);
            nextInputTime = Time.time + inputCooldown; // ��ٿ� ����
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left, Space.World); // ���� ��ǥ�� ���� �������� 1 ���� �̵�
            Debug.Log("���� ��ġ: " + transform.position);
            nextInputTime = Time.time + inputCooldown; // ��ٿ� ����
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
        Debug.Log("idle��");
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // �ٴڰ� ������ ���� ����
        {
            isGrounded = true;
            Debug.Log("IsGrounded: " + isGrounded);
        }
    }
}