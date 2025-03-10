using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MiniGameUIAnim : MonoBehaviour
{
    public float moveDistance = 250f; // 이동 거리
    public float duration = 1f; // 애니메이션 지속 시간
    public Ease easingType = Ease.OutBounce;
    public bool moveLeft = false;

    // 처음 위치
    private Vector3 startPosition;

    void Awake()
    {
        startPosition = transform.position;
    }
    void OnEnable()
    {
        ResetPosition(); 
        MoveUI();
    }
    void ResetPosition()
    {
        transform.position = startPosition;
        Debug.Log("위치 이동 다시");
    }
    void MoveUI()
    {
        float direction = moveLeft ? 1f : -1f;

        transform.DOMoveX(transform.position.x + (moveDistance * direction), duration)
            .SetEase(easingType);
    }
}
