using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TextBounceEffect : MonoBehaviour
{

    private RectTransform textRectTransform;
    public float moveDistance = 1f;
    public float duration = 0.5f;

    void Start()
    {
        textRectTransform = GetComponent<RectTransform>();

        textRectTransform.DOAnchorPosY(textRectTransform.anchoredPosition.y + moveDistance, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
