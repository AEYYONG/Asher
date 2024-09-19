using UnityEngine;

public class DetectingPlayer : MonoBehaviour
{
    private NPCMove parentNPC;
    public bool isCollidingWithAsher = false;

    void Start()
    {
        // 부모의 NPCMove 스크립트를 가져옴
        parentNPC = GetComponentInParent<NPCMove>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌 대상이 Asher라는 이름의 게임 오브젝트일 경우
        if (other.CompareTag("Player"))
        { 
            // NPC의 상태를 추격 모드로 변경
            parentNPC.state = 2;
            isCollidingWithAsher = true;

            // 현재 진행 중인 이동 코루틴을 멈춤
            parentNPC.StopMoving();

            // 충돌 시 부모 컴포넌트(NPC)의 현재 위치를 startPos로 설정
            Vector3 parentPosition = parentNPC.transform.position;
            parentNPC.startPos = (new Vector3Int(
                Mathf.FloorToInt(parentPosition.x),
                Mathf.FloorToInt(parentPosition.y),
                Mathf.FloorToInt(parentPosition.z)
            ));
            Debug.Log("충돌후 다시 생성한 위치 startPos: " + parentPosition);

            Vector3 asherPosition = other.transform.position;
            parentNPC.targetPos = (new Vector3Int(
                Mathf.FloorToInt(asherPosition.x),
                Mathf.FloorToInt(asherPosition.y),
                Mathf.FloorToInt(asherPosition.z)
            ));

            // 새 경로 탐색 시작
            parentNPC.StartPathFinding();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Asher와의 충돌이 끝났을 경우
        if (other.CompareTag("Player"))
        {
            isCollidingWithAsher = false;
            parentNPC.state = 3;
            Debug.Log("Asher와의 충돌이 종료되었습니다.");

        }

    }

    public bool IsCollidingWithAsher()
    {
        return isCollidingWithAsher;
    }


}
