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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // ��ٿ��� ������ �ʾ����� �Է��� ����
        if (Time.time < nextInputTime)
            return;

        // ���� ó��
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            Debug.Log("�����Ұ�: " + isGrounded);
            nextInputTime = Time.time + inputCooldown; // ��ٿ� ����
        }

        // WASD �̵� ó�� (1�� �̵�, ���� ��ǥ�� ����)
        if (Input.GetKey(KeyCode.D))
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
            nextInputTime = Time.time + inputCooldown; // ��ٿ� ����
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left, Space.World); // ���� ��ǥ�� ���� �������� 1 ���� �̵�
            Debug.Log("���� ��ġ: " + transform.position);
            nextInputTime = Time.time + inputCooldown; // ��ٿ� ����
        }
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
