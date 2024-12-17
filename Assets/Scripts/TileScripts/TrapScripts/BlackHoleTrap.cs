using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleTrap : Tile
{
    private Vector3 _playerPos;
    private Vector3 _npcPos;
    private Vector3 _npcRayPos;
    private RaycastHit _npcRayHit;
    private bool _isDetect = false;
    void Update()
    {
        if (_isDetect)
        {
            _npcRayPos = new Vector3(_npcPos.x, _npcPos.y, _npcPos.z);
            Debug.DrawRay(_npcRayPos,Vector3.down * 1f, Color.blue);
            if (Physics.Raycast(_npcRayPos, Vector3.down, out _npcRayHit, 1f))
            {
                if (_npcRayHit.collider.CompareTag("Ground"))
                {
                    Vector2Int npcTilePos = _npcRayHit.collider.GetComponent<Tile>().ReturnPos();
                    _npcPos = new Vector3(npcTilePos.x, _npcPos.y, npcTilePos.y);
                    _isDetect = false;
                }
            }
        }
    }
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("블랙홀 아이템 사용");
        StartCoroutine(SwitchPosition(uiManager));
    }

    IEnumerator SwitchPosition(StageUIManager uiManager)
    {
        //플레이어와 NPC 위치 읽어오기
        _npcPos = uiManager.npc.transform.position;
        _playerPos = uiManager.player.transform.position;
        _isDetect = true;
        //플레이어와 NPC 움직임 멈추기

        yield return new WaitForSeconds(tileSO.duration);
        //플레이어와 NPC 위치 바꾸기
        uiManager.npc.transform.position = new Vector3(_playerPos.x, _npcPos.y, _playerPos.z);
        uiManager.player.transform.position = new Vector3(_npcPos.x, _playerPos.y, _npcPos.z);
        //플레이어와 NPC 다시 움직이게 하기
        
        //vfx 실행
        Animator effectAnimator = transform.GetChild(0).GetComponent<Animator>();
        effectAnimator.SetTrigger("TrapMatch");
    }
}
