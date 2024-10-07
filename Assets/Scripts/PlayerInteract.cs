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
    //타일을 뒤집을 수 있는지 여부
    public bool canInteract;
    //타일 검사 flag
    private bool _compareStart;
    //타일 매니저 스크립트
    private TileManager _tileManager;
    
    //아이템 추가 이벤트
    public InventorySO inventory;
    
    
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
            _tiles[0].Use();
            InitValue();
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
        
        //이벤트 타일에 대해 n-1번째 타일을 선택하였을 때(세번째 타일까지 왔다면, 첫번째 두번째 타일은 모두 이벤트 타일인 것.
        if (_curSelectCnt <= _tileManager.eventTileCnt && _curSelectCnt > maxSelectCnt && _tileManager.eventTileCnt > 0)
        {
            if (_tiles[_curSelectCnt-1].tileSO.tileID != TileID.Event)
            {
                //타일이 같지 않다면 타일 원상복귀
                Debug.Log("Not Event Tile");
                _tileManager.ReturnTile(_tiles);
                InitValue();
            }
            else if (_curSelectCnt == _tileManager.eventTileCnt &&
                     _tiles[_curSelectCnt - 1].tileSO.tileID == TileID.Event)
            {
                //이벤트 타일을 모두 뒤집었다면 실행할 함수
                _tiles[_curSelectCnt-1].Use();
                //값 초기화
                InitValue();
            }
        }
    }

    void CompareTile(Tile tile1, Tile tile2)
    {
        // 서로 다른 타일(일반 타일, 그린존 타일 포함) -> 다시 뒤집기
        // 서로 같은 아이템 타일 -> 획득
        // 마음의 조각 타일 -> 획득
        // 모두 조커 타일 -> 다시 뒤집기
        // 모두 이벤트 타일 -> 다음 타일들 확인
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
                    InitValue();
                    break;
                case TileID.HeartStone:
                    Debug.Log("Heart Piece Tile");
                    InitValue();
                    break;
                case TileID.Item:
                    if (tile1.tileSO.itemID == tile2.tileSO.itemID)
                    {
                        Debug.Log("Same Item Tile");
                        inventory.AddItemEvent(tile1);
                        InitValue();
                        break;
                    }
                    Debug.Log("Not Same Item Tile"); 
                    InitValue();
                    break;
                case TileID.Joker:
                    Debug.Log("Red and Black Joker");
                    _tileManager.ReturnTile(_tiles);
                    InitValue();
                    break;
                case TileID.Event:
                    Debug.Log("Select Two Event Tile");
                    canInteract = true; //모두 이벤트 타일이라면, 계속 타일을 선택할 수 있는 기회가 주어짐.
                    break;
                default:
                    _tileManager.ReturnTile(_tiles);
                    InitValue();
                    break;
            }
        }
        else
        {
            //두 타일의 타입이 같지 않지만 조커와 아이템의 조합이라면
            if (id1 == TileID.Joker && id2 == TileID.Item)
            {
                Debug.Log("Joker and Item");
                inventory.AddItemEvent(tile2);
            }
            else if (id1 == TileID.Item && id2 == TileID.Joker)
            {
                Debug.Log("Joker and Item");
                inventory.AddItemEvent(tile1);
            }
            else if (id2 == TileID.Trap)
            {
                tile2.Use();
            }
            else
            {
                _tileManager.ReturnTile(_tiles);
            }
            InitValue();
        }
    }
    public void IncSelectCnt()
    {
        _curSelectCnt++;
        Debug.Log("타일 선택 횟수 증가");
    }
    public int GetCurSelectCnt()
    {
        Debug.Log($"현재 타일 횟수 : {_curSelectCnt}");
        return _curSelectCnt;
    }

    //타일을 선택하였을 경우, 리스트에 추가함
    
    public void AddTile(Tile tile)
    {
        Debug.Log("타일 info에 추가");
        _tiles.Add(tile);
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
        item.script.Use();
    }
}
