using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeEffect : MonoBehaviour
{
    public LoopType loopType;
    private Image image;
    void Start()
    {
        image = GetComponent<Image>();
        image.DOFade(0.0f, 1f) 
               .SetLoops(-1, loopType) 
               .SetEase(Ease.InOutSine);
    }

}
