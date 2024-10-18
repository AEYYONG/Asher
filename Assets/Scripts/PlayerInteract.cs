using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    //최대 타일 선택 횟수
    [SerializeField] private int maxSelectCnt = 2;
    //현재 타일 선택 횟수
    [SerializeField] int _curSelectCnt;
    //각 타일의 정보
    private List<Tile> _tiles = new List<Tile>();
    public int GetSelectedTilesCnt() { return _tiles.Count; }
    //타일을 뒤집을 수 있는지 여부
    public bool canInteract;
    //타일 검사 flag
    private bool _compareStart;
    //타일 매니저 스크립트
    private TileManager _tileManager;
    
    //아이템 추가 이벤트
    public InventorySO inventory;
    
    //Stage UI Manager
    [SerializeField] private StageUIManager _stageUIManager;
    
    //타일을 뒤집기 위한 레이캐스트
    private RaycastHit hit;
    
    
    void Awake()
    {
        _tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        _curSelectCnt = 0;
        canInteract = true;
        _compareStart = false;
    }

    void OnEnable()
    {
        inventory.OnItemUsed += UseItem;
    }

    void OnDisable()
    {
        inventory.OnItemUsed -= UseItem;
    }

    void Update()
    {
        //선택한 첫번째 카드가 함정 카드라면
        if (_tiles.Count > 0 && _tiles[0].tileSO.tileID == TileID.Trap)
        {
            canInteract = false;
            _tiles[0].TrapUse(_stageUIManager);
            StartCoroutine(InvokeInitValue());
        }
        
        //선택 가능한 타일을 모두 선택하였을 때
        if (_curSelectCnt == maxSelectCnt && !_compareStart)
        {
            //상호작용 불가능함
            canInteract = false;
            
            //타일 비교
            _compareStart = true;
            CompareTile(_tiles[0], _tiles[1]);
        }
        
        //플레이어가 타일 뒤집기(space)를 클릭한다면
        if (Input.GetButtonUp("FlipTile"))
        {
            Debug.DrawRay(transform.position,Vector3.down * 2f, Color.red);
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f)){}
            {
                Tile curTile = hit.collider.GetComponent<Tile>();
                //선택되지 않은 타일이라면 && 상호작용 가능하다면
                if (!curTile.isSelected && canInteract && curTile.tileType!=TileType.RandomNotAvail)
                {
                    //타일 선택 횟수 하나 증가
                    _curSelectCnt++;
                    curTile.tileSO.selectNum = _curSelectCnt;
                    //선택 여부 true로 변경
                    curTile.isSelected = true;
                    //뒤집기 애니메이션 시작
                    curTile._animator.SetTrigger("Select");
            
                    //타일 아이디 값 저장
                    _tiles.Add(curTile);
                }
            }
        }
    }

    void CompareTile(Tile tile1, Tile tile2)
    {
        // 서로 다른 타일(일반 타일, 그린존 타일 포함) -> 다시 뒤집기
        // 서로 같은 아이템 타일 -> 획득
        // 마음의 조각 타일 -> 획득
        // 모두 조커 타일 -> 다시 뒤집기
        // 아이템 타일과 조커 타일 -> 획득
        TileID id1 = tile1.tileSO.tileID;
        TileID id2 = tile2.tileSO.tileID;
        //두 타일의 아이디 값이 같다면(같은 타입의 타일이라면)
        if (id1 == id2)
        {
            switch (id1)
            {
                case TileID.General:
                    Debug.Log("General Tile");
                    _tileManager.ReturnTile(_tiles);
                    break;
                case TileID.HeartStone:
                    Debug.Log("Heart Piece Tile");
                    tile1.Use();
                    StartCoroutine(InvokeInitValue());
                    break;
                case TileID.Item:
                    if (tile1.tileSO.itemID == tile2.tileSO.itemID)
                    {
                        Debug.Log("Same Item Tile");
                        inventory.AddItemEvent(tile1);
                        StartCoroutine(InvokeInitValue());
                        break;
                    }
                    Debug.Log("Not Same Item Tile"); 
                    _tileManager.ReturnTile(_tiles);
                    break;
                case TileID.Joker:
                    Debug.Log("Red and Black Joker");
                    _tileManager.ReturnTile(_tiles);
                    break;
                default:
                    _tileManager.ReturnTile(_tiles);
                    break;
            }
            //InitValue();
        }
        else
        {
            //두 타일의 타입이 같지 않지만 조커와 아이템의 조합이라면
            if (id1 == TileID.Joker && id2 == TileID.Item)
            {
                Debug.Log("Joker and Item");
                inventory.AddItemEvent(tile2);
                StartCoroutine(InvokeInitValue());
            }
            else if (id1 == TileID.Item && id2 == TileID.Joker)
            {
                Debug.Log("Joker and Item");
                inventory.AddItemEvent(tile1);
                StartCoroutine(InvokeInitValue());
            }
            else if (id2 == TileID.Trap)
            {
                tile2.TrapUse(_stageUIManager);
                _tiles.Remove(tile2);
                _tileManager.ReturnTile(_tiles);
            }
            else
            {
                _tileManager.ReturnTile(_tiles);
            }
            //InitValue();
        }
    }
    public void InitValue()
    {
        //값 초기화
        _curSelectCnt = 0;
        _compareStart = false;
        canInteract = true;
        //선택된 타일 정보가 담긴 리스트 초기화
        _tiles.Clear();
    }
    
    public void UseItem(InventorySlot item)
    {
        item.script.ItemUse();
    }

    public IEnumerator InvokeInitValue()
    {
        yield return new WaitForSeconds(_tileManager.tileReturnTime);
        InitValue();
    }
}
