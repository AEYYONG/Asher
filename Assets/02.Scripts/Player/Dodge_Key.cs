using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dodge_Key : MonoBehaviour
{
    private string[] keys = { "X", "Z", "C", "F", "V" }; // 표시할 키 목록
    private string currentKey;
    private bool isKeyPromptActive = false;

    public TextMeshProUGUI keyPromptText;

    public delegate void DodgeResult(bool success);
    public static event DodgeResult OnDodgeComplete;


    private void Awake()
    {
        keyPromptText = GetComponent<TextMeshProUGUI>();

    }


    public void ShowRandomKey()
    {
        keyPromptText.gameObject.SetActive(true);
        StartCoroutine(DisplayKeyCoroutine());
        Debug.Log("호출?");
    }

    private IEnumerator DisplayKeyCoroutine()
    {
        // 무작위 키 선택
        currentKey = keys[Random.Range(0, keys.Length)];
        keyPromptText.text = currentKey;

        // 텍스트 활성화
        keyPromptText.gameObject.SetActive(true);
        isKeyPromptActive = true;

        // 0.2초 동안 활성화
        float timer = 1.6f;
        while (timer > 0f)
        {
            if (Input.GetKeyDown(currentKey.ToLower())) // 올바른 키 입력
            {
                OnDodgeComplete?.Invoke(true); // 회피 성공 알림
                ClearPrompt();
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }

        // 시간 초과 시 실패 처리
        OnDodgeComplete?.Invoke(false); // 회피 실패 알림
        ClearPrompt();

    }

    private void ClearPrompt()
    {
        isKeyPromptActive = false;
        keyPromptText.gameObject.SetActive(false);
    }
}
