using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StealGauge : MonoBehaviour
{

    private Image playerGauge; // 게이지 변화(플레이어)

    private float playerCount = 50f;
    private float npcCount = 50f;

    public float repeatTime = 0.2f;
    public float playerScore = 1f;
    public float npcScore = 1f;
    private const int Max_Total = 100;

    // 승리, 패배 변수
    public bool isGameOver = false;


    // 변수, ui 제어
    [SerializeField] private Phase2PlayerInteract Phase2PlayerInteract;
    [SerializeField] private GameObject WinUI;
    [SerializeField] private GameObject LoseUI;

    [SerializeField] private GameObject StealController;

    public float fillValue = 0.5f;


    void Start()
    {
        playerGauge = GetComponent<Image>();

        SetGauge(0.5f);

        // NPC 카운트 2초마다 1씩 증가
        InvokeRepeating("IncreaseNpcCount", 0.1f, 0.2f);
    }

    void Update()
    {
        if (isGameOver) return;

        // 플레이어가 스페이스바를 누르면 카운트 증가
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangePlayerCount(playerScore);
        }
    }

    void IncreaseNpcCount()
    {
        if (isGameOver) return;

        ChangePlayerCount(npcScore);
    }

    void ChangePlayerCount(float amount)
    {
        playerCount += amount;

        playerCount = Mathf.Clamp(playerCount, 0, Max_Total);
        npcCount = Max_Total - playerCount;

        UpdateGauge();
    }

    void UpdateGauge()
    {
        fillValue = (float)playerCount / Max_Total; 

        SetGauge(fillValue);

        if (fillValue >= 0.99f)
        {
            GameWin();
        }

        else if (fillValue <= 0.01f)
        {
            GameLose();
        }

    }

    void SetGauge(float value)
    {
        playerGauge.fillAmount = value; 
    }

    void GameWin()
    {
        isGameOver = true; 
        CancelInvoke("IncreaseNpcCount");

        // 승리 UI 활성화
        WinUI.SetActive(true);
        // 승리하면 
        Debug.Log("승리");

        // 승리하면 플레이어 턴으로 변경
        /*Phase2PlayerInteract.isPlayerTurn = true;
        Phase2PlayerInteract.isNPCTurn = false;*/

        Invoke("ActiveStealController", 2f);
       
        
    }

    void GameLose()
    {
        isGameOver = true; 
        CancelInvoke("IncreaseNpcCount");

        // 패배 UI 활성화
        LoseUI.SetActive(true);
        Debug.Log("패배");
        // 패배하면 NPC 턴으로 변경(임시)
        /*Phase2PlayerInteract.isPlayerTurn = false;
        Phase2PlayerInteract.isNPCTurn = true;*/
    }

    void ActiveStealController()
    {
        StealController.SetActive(true);
    }
}