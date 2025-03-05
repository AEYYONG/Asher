using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Balloon : MonoBehaviour
{
    [SerializeField] private RectTransform balloonRect;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Vector2 padding = new Vector2(1000f, 1000f);

    [SerializeField] private List<string> StartGame = new List<string>();
    [SerializeField] private List<string> FirstFind = new List<string>();
    [SerializeField] private List<string> WrongChoice = new List<string>();
    [SerializeField] private List<string> FirstSteal = new List<string>(); // 먼저 뺏기 아이템 사용한 경우
    [SerializeField] private List<string> Steal = new List<string>();
    [SerializeField] private List<string> MiniGameWin = new List<string>();
    [SerializeField] private List<string> MiniGameLose = new List<string>();
    [SerializeField] private List<string> AlmostWin = new List<string>();
    [SerializeField] private List<string> AlmostLose = new List<string>();

    // 
    // 플레이어 변수 0은 둘다 아닌경우 1은 플레이어 2는 npc
    public int isPlayer = 0;

    private int wrongCount = 0;
    private int npcwrongCount = 0;
    private string _condition = "";

    private Image image;

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
        ScriptChange();
    }

    void ScriptChange()
    {
        // 이미지, 텍스트 활성화
        image.enabled = true;
        switch (_condition)
        {
            case "start": //!
                // 시작 스크립트 중 하나 랜덤으로 출력
                ShowRandomDialogue(StartGame);
                break;
            case "first_find":
                // 첫음 찾을때 스크립트 하나 랜덤으로
                if (isPlayer == 1)// 플레이어가 첫음 뒤집은 경우
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
                }
                
                break;
            case "wrong_choice":
                // 틀린 타일을 뒤집었을 때
                if(isPlayer == 1)// 플레이어가 잘못 뒤집은 경우
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
                }

                
                
                break;

            case "steal": //!
                // 뺏기 아이템 사용시
                if (isPlayer == 1)// 플레이어가 아이템 사용한 경우
                {
                    if (gameObject.tag == "Player")
                    {
                        
                        ShowRandomDialogue(FirstSteal);
                       
                        isPlayer = 0;
                    }
                    else // npc 대사
                    {
                        ShowRandomDialogue(Steal);

                        isPlayer = 0;
                    }
                }
                else if (isPlayer == 2)
                {
                    if (gameObject.tag == "NPC")
                    {
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

                // 태그가 player인 경우 승리 dialog
                if (gameObject.tag == "Player")
                {
                    ShowRandomDialogue(MiniGameWin);
                }
                else // 태그가 npc인 경우 패배 dialog
                {
                    ShowRandomDialogue(MiniGameLose);
                }

                break;
            case "mg_lose": //!!
                // 미니게임 패배시
                ShowRandomDialogue(MiniGameLose);
                // 태그가 player인 경우 패배 dialog
                if (gameObject.tag == "Player")
                {
                    ShowRandomDialogue(MiniGameLose);
                }
                else // 태그가 npc인 경우 승리 dialog
                {
                    ShowRandomDialogue(MiniGameWin);
                }
                break;
            case "almostwin":
                // 4개 모은 경우

                // 태그가 player인 경우 승리 dialog
                if (gameObject.tag == "Player")
                {
                    ShowRandomDialogue(AlmostWin);
                }
                else // 태그가 npc인 경우 패배 dialog
                {
                    ShowRandomDialogue(AlmostLose);
                }

               
                break;
            case "almostlose":
                // 상대가 4개 모은 경우

                // 태그가 player인 경우 패배 dialog
                if (gameObject.tag == "Player")
                {
                    ShowRandomDialogue(AlmostLose);
                }
                else // 태그가 npc인 경우 승리 dialog
                {
                    ShowRandomDialogue(AlmostWin);
                }

              
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
        Debug.Log("현재 텍스트: " + text);
        UpdateSpeechBubbleSize();
    }

    private void UpdateSpeechBubbleSize()
    {
        dialogText.ForceMeshUpdate();
        Vector2 textSize = dialogText.textBounds.size;
        balloonRect.sizeDelta = textSize + padding; // 텍스트 크기 + 패딩
        Invoke("Done", 2f);
    }

    private void Done()
    {
        // 이미지 텍스트 비활성화
        
        image.enabled = false;


        string Empty = "";
        SetDialog(Empty);
    }
}
