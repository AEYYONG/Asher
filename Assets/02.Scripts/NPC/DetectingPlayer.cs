using UnityEngine;

public class DetectingPlayer : MonoBehaviour
{
    private NPCMove parentNPC;
    public bool isCollidingWithAsher = false;

    void Start()
    {
        // �θ��� NPCMove ��ũ��Ʈ�� ������
        parentNPC = GetComponentInParent<NPCMove>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // �浹 ����� Asher��� �̸��� ���� ������Ʈ�� ���
        if (other.CompareTag("Player"))
        { 
            // NPC�� ���¸� �߰� ���� ����
            parentNPC.state = 2;
            isCollidingWithAsher = true;

            // ���� ���� ���� �̵� �ڷ�ƾ�� ����
            parentNPC.StopMoving();

            // �浹 �� �θ� ������Ʈ(NPC)�� ���� ��ġ�� startPos�� ����
            Vector3 parentPosition = parentNPC.transform.position;
            parentNPC.startPos = (new Vector3Int(
                Mathf.FloorToInt(parentPosition.x),
                Mathf.FloorToInt(parentPosition.y),
                Mathf.FloorToInt(parentPosition.z)
            ));
            Debug.Log("�浹�� �ٽ� ������ ��ġ startPos: " + parentPosition);

            Vector3 asherPosition = other.transform.position;
            parentNPC.targetPos = (new Vector3Int(
                Mathf.FloorToInt(asherPosition.x),
                Mathf.FloorToInt(asherPosition.y),
                Mathf.FloorToInt(asherPosition.z)
            ));

            // �� ��� Ž�� ����
            parentNPC.StartPathFinding();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Asher���� �浹�� ������ ���
        if (other.CompareTag("Player"))
        {
            isCollidingWithAsher = false;
            parentNPC.state = 3;
            Debug.Log("Asher���� �浹�� ����Ǿ����ϴ�.");

        }

    }

    public bool IsCollidingWithAsher()
    {
        return isCollidingWithAsher;
    }


}
