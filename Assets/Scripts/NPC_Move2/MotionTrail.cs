using System.Collections;
using UnityEngine;

public class MotionTrail : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float trailLifetime = 1f;    // 잔상 유지 시간
    public float trailInterval = 0.3f;    // 잔상 생성 간격
    public Color trailColor = new Color(1, 1, 1, 0.5f); // 잔상색

    private bool isCreatingTrail = false;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartTrail()
    {
        if (!isCreatingTrail)
            StartCoroutine(CreateTrail());
    }

    public void StopTrail()
    {
        isCreatingTrail = false;
    }

    private IEnumerator CreateTrail()
    {
        isCreatingTrail = true;

        while (isCreatingTrail)
        {
            // 잔상 생성
            GameObject trail = new GameObject("Trail");
            SpriteRenderer trailSprite = trail.AddComponent<SpriteRenderer>();
            trailSprite.sprite = spriteRenderer.sprite;
            trailSprite.color = trailColor;
            trailSprite.transform.position = transform.position;
            trailSprite.transform.rotation = transform.rotation;
            trailSprite.sortingLayerID = spriteRenderer.sortingLayerID;
            trailSprite.sortingOrder = spriteRenderer.sortingOrder - 1;

            // 현재 오브젝트의 스케일 적용
            trailSprite.transform.localScale = transform.localScale;


            StartCoroutine(FadeOutTrail(trailSprite));

            // 잔상 간격 유지
            yield return new WaitForSeconds(trailInterval);
        }
    }

    private IEnumerator FadeOutTrail(SpriteRenderer trailSprite)
    {
        float elapsedTime = 0f;

        while (elapsedTime < trailLifetime)
        {
            float alpha = Mathf.Lerp(trailColor.a, 0, elapsedTime / trailLifetime);
            trailSprite.color = new Color(trailColor.r, trailColor.g, trailColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(trailSprite.gameObject); // 잔상 제거
    }
}
