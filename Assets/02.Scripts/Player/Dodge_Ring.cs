using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dodge_Ring : MonoBehaviour
{
    public static Dodge_Ring Instance;

    private Image ringImage;
    public float duration = 2f;
    public Dodge_Key dodgeKey;
    public GameObject Key;


    private float elapsedTime = 0f;

    private void Awake()
    {
        ringImage = GetComponent<Image>();

        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 싱글턴 중복 방지
        }
    }


    void Start()
    {
        
        ringImage.fillAmount = 0f;
        Key.SetActive(true);
        dodgeKey.ShowRandomKey();
    }

    private void OnEnable()
    {
        elapsedTime = 0f;
        ringImage.fillAmount = 0f;
        Key.SetActive(true);
        dodgeKey.ShowRandomKey(); // Dodge_Key에서 랜덤 키 표시
    }

    void Update()
    {
        // 시간이 지나면서 링을 채우는 로직
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float fill = elapsedTime / duration; // 현재 시간 비율
            ringImage.fillAmount = fill;
        }
        
    }

    public void OffEnable()
    {
        Key.SetActive(false);
        gameObject.SetActive(false);
    }
}
