using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UP_Anim : MonoBehaviour
{
    // 애니메이션 종료시간 가져오기위한 스크립트
    [SerializeField] private MiniGameUIAnim minigameUIAnim;
    [SerializeField] private GameObject MiniGameTitleUI;


    public float moveDistance = 100f; // 이동 거리
    public float duration = 1f; // 애니메이션 지속 시간
    public Ease easingType = Ease.OutBounce;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("MoveStart", minigameUIAnim.duration);
    }

    void MoveStart()
    {
        Invoke("SetActiveUI", duration);
        transform.DOMoveY(transform.position.y + (moveDistance), duration)
            .SetEase(easingType);
    }

    void SetActiveUI()
    {
        MiniGameTitleUI.SetActive(true);
    }

}
