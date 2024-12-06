using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AggroTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        StartCoroutine(ActivateAggroTrap());
    }

    private IEnumerator ActivateAggroTrap()
    {
        NPC_Move npc = FindObjectOfType<NPC_Move>();
        if (npc != null)
        {
            // 기존 값 저장
            float originalDetectionRange = npc.detectionRange;
            NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
            float originalSpeed = agent != null ? agent.speed : 1.5f;

            npc.detectionRange = 4.5f;
            Debug.Log("NPC의 detectionRange가 4.5로 변경");


            if (agent != null)
            {
                agent.speed = 3.5f;
                Debug.Log("NPC speed up");
            }
            else
            {
                Debug.LogWarning("NPC에서 NavMeshAgent 컴포넌트를 찾을 수 없습니다.");
            }

            yield return new WaitForSeconds(10f);
            npc.detectionRange = originalDetectionRange;
            agent.speed = originalSpeed;
            Debug.Log("NPC 속도 복구");
        }
        else
        {
            Debug.LogWarning("NPC_Move 스크립트를 가진 오브젝트를 찾을 수 없습니다.");
        }

        //vfx 실행
        Animator effectAnimator = transform.GetChild(0).GetComponent<Animator>();
        effectAnimator.SetTrigger("TrapMatch");
    }
}
