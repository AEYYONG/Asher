using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogueData
{
    public Sprite portrait; 
    public string dialogueText;
}
public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image portraitImage; // 초상화 이미지
    public TextMeshProUGUI dialogueText; // 대화 텍스트
    public GameObject toggleButton; // 토글 버튼

    [Header("Dialogue Data")]
    public List<DialogueData> dialogues; // 대화 데이터 목록

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f; // 한 글자 출력 속도

    private int currentDialogueIndex = 0; // 현재 대화 인덱스
    private Coroutine typingCoroutine;
    private bool isTyping = false; // 현재 텍스트가 출력 중인지 여부

    private void Start()
    {
        toggleButton.SetActive(false);
        StartDialogue();
    }

    private void Update()
    {
        // SPACE 키 입력 처리
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping) // 현재 대사가 출력 중이라면
            {
                // 모든 문장을 즉시 출력
                FinishTyping();
            }
            else if (toggleButton.activeSelf) // 토글 버튼이 활성화된 상태라면
            {
                // 다음 대사로 이동
                NextDialogue();
            }
        }
    }
    public void StartDialogue()
    {
        currentDialogueIndex = 0;
        ShowDialogue(currentDialogueIndex);
    }
    
    private void ShowDialogue(int index)
    {
        if (index >= dialogues.Count)
        {
            Debug.Log("모든 대화를 완료했습니다.");
            EndDialogue();
            return;
        }

        // UI 초기화
        toggleButton.SetActive(false);
        dialogueText.text = "";

        // 데이터 적용
        DialogueData dialogueData = dialogues[index];
        portraitImage.sprite = dialogueData.portrait;

        // 대사 출력 시작
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeDialogue(dialogueData.dialogueText));
    }
    
    private IEnumerator TypeDialogue(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // 대사 출력 완료
        FinishTyping();
    }

    private void FinishTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        DialogueData dialogueData = dialogues[currentDialogueIndex];
        dialogueText.text = dialogueData.dialogueText;

        isTyping = false;
        toggleButton.SetActive(true);
    }
    
    private void NextDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Count)
        {
            ShowDialogue(currentDialogueIndex);
        }
        else
        {
            EndDialogue();
        }
    }
    
    private void EndDialogue()
    {
        Debug.Log("대화가 종료되었습니다.");
        dialogueText.text = "";
        toggleButton.SetActive(false);
    }
}
