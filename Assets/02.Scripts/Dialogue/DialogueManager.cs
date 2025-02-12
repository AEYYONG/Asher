using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DialogueData
{
    public string characterId;
    public DialogueTextData[] textDatas;
}

public class DialogueTextData
{
    public string dialogueText;
    public string choiceText1;
    public string choiceText2;
    public string resultDialogueId1;
    public string resultDialogueId2;
    public string nextDialougeId;
}
public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")] public GameObject dialougeUI;
    public Image portraitImage; // 초상화 이미지
    public TextMeshProUGUI dialogueText; // 대화 텍스트
    public TextMeshProUGUI choice1Text; // 선택자1 텍스트
    public TextMeshProUGUI choice2Text; // 선택지2 텍스트
    public GameObject toggleButton; // 토글 버튼

    [Header("Dialogue Data")] [SerializeField]
    private string _startDialougeKey;
    private Dictionary<string, DialogueData> _dialogueDatas = new Dictionary<string, DialogueData>();

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f; // 한 글자 출력 속도
    private Coroutine _typingCoroutine;
    private bool _isTyping = false; // 현재 텍스트가 출력 중인지 여부
    public Action OnSelectEnd;

    [Header("Portrait Data")] [SerializeField]
    private List<Sprite> _portraits;

    private void Awake()
    {
        ParserCSV parser = new ParserCSV();
        _dialogueDatas = parser.ParseDialogue(_startDialougeKey);
    }

    private void Start()
    {
        toggleButton.SetActive(false);
        StartDialogue();
    }

    private void Update()
    {
        // SPACE 키 입력 처리
        if (Input.GetKeyDown(KeyCode.Space) && OnSelectEnd == null)
        {
            if (_isTyping) // 현재 대사가 출력 중이라면
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
        ShowDialogue(_startDialougeKey);
    }
    
    private void ShowDialogue(string key)
    {
        //키 값에 따른 데화 데이터 가져오기
        //키 내의 텍스트 데이터 모두 출력하기
        //선택지가 있는 텍스트면 
        
        
        // // 데이터 적용
        // DialogueData dialogueData = _dialogueDatas[key];
        // // 초상화 설정
        // if (dialogueData.characterId == "Character_01")
        // {
        //     portraitImage.sprite = _portraits[0];
        // }
        // else
        // {
        //     portraitImage.sprite = _portraits[1];
        // }
        //
        // foreach (var textData in dialogueData.textDatas)
        // {
        //     // UI 초기화
        //     toggleButton.SetActive(false);
        //     dialogueText.text = "";
        //     OnSelectEnd = null;
        //     //선택지
        //     if (!string.IsNullOrEmpty(textData.choiceText1) && !string.IsNullOrEmpty(textData.choiceText2))
        //     {
        //         choice1Text.text = textData.choiceText1;
        //         choice2Text.text = textData.choiceText2;
        //         OnSelectEnd += () =>
        //         {
        //             ShowDialogue(textData.nextDialougeId);
        //         };
        //     }
        //     else
        //     {
        //         choice1Text.text = "";
        //         choice2Text.text = "";
        //     }
        // }
        //
        // // 대사 출력 시작
        // if (_typingCoroutine != null)
        // {
        //     StopCoroutine(_typingCoroutine);
        // }
        // _typingCoroutine = StartCoroutine(TypeDialogue(dialogueData.dialogueText));
    }
    
    private IEnumerator TypeDialogue(string text)
    {
        _isTyping = true;
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
        // if (typingCoroutine != null)
        // {
        //     StopCoroutine(typingCoroutine);
        // }
        //
        // DialogueData dialogueData = dialogues[currentDialogueIndex];
        // //dialogueText.text = dialogueData.dialogueText;
        //
        // isTyping = false;
        // toggleButton.SetActive(true);
    }
    
    private void NextDialogue()
    {
        // currentDialogueIndex++;
        // if (currentDialogueIndex == 1)
        // {
        //     GetComponent<Animator>().SetTrigger("Transition");
        // }
        // if (currentDialogueIndex < dialogues.Count)
        // {
        //     ShowDialogue(currentDialogueIndex);
        // }
        // else
        // {
        //     EndDialogue();
        // }
    }
    
    private void EndDialogue()
    {
        Debug.Log("대화가 종료되었습니다.");
        dialogueText.text = "";
        dialougeUI.SetActive(false);
        if (SceneManager.GetActiveScene().name == "GameClear")
        {
            MySceneManager.Instance.ChangeScene("GameResult");
        }
        else if (SceneManager.GetActiveScene().name == "GameOver")
        {
            GetComponent<Animator>().SetTrigger("Gray");
        }
    }
}
