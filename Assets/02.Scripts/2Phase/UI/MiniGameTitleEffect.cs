using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameTitleEffect : MonoBehaviour
{
    private Image image;
    [SerializeField] private GameObject FlashImage;
    [SerializeField] private float duration = 1f;

    [SerializeField] private GameObject StealMiniGemaUI;

    // 애니메이션 종료후 사라질 본 게임 오브젝트
    [SerializeField] private GameObject MiniGameStartUI;

    // 타이틀 이펙트 종료 후 UI 사라지기까지 시간
    public float SetActiveMiniGameUITime;



    void OnEnable()
    {
        FlashImage.SetActive(true);
        image = GetComponent<Image>();
        image.fillMethod = Image.FillMethod.Horizontal;
        image.fillOrigin = (int)Image.OriginHorizontal.Left;

        image.fillAmount = 0;

        StartCoroutine(FillImage());
    }

    private IEnumerator FillImage()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.fillAmount = Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }

        image.fillAmount = 1f;
        FlashImage.SetActive(false);
        Invoke("SetActiveMiniGameUI", SetActiveMiniGameUITime);
    }

    void SetActiveMiniGameUI()
    {
        gameObject.SetActive(false);
        MiniGameStartUI.SetActive(false);
        StealMiniGemaUI.SetActive(true);

    }
}
