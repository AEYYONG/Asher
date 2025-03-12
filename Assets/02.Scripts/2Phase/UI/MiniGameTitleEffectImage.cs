using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;



public class MiniGameTitleEffectImage : MonoBehaviour
{
    [SerializeField] private Image gaugeImage;


    [SerializeField] private float minX = -460f; // 게이지의 왼쪽 끝 위치
    [SerializeField] private float maxX = 460f;  // 게이지의 오른쪽 끝 위치

    private RectTransform rectTransform;


    // 이미지 변경
    private Image thisImage; 

    void Start()
    {

        thisImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }


    void Update()
    {
        
        // fillAmount (0~1) 값에 따라 X 좌표 조정
        float newX = Mathf.Lerp(minX, maxX, gaugeImage.fillAmount);

        // 현재 오브젝트의 위치 업데이트
        rectTransform.anchoredPosition = new Vector2(newX, rectTransform.anchoredPosition.y);
    }

}
