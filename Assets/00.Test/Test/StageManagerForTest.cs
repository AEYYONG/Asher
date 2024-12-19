using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManagerForTest : Singleton<StageManagerForTest>
{
    public StageInfoSO stageSO;
    private Player_Move player;
    private NPC_Move npc;
    private bool isEnd = false;
    public RectTransform circleMask;
    public GameObject gameOverTransition;
    public bool isGameOver = false;
    public Timer timer;

    void Awake()
    {
        stageSO.score = 0;
    }
    void Start()
    {
        player = FindObjectOfType<Player_Move>();
        npc = FindObjectOfType<NPC_Move>();
        StopAllCharacterMove();
        StartCoroutine(GameStart());
    }

    void Update()
    {
        if (stageSO.GetHeartStoneCnt() >= stageSO.heartStoneTotalCnt && !isEnd)
        {
            StartCoroutine(GameClear());
        }
    }

    IEnumerator GameStart()
    {
        //3,2,1 후 게임 시작
        VFXManager.Instance.PlayVFX("StartTimer",FindObjectOfType<StageUIManager>().transform);
        yield return new WaitForSeconds(4.5f);
        StartCoroutine(timer.TimerStart(timer._time));
        //BGM 재생 시작
        AudioData bgm1 = AudioManager.Instance.bgmDictionary[stageSO.bgm1];
        AudioData bgm2 = AudioManager.Instance.bgmDictionary[stageSO.bgm2];
        StartCoroutine(AudioManager.Instance.PlaySequentialBGM(bgm1,bgm2));
        //플레이어와 NPC 이동 시작
        StartAllCharacterMove();
    }

    public void StopAllCharacterMove()
    {
        player.isStart = false;
        npc.agent.isStopped = true;
    }

    public void StartAllCharacterMove()
    {
        player.isStart = true;
        npc.agent.isStopped = false;
    }

    IEnumerator GameClear()
    {
        isEnd = true;
        StopAllCharacterMove();
        VFXManager.Instance.PlayVFX("GameClearTransition",FindObjectOfType<StageUIManager>().transform);
        yield return new WaitForSeconds(1.4f);
        MySceneManager.Instance.ChangeScene("GameClear");
    }

    public IEnumerator GameOver()
    {
        isGameOver = true;
        StopAllCharacterMove();
        //현재 플레이어 위치 스크린 좌표로 가져오기
        gameOverTransition.SetActive(true);
        Vector3 playerPosition = player.transform.position;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(playerPosition);
        Vector2 uiPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            FindObjectOfType<StageUIManager>().GetComponent<RectTransform>(),
            screenPosition, 
            FindObjectOfType<StageUIManager>().GetComponent<Canvas>().worldCamera, 
            out uiPosition 
        );
        circleMask.anchoredPosition = uiPosition;
        gameOverTransition.GetComponent<Animator>().SetTrigger("Start Transition");
        yield return new WaitForSeconds(2.5f);
        MySceneManager.Instance.ChangeScene("GameOver");
    }
    
    public void UpdateHeartStoneScore()
    {
        stageSO.score += 10000;
    }
    
    public void UpdateItemScore()
    {
        stageSO.score += 8000;
    }
}
