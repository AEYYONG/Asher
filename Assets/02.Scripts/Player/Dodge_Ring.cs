using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dodge_Ring : MonoBehaviour
{
    private Image ringImage;
    public float duration = 1.6f;

    private float elapsedTime = 0f;

    void Start()
    {
        ringImage = GetComponent<Image>();
        ringImage.fillAmount = 0f;
    }

    void Update()
    {
        // 시간이 지나면서 링을 채우는 로직
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float fill = elapsedTime / duration; // 현재 시간 비율
            ringImage.fillAmount = fill;
            if (ringImage.fillAmount == 1f)
            {
                gameObject.SetActive(false);
            }
        }
        
    }
}
