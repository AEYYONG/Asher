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

    //인접 타일 리스트
    private List<Tile> nearTiles = new List<Tile>();

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

    public virtual void Use(StageUIManager uiManager)
    {
        uiManager.ActiveSideCutSceneUI(tileSO);
    }
    
    //피버 타임 시, 주변 8개의 타일 정보 가져오기
    public List<Tile> GetNearTiles()
    {
        int width = _tileManager.width;
        int height = _tileManager.height;

        int left = _x - 1;
        int right = _x + 1;
        int bottom = _z - 1;
        int top = _z + 1;
        
        for (int i = bottom; i <= top; i++)
        {
            for (int j = left; j <= right; j++)
            {
                if (!(i < 0 || i >= height || j < 0 || j >= width))
                {
                    Vector2Int tile = new Vector2Int(j, i);
                    if (_tileManager._tiles.ContainsKey(tile))
                    {
                        nearTiles.Add(_tileManager._tiles[tile].GetComponent<Tile>());
                    }
                }
            }
        }

        return nearTiles;
    }
}
