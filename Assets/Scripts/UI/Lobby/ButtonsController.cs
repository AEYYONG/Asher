using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ButtonsController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float moveDistance = 250f; // 이동 거리
    public float duration = 1f; // 애니메이션 지속 시간
    public Ease easingType = Ease.OutBounce;

    private GameObject[] buttonObjects;


    private Vector3 originalScale;// 원래 크기

    public GameObject Windows;

    public bool isMoving = false;

    void Start()
    {


        // 다른 버튼 오브젝트
        buttonObjects = GameObject.FindGameObjectsWithTag("Button");


        originalScale = transform.localScale;

    }
    public bool IsMoving()
    {
        return isMoving; // 현재 이동 중 상태를 반환
    }



    // 마우스가 버튼 위로 올라갔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(1.1f, 0.1f).SetEase(Ease.OutBack); // 부드럽게 커짐
    }

    // 마우스가 버튼에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1f, 0.1f).SetEase(Ease.InBack); // 원래 크기로 복귀
    }

    public void OnButtonClick()
    {
        isMoving = false;

        Windows.SetActive(true);

        foreach (GameObject button in buttonObjects)
        {
            button.transform.DOMoveX(button.transform.position.x - moveDistance, duration)
                            .SetEase(easingType) // Tween 애니메이션
                             .OnComplete(() => isMoving = false);
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
        LoadingScene.LoadScene("StageNoaP");
            //SceneManager.LoadScene("StageNoaP");
    }



}
