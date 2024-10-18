using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public enum TileType
{
    RandomAvail,
    RandomNotAvail,
    Event
}
public class Tile : MonoBehaviour
{
    //TileSO
    public TileSO tileSO;
    //타일 x값, z 값
    public int _x, _z;
    //타일이 선택되었는지
    public bool isSelected;
    public TileType tileType;
    
    //타일 애니메이터
    public Animator _animator;
    //타일 매니저 스크립트 할당
    private TileManager _tileManager;
    //플레이어 상호작용 스크립트 할당
    private PlayerInteract _playerInteract;

    void Awake()
    {
        //타일 선택 여부 초기화
        isSelected = false;
        //타일 애니메이터 할당
        _animator = GetComponent<Animator>();
        //타일 매니저 스크립트 할당
        _tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        //플레이어 상호작용 스크립트 할당
        _playerInteract = GameObject.FindWithTag("Player").GetComponent<PlayerInteract>();
    }
    //타일 초기화
    public void InitTile(int x, int z)
    {
        _x = x;
        _z = z;
        tileType = TileType.RandomAvail;
    }

    //타일이 다시 원상복귀
    public IEnumerator ReturnTile()
    {
        yield return new WaitForSeconds(_tileManager.tileReturnTime);
        //되돌아오는 애니메이션 실행
        _animator.SetTrigger("Return");
        //선택 여부 false로 변경
        isSelected = false;
    }

    //타일 위치 반환
    public Vector2Int ReturnPos()
    {
        return new Vector2Int(_x, _z);
    }

    //타일이 사용될 경우 발동되는 가상 함수
    public virtual void ItemUse(StageUIManager uiManager)
    {
        uiManager.ActiveSideCutSceneUI(tileSO);
    }

    public virtual void TrapUse(StageUIManager uiManager)
    {
        uiManager.ActiveSideCutSceneUI(tileSO);
    }
    
    public virtual void Use(){}
    
}
