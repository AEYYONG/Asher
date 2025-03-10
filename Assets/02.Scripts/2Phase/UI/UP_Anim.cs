using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UP_Anim : MonoBehaviour
{
    // 애니메이션 종료시간 가져오기위한 스크립트
    [SerializeField] private MiniGameUIAnim minigameUIAnim;
    [SerializeField] private GameObject MiniGameTitleUI;
    [SerializeField] private GameObject StealMiniGemaUI;

    // 애니메이션 종료후 사라질 본 게임 오브젝트
    [SerializeField] private GameObject MiniGameStartUI;

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
        // 아래 duration은 추우 이펙트 개발 후 이펙트 duration으로 변경 필요
        Invoke("SetActiveMiniGameUI",duration);
    }
    void SetActiveMiniGameUI()
    {
        MiniGameStartUI.SetActive(false);
        StealMiniGemaUI.SetActive(true);
        
    }
}
