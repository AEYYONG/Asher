using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class Balloon : MonoBehaviour
{
    [SerializeField] private RectTransform balloonRect;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Vector2 padding = new Vector2(1000f, 1000f);

    [SerializeField] private List<string> StartGame = new List<string>();
    [SerializeField] private List<string> FirstFind = new List<string>();
    [SerializeField] private List<string> WrongChoice = new List<string>();
    [SerializeField] private List<string> FirstSteal = new List<string>(); // 먼저 뺏기 아이템 사용한 경우
    [SerializeField] private List<string> Steal = new List<string>(); // 상대가 먼저 뺏기 아이템 사용한 경우
    [SerializeField] private List<string> MiniGameWin = new List<string>();
    [SerializeField] private List<string> MiniGameLose = new List<string>();
    [SerializeField] private List<string> AlmostWin = new List<string>();
    [SerializeField] private List<string> AlmostLose = new List<string>();

    // 애니메이션 UI
    [SerializeField] private GameObject StealAnimUI;

   
    // 
    // 플레이어 변수 0은 둘다 아닌경우 1은 플레이어 2는 npc
    public int isPlayer = 0;

    private int wrongCount = 0;
    private int npcwrongCount = 0;
    public string _condition = "";

    // 아이템 사용 유무
    private bool stealItemUse = false;

    // 이미지 활성화시 dotween 애니메이션
    private Image image;
    [SerializeField] private float enableDuration = 0.5f;
    [SerializeField] private Ease enableEase = Ease.OutQuad;
    [SerializeField] private Ease hideEase = Ease.InBack;

     private Vector3 originalScale;

    public string Condition
    {
        get => _condition;
        set
        {
            if (_condition != value) 
            {
                _condition = value;
                ScriptChange(); 
            }
        }
    }
    void Start()
    {
        image = GetComponent<Image>();
        originalScale = balloonRect.localScale;
        balloonRect.localScale = Vector3.zero; // 시작 시 숨김
    }

    private void Update()
    {
    }
    void ScriptChange()
    {
        Debug.Log("스크립트 변경, image 떠야함: "+_condition);
        // 이미지, 텍스트 활성화
        image.enabled = true;
        ShowBalloon();
        switch (_condition)
        {
            case "start": //!
                // 시작 스크립트 중 하나 랜덤으로 출력
                ShowRandomDialogue(StartGame);
                break;
            case "first_find":
                // 첫음 찾을때 스크립트 하나 랜덤으로
                ShowRandomDialogue(FirstFind);
                /*  if (isPlayer == 1)// 플레이어가 첫음 뒤집은 경우
                  {
                      if (gameObject.tag == "Player")
                      {
                          ShowRandomDialogue(FirstFind);
                          isPlayer = 0;
                      }
                  }
                  else if (isPlayer == 2)
                  {
                      if (gameObject.tag == "NPC")
                      {
                          ShowRandomDialogue(FirstFind);
                          isPlayer = 0;

                      }
                  }*/

                break;
            case "wrong_choice":
                // 틀린 타일을 뒤집었을 때

                ShowRandomDialogue(WrongChoice);
                wrongCount++;

                // npc인지 플레이어인지 구분 필요

               /* if (isPlayer == 1)// 플레이어가 잘못 뒤집은 경우
                {
                    if(gameObject.tag == "Player")
                    {
                        // 랜덤이 아니라 리스트 인덱스 순서대로 출력
                        ShowRandomDialogue(WrongChoice);
                        wrongCount++;
                        isPlayer = 0;
                    }
                }
                else if(isPlayer == 2)
                {
                    if(gameObject.tag == "NPC")
                    {
                        ShowInOrderDialogue(WrongChoice);
                        npcwrongCount++;
                        isPlayer = 0;

                    }
                }*/

                
                
                break;

            case "steal": // steal 버튼 눌러서 사용시 case steal로 변경, 스크립트 사라지면 ui 애니메이션 재생
                          // 먼저 뺏기 아이템 사용시

                if (isPlayer == 1)// 플레이어가 아이템 사용한 경우
                {
                    if (gameObject.tag == "Player")
                    {
                        stealItemUse = true;
                        ShowRandomDialogue(FirstSteal);

                        isPlayer = 0;
                    }
                    else // npc 대사
                    {
                        ShowRandomDialogue(Steal);

                        isPlayer = 0;
                    }
                }
                else if (isPlayer == 2) // npc가아이템 사용한 경우
                {
                    if (gameObject.tag == "NPC")
                    {
                        stealItemUse = true;
                        ShowRandomDialogue(FirstSteal);

                        isPlayer = 0;
                    }
                    else 
                    {
                        ShowRandomDialogue(Steal);

                        isPlayer = 0;
                    }
                }
               
                break;

           
            case "mg_win":
                // 미니게임 승리시
                ShowRandomDialogue(MiniGameWin);

                break;
            case "mg_lose": 
                // 미니게임 패배시
                ShowRandomDialogue(MiniGameLose);
               
                break;
            case "almostwin":
                // 4개 모은 경우
                ShowRandomDialogue(AlmostWin);


                break;
            case "almostlose":
                // 상대가 4개 모은 경우
                ShowRandomDialogue(AlmostLose);

                break;
            default:
                Debug.Log("Unknown condition!");
                break;
        }
    }

    public void ShowRandomDialogue(List<string> dialogueList)
    {
        if (dialogueList.Count > 0)
        {
            string randomText = dialogueList[Random.Range(0, dialogueList.Count)];
            SetDialog(randomText);
            _condition = "";
        }
        else
        {
            Debug.LogWarning("대사 리스트가 비어 있음");
        }
    }


    public void ShowInOrderDialogue(List<string> dialogueList)
    {
        
        if (dialogueList.Count > 0)
        {
            // npc 플레이어 나눠야 함 회의 후 변동사항 적용 필요
            string inOrderText = dialogueList[wrongCount];
            SetDialog(inOrderText);
        }
        else
        {
            Debug.LogWarning("대사 리스트가 비어 있음");
        }
    }

    public void SetDialog(string text)
    {
        dialogText.text = text;
        UpdateSpeechBubbleSize();
    }

    private void UpdateSpeechBubbleSize()
    {
        dialogText.ForceMeshUpdate();
        Vector2 textSize = dialogText.textBounds.size;
        balloonRect.sizeDelta = textSize + padding; // 텍스트 크기 + 패딩
        if (stealItemUse)
        {
            stealItemUse = false;
            Invoke("StealAnimStart",2f);
        }
        else
        {
            Invoke("Done", 2f);
        }
       
    }

    public void ShowBalloon()
    {
        image.enabled = true;
        balloonRect.DOScale(originalScale, enableDuration)
            .SetEase(enableEase)
            .OnComplete(() => StartCoroutine(AutoHideBalloon()));
    }

    private IEnumerator AutoHideBalloon()
    {
        yield return new WaitForSeconds(3f);

        if (stealItemUse)
            StealAnimStart();
        else
            HideBalloon();
    }

    private void Done()
    {
        HideBalloon();

        string Empty = "";
        SetDialog(Empty);
    }

    public void HideBalloon()
    {
        balloonRect.DOScale(Vector3.zero, enableDuration * 0.5f)
       .SetEase(hideEase)
       .OnComplete(() =>
       {
           image.enabled = false;
        });
    }

    private void StealAnimStart()
    {
        // 이미지 텍스트 비활성화

        image.enabled = false;


        string Empty = "";
        SetDialog(Empty);

        // 애니메이션 UI 활성화
        StealAnimUI.SetActive(true);
    }

    // 말풍선 활성화 닷트윈 효과
    
}
