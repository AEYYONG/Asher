using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class ButtonsController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float moveDistance = 250f; // 이동 거리
    public float duration = 1f; // 애니메이션 지속 시간
    public Ease easingType = Ease.OutBounce;

    private GameObject[] buttonObjects;


    private Vector3 originalScale;// 원래 크기

    public GameObject Windows;

    public bool isMoving = false;

    private bool isClick = false;

    private Image buttonImage; // 버튼의 이미지 컴포넌트
    public Sprite hoverSprite; // 마우스가 위로 올라갔을 때 변경될 이미지
    public Sprite normalSprite; // 기본 이미지


    void Start()
    {


        // 다른 버튼 오브젝트
        buttonObjects = GameObject.FindGameObjectsWithTag("Button");


        originalScale = transform.localScale;

        buttonImage = GetComponent<Image>();

    }
    public bool IsMoving()
    {
        return isMoving; // 현재 이동 중 상태를 반환
    }



    // 마우스가 버튼 위로 올라갔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        Transform child = transform.GetChild(0);
        child.gameObject.SetActive(true);

        transform.DOScale(0.8f, 0.1f).SetEase(Ease.OutBack); // 부드럽게 커짐
        // 이미지 변경
        if (!isClick)
        {
            buttonImage.sprite = hoverSprite;
        }
    }

    // 마우스가 버튼에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        Transform child = transform.GetChild(0);
        child.gameObject.SetActive(false);

        transform.DOScale(0.7f, 0.1f).SetEase(Ease.InBack); // 원래 크기로 복귀
        // 이미지 변경
        if (!isClick)
        {
            buttonImage.sprite = normalSprite;
        }
    }

    public void OnButtonClick()
    {
        isMoving = false;
        isClick = true;

        Windows.SetActive(true);

        foreach (GameObject button in buttonObjects)
        {
            buttonImage.sprite = normalSprite;
            button.transform.DOMoveX(button.transform.position.x - moveDistance, duration)
                            .SetEase(easingType) // Tween 애니메이션
                             .OnComplete(() =>
                             {
                                 
                                 isMoving = false;
                                 isClick = false;
                             });
        }
    }

    public void ComeBack()
    {
        if (isMoving) return;

        isMoving = true;

        foreach (GameObject button in buttonObjects)
        {
            button.transform.DOMoveX(button.transform.position.x + moveDistance, duration)
                            .SetEase(easingType)
                            .OnComplete(() => isMoving = false);
        }

    }

    public void OnClickPlay()
    {
       // LoadingScene.LoadScene("StageNoaP");
       SceneManager.LoadScene("StageNoaP");
    }



}
