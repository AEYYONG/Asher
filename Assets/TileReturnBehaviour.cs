using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileReturnBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerInteract _playerInteract = FindObjectOfType<PlayerInteract>();
        Tile _tile = animator.GetComponent<Tile>();
        int tileSelectedNum = _tile.tileSO.selectNum;
        //Debug.Log(_playerInteract.GetSelectedTilesCnt() + "," + tileSelectedNum);
        if (_playerInteract.GetSelectedTilesCnt() == tileSelectedNum)
        {
            //초기화
            _playerInteract.InitValue();
        }
        _tile.tileSO.selectNum = 0;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
