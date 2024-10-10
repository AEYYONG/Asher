using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class To_Idle : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 애니메이션 상태가 종료될 때 Idle 상태로 전환
        animator.Play("idle");  
    }
}
