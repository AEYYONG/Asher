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
    [SerializeField] private Animator _animator;
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
    //타일을 클릭하였을 때
    void OnMouseDown()
    {
        //선택되지 않은 타일이라면 && 상호작용 가능하다면
        if (!isSelected && _playerInteract.canInteract && tileType!=TileType.RandomNotAvail)
        {
            //타일 선택 횟수 하나 증가
            _playerInteract.IncSelectCnt();
            //선택 여부 true로 변경
            isSelected = true;
            //뒤집기 애니메이션 시작
            _animator.SetTrigger("Select");
            
            //타일 아이디 값 저장
            _playerInteract.AddTile(this);
        }
    }

    //타일이 다시 원상복귀
    public IEnumerator ReturnTile()
    {
        yield return new WaitForSeconds(_tileManager.tileReturnTime);
        //되돌아오는 애니메이션 실행
        _animator.SetTrigger("Return");
        //선택 여부 false로 변경
        isSelected = false;
        //yield return new WaitForSeconds(0.2f);
        //PlayerInteract 값 초기화
        _playerInteract.InitValue();
    }

    //타일 위치 반환
    public Vector2Int ReturnPos()
    {
        return new Vector2Int(_x, _z);
    }

    //타일이 사용될 경우 발동되는 가상 함수
    public virtual void Use()
    {
        
    }
    
}
