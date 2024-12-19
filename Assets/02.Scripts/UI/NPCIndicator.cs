using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCIndicator : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform indicatorTransform;
    public Camera fogCamera; 
    public Transform npcTransform;
    public RectTransform indicatorBounds;
    public RectTransform npcPortraitTransform;
    [Header("Settings")]
    public float arrowOffset = 100f;
    void Update()
    {
        // NPC의 화면 좌표를 구함 (스크린 좌표로 변환)
        Vector3 screenPos = fogCamera.WorldToScreenPoint(npcTransform.position);
        
        // NPC가 화면 안에 있으면 화살표 비활성화
        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
        {
            indicatorTransform.gameObject.SetActive(false);
            return;
        }
        
        indicatorTransform.gameObject.SetActive(true);

        // 직사각형의 중심 좌표 (UI 좌표계로 변환)
        Vector2 rectCenter = indicatorBounds.rect.center;

        // NPC의 방향 벡터 구하기 (화면 중심에서 NPC까지의 벡터)
        Vector2 direction = new Vector2(screenPos.x - Screen.width / 2, screenPos.y - Screen.height / 2).normalized;

        // 직사각형 영역을 가득 채우는 타원의 x, y 반지름
        float a = indicatorBounds.rect.width / 2f - arrowOffset; // 타원의 x 반지름
        float b = indicatorBounds.rect.height / 2f - arrowOffset; // 타원의 y 반지름

        // 방향 벡터에 따른 타원 위의 점 구하기 (타원의 매개변수 방정식)
        float angle = Mathf.Atan2(direction.y, direction.x);
        float x = a * Mathf.Cos(angle);
        float y = b * Mathf.Sin(angle);
        
        // 화살표의 새 위치 설정 (RectTransform의 로컬 위치로 설정)
        indicatorTransform.localPosition = new Vector2(x, y);

        // 화살표 회전 설정 (위쪽이 0도이므로 90도를 빼줌)
        float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        indicatorTransform.rotation = Quaternion.Euler(0, 0, rotationAngle + 90);
        npcPortraitTransform.rotation = Quaternion.Euler(0,0,0);
    }
}
