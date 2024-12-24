using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideCutSceneAnimationController : MonoBehaviour
{
    public Animator parentAnimator;
    public void SlideOut()
    {
        //부모 오브젝트에서 Animator 참조해서 SlideOut Trigger 호출하기
        Debug.Log("호출되는지 체크");
        parentAnimator.SetTrigger("SlideOut");
    }
}
