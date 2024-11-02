using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AggroTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("어그로 아이템 사용");

        NPC_Move npc = FindObjectOfType<NPC_Move>();
        if (npc != null)
        {
            npc.detectionRange = 4.5f;
            Debug.Log("NPC의 detectionRange가 4.5로 변경");

            // NPC의 NavMeshAgent 속도 변경
            NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = 3.5f;
                Debug.Log("NPC speed up");
            }
            else
            {
                Debug.LogWarning("NPC에서 NavMeshAgent 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("NPC_Move 스크립트를 가진 오브젝트를 찾을 수 없습니다.");
        }


    }
}
