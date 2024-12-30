using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleTrap : Tile
{
    private Vector3 _playerPos;
    private Vector3 _npcPos;
    
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("블랙홀 아이템 사용");
        
        //NPC 옆에 느낌표 띄우기
        VFXManager.Instance.PlayVFX("TrapEmotion", uiManager.npc.transform.GetChild(0).transform);
        
        StartCoroutine(SwitchPosition(uiManager));
        StartCoroutine(Flat(uiManager));
    }

    IEnumerator SwitchPosition(StageUIManager uiManager)
    {
        //플레이어와 NPC 위치 읽어오기
        //플레이어도 딱 타일 위치로 이동하기
        uiManager.player.transform.position = new Vector3(transform.position.x, uiManager.player.transform.position.y, transform.position.z+0.5f);
        _playerPos = uiManager.player.transform.position;
        //npc의 경우 위치를 정수로 반올림해주고, 해당 위치로 이동시키기
        uiManager.npc.GetComponent<NPC_Move>().agent.enabled = false;
        _npcPos = uiManager.npc.transform.position;
        _npcPos = new Vector3((float)Math.Round(_npcPos.x), _npcPos.y, (float)Math.Round(_npcPos.z)-0.3f);
        uiManager.npc.transform.position = _npcPos;
        uiManager.npc.GetComponent<NPC_Move>().agent.enabled = true;
        
        //플레이어와 NPC 움직임 멈추기
        StageManager.Instance.StopAllCharacterMove();
        
        //플레이어와 NPC 발 밑에 블랙홀 나타나기
        ShowBlackHole(_npcPos, _playerPos);
        
        yield return new WaitForSeconds(tileSO.duration);
        
        //플레이어와 NPC 위치 바꾸기
        Vector3 tempNpcPos = new Vector3(_playerPos.x, _npcPos.y, _playerPos.z - 0.8f);
        Vector3 tempPlayerPos = new Vector3(_npcPos.x, _playerPos.y, _npcPos.z + 0.8f);

        _npcPos = tempNpcPos;
        _playerPos = tempPlayerPos;
        
        uiManager.npc.GetComponent<NPC_Move>().agent.enabled = false;
        uiManager.player.transform.position = _playerPos;
        uiManager.npc.transform.position = _npcPos;
        uiManager.npc.GetComponent<NPC_Move>().agent.enabled = true;
        
        Debug.Log($"#npc pos : {uiManager.npc.transform.position.x},{uiManager.npc.transform.position.y},{uiManager.npc.transform.position.z}");
        Debug.Log($"#player pos : {uiManager.player.transform.position.x},{uiManager.player.transform.position.y},{uiManager.player.transform.position.z}");
        
        ShowBlackHoleReturn(_npcPos,_playerPos);
        
        //납작해진 플레이어와 NPC의 애니메이션 원상복구 강제 변경
        uiManager.player.GetComponent<Animator>().Play("BlackHole_Flat_Return");
        uiManager.npc.GetComponent<Animator>().Play("BlackHole_Flat_Return");
        
        yield return new WaitForSeconds(0.5f);
        
        //플레이어와 NPC 다시 움직이게 하기
        StageManager.Instance.StartAllCharacterMove();
        
        //vfx 실행
        Animator effectAnimator = transform.GetChild(0).GetComponent<Animator>();
        effectAnimator.SetTrigger("TrapMatch");
    }

    //블랙홀 나타나게 하는 함수
    void ShowBlackHole(Vector3 npcPos, Vector3 playerPos)
    {
        Vector3 npcBlackholePos = new Vector3(npcPos.x, 0.4f, npcPos.z+0.3f);
        Vector3 playerBlackholePos = new Vector3(playerPos.x, 0.4f, playerPos.z-0.5f);
        
        VFXManager.Instance.PlayVFX("BlackHole",npcBlackholePos);
        VFXManager.Instance.PlayVFX("BlackHole",playerBlackholePos);
    }
    
    void ShowBlackHoleReturn(Vector3 npcPos, Vector3 playerPos)
    {
        Vector3 npcBlackholePos = new Vector3(npcPos.x, 0.4f, npcPos.z+0.3f);
        Vector3 playerBlackholePos = new Vector3(playerPos.x, 0.4f, playerPos.z-0.5f);
        
        VFXManager.Instance.PlayVFX("BlackHole_Return",npcBlackholePos);
        VFXManager.Instance.PlayVFX("BlackHole_Return",playerBlackholePos);
    }

    IEnumerator Flat(StageUIManager uiManager)
    {
        yield return new WaitForSeconds(1.8f);
        //플레이어와 NPC 납작해지는 애니메이션 실행하기
        uiManager.player.GetComponent<Animator>().Play("BlackHole_Flat");
        uiManager.npc.GetComponent<Animator>().Play("BlackHole_Flat");
    }
}
