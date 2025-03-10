using UnityEngine;
using UnityEngine.UI;

public class GaugeMover : MonoBehaviour
{
    [SerializeField] private Image gaugeImage;


    [SerializeField] private float minX = -950f; // 게이지의 왼쪽 끝 위치
    [SerializeField] private float maxX = 800f;  // 게이지의 오른쪽 끝 위치

    private RectTransform rectTransform;


    // 이미지 변경
    private Image thisImage;
    [SerializeField] private Sprite pressedSprite; // 스페이스바를 누를 때 변경될 이미지
    private Sprite normalSprite; // 기본 이미지 저장

    // 게임오버시 더이상 스페이스바 움직임 x
    [SerializeField] private StealGauge StealGauge;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        thisImage = GetComponent<Image>();

        if (thisImage != null)
        {
            normalSprite = thisImage.sprite; // 기본 이미지 저장
        }
    }


        void Update()
    {

        if (gaugeImage == null || rectTransform == null) return;

        // 스페이스바 입력 시 이미지 변경
        if (!StealGauge.isGameOver)
        {
            if (pressedSprite != null)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    thisImage.sprite = pressedSprite;
                }
                else
                {
                    thisImage.sprite = normalSprite;
                }
            }

        }

        // fillAmount (0~1) 값에 따라 X 좌표 조정
        float newX = Mathf.Lerp(minX, maxX, gaugeImage.fillAmount);

        // 현재 오브젝트의 위치 업데이트
        rectTransform.anchoredPosition = new Vector2(newX, rectTransform.anchoredPosition.y);
    }
}