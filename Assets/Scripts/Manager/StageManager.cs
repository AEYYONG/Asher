using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public StageInfoSO stageSO;
    private Player_Move player;
    private NPC_Move npc;

    void Start()
    {
        player = FindObjectOfType<Player_Move>();
        npc = FindObjectOfType<NPC_Move>();
        StopAllCharacterMove();
        StartCoroutine(GameStart());
    }

    void Update()
    {
        
    }

    IEnumerator GameStart()
    {
        //3,2,1 후 게임 시작
        VFXManager.Instance.PlayVFX("StartTimer",FindObjectOfType<StageUIManager>().transform);
        yield return new WaitForSeconds(4.5f);
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
}
