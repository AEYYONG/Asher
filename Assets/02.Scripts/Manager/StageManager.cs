using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
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
        stageSO.InitHeartStoneCnt();
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
        // 감지범위 시각화
        NPC_Move npc = FindObjectOfType<NPC_Move>();
        npc.SensorON = true;
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
        
        //플레이어와 NPC의 애니메이션 강제 변경
        player.GetComponent<Animator>().Play("idle");
        npc.GetComponent<Animator>().Play("down_npc");
        
        //타이머 일시 중지
        timer.StopTimer();
    }

    public void StartAllCharacterMove()
    {
        //플레이어와 NPC의 애니메이션 강제 변경
        player.GetComponent<Animator>().Play("idle");
        npc.GetComponent<Animator>().Play("down_npc");
        
        //이동 지점 재설정
        player.StartRemove(player.transform.position);
        npc.StartRemove();
        
        //타이머 재개하기
        timer.RestartTimer();
        
        //플레이어와 NPC 움직임 재개하기
        player.isStart = true;
        npc.agent.isStopped = false;
    }

    IEnumerator GameClear()
    {
        stageSO.InitHeartStoneCnt();
        isEnd = true;
        StopAllCharacterMove();
        VFXManager.Instance.PlayVFX("GameClearTransition",FindObjectOfType<StageUIManager>().transform);
        yield return new WaitForSeconds(1.4f);
        MySceneManager.Instance.ChangeScene("GameClear");
        AudioManager.Instance.PlayBGM(AudioManager.Instance.bgmDictionary["Asher Talk Theme"]);
    }

    public IEnumerator GameOver()
    {
        stageSO.InitHeartStoneCnt();
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
        AudioManager.Instance.PlayBGM(AudioManager.Instance.bgmDictionary["Asher Talk Theme"]);
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
