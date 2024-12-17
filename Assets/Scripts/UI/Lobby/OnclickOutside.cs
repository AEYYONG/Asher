using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnclickOutside : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isPointerInside = false;

    public GameObject Buttons;
    private ButtonsController buttonController;


    private void Start()
    {
        buttonController = Buttons.GetComponent<ButtonsController>();
    }
    void Update()
    {
        // 마우스 클릭이 감지되었고 포인터가 버튼 밖에 있는 경우
        if (Input.GetMouseButtonDown(0) && !isPointerInside)
        {
            // 버튼 컨트롤러가 이동 중이면 아무 작업도 하지 않음
            if (buttonController != null && buttonController.IsMoving())
            {
                return;
            }

            gameObject.SetActive(false);
            buttonController.ComeBack();
        }
    }



        // 마우스가 버튼 위에 올라갔을 때 호출
        public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerInside = true;
    }

    // 마우스가 버튼에서 벗어났을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
    }

    private bool IsPointerOverUIElement()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // 현재 UI 요소에 포인터가 있는지 확인
        foreach (var result in results)
        {
            if (result.gameObject == gameObject)
                return true; // 현재 GameObject에 포인터가 있음
        }
        return false; // 버튼 밖임
    }
}
