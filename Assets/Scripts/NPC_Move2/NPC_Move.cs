using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Move : MonoBehaviour
{
    public NavMeshAgent agent;
    private Vector3 targetPosition;
    private bool moveInXAxis = true;
    public float detectionRange = 2.0f;


    // �̵� ������ ���� ����
    public Vector3 minRange = new Vector3(0, 0, 0);  // �ּ� ��ǥ
    public Vector3 maxRange = new Vector3(12, 0, 8); // �ִ� ��ǥ

    // Start is called before the first frame update
    void Start()
    {
        agent.updateRotation = false;
        SetRandomDestination();
    }

    // Update is called once per frame
    void Update()
    {
        DetectInFront();
        if (agent.remainingDistance < 0.1f && !agent.pathPending)
        {
            // ��ǥ ������ �����ϸ� ���ο� ���� �������� ����
            SetRandomDestination();
        }
        else
        {
            MoveInGrid();
        }
    }


    void DetectInFront()
    {
        Vector3 rayOrigin = transform.position + new Vector3(0, -0.3f, 0);  // NPC�� �Ʒ��ʿ��� ����ĳ��Ʈ ����

        // NPC�� ���� �̵� ���⿡ ���� ����ĳ��Ʈ ������ ����
        Vector3 rayDirection = agent.velocity.normalized;

        // velocity�� 0�� ���� ����� �⺻��
        if (rayDirection == Vector3.zero)
        {
            rayDirection = transform.forward; // �⺻������ �������� ����
        }

        Ray ray = new Ray(rayOrigin, rayDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange))
        {
            // ������ ��ü�� �̸��� "Asher"���
            if (hit.collider.name == "Asher")
            {
                Debug.Log("Asher ������: " + hit.collider.name);

                // Asher�� ��ġ ��ǥ�� �׸��忡 ����
                Vector3 asherPosition = SnapToGrid(hit.transform.position);

                // ������ ��ǥ�� ��ǥ �������� ����
                SetDestination(asherPosition);
            }
        }
    }



    void SetDestination(Vector3 destination)
    {
        targetPosition = destination;
        agent.SetDestination(targetPosition);
        Debug.Log("Asher�� ��ġ�� �̵�: " + targetPosition);
    }

    // OnDrawGizmos�� ����Ͽ� ����ĳ��Ʈ ��θ� Scene�� �ð������� ǥ��
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // ������ ������ ���������� ����

        Vector3 rayOrigin = transform.position + new Vector3(0, -0.3f, 0);

        // NPC�� ���� �̵� ���⿡ ���� ����ĳ��Ʈ ������ ����
        Vector3 rayDirection = agent.velocity.normalized;

        // velocity�� 0�� ���� ����� �⺻��
        if (rayDirection == Vector3.zero)
        {
            rayDirection = transform.forward; // �⺻������ �������� ����
        }

        // ����ĳ��Ʈ�� �������� ���� ������ŭ Gizmos�� ���� �׸���
        Gizmos.DrawRay(rayOrigin, rayDirection * detectionRange);
    }

    // ������ �������� �����ϰ� �����ϴ� �Լ�
    void SetRandomDestination()
    {
        // minRange�� maxRange ���̿��� ������ ��ġ ����
        Vector3 randomPosition = new Vector3(
            Random.Range(minRange.x, maxRange.x),
            0,  // Y�� �׻� 0
            Random.Range(minRange.z, maxRange.z)
        );

        // ���� ��ǥ�� �׸��忡 ����
        targetPosition = SnapToGrid(randomPosition);
      //  Debug.Log("��ġ:" + targetPosition);

        // ������ ����
        agent.SetDestination(targetPosition);
    }

    // ���� ��ǥ�� �����ϴ� �Լ�
    Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), position.y, Mathf.Round(position.z));
    }

    // �׸��� �󿡼� X��� Z�����θ� �̵��ϴ� �Լ�
    void MoveInGrid()
    {
        Vector3 currentPosition = transform.position;

        if (moveInXAxis)
        {
            // X�� �������� �̵�
            Vector3 nextPosition = new Vector3(targetPosition.x, currentPosition.y, currentPosition.z);
            agent.SetDestination(nextPosition);

            // X�� �̵��� �Ϸ�Ǿ����� Ȯ��
            if (Mathf.Abs(agent.remainingDistance) < 0.1f)
            {
                moveInXAxis = false; // Z�� �̵����� ��ȯ
            }
        }
        else
        {
            // Z�� �������� �̵�
            Vector3 nextPosition = new Vector3(currentPosition.x, currentPosition.y, targetPosition.z);
            agent.SetDestination(nextPosition);

            // Z�� �̵��� �Ϸ�Ǿ����� Ȯ��
            if (Mathf.Abs(agent.remainingDistance) < 0.1f)
            {
                moveInXAxis = true; // X�� �̵����� �ٽ� ��ȯ
            }
        }
    }
}