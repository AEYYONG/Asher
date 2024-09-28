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
        //선택 가능한 타일을 모두 선택하였을 때
        if (_curSelectCnt == maxSelectCnt && !_compareStart)
        {
            //상호작용 불가능함
            canInteract = false;
            
            //타일 비교
            _compareStart = true;
            if (!CompareTile(_tiles[0], _tiles[1]))
            {
                //타일이 같지 않다면 타일 원상복귀
                _tileManager.ReturnTile(_tiles);
            }
            else
            {
                //현재까지 선택된 타일이 모두 이벤트 타일들이 아니라면
                if (_tiles[0].tileSO.tileID != TileID.Event)
                {
                    //버프 아이템인 경우 인벤토리에 습득
                    if (_tiles[0].tileSO.tileID == TileID.Item)
                    {
                        Debug.Log("get item");
                        ItemTile item = _tiles[0].GetComponent<ItemTile>();
                        inventory.AddItemEvent(item);
                    }
                    else if (_tiles[0].tileSO.tileID == TileID.Trap)
                    {
                        //디버프 아이템인 경우, 인벤토리에 저장 없이 바로 실행
                        TrapTile trap = _tiles[0].GetComponent<TrapTile>();
                        trap.Use(gameObject);
                    }
                    
                    //이벤트 타일이 아니면서 타일이 서로 같으면 아이템 획득 후, 값 초기화(다시 타일 선택할 수 있도록)
                    InitValue();
                }
                else
                {
                    //선택된 타일이 모두 이벤트 타일이라면
                    canInteract = true;
                }
            }
        }
        
        //이벤트 타일에 대해 n-1번째 타일을 선택하였을 때(세번째 타일까지 왔다면, 첫번째 두번째 타일은 모두 이벤트 타일인 것.
        if (_curSelectCnt < _tileManager.eventTileCnt && _curSelectCnt > maxSelectCnt && _tileManager.eventTileCnt > 0)
        {
            /*
            if (_tileInfos[_curSelectCnt-1].tileID != TileID.Event)
            {
                //타일이 같지 않다면 타일 원상복귀
                Debug.Log("이벤트 타일 debug");
                _tileManager.ReturnTile(_tileInfos);
                InitValue();
            }
            */
        }
        else if (_curSelectCnt == _tileManager.eventTileCnt && _tileManager.eventTileCnt > 0)
        {
            /*
            if (_tileInfos[_curSelectCnt-1].tileID != TileID.Event)
            {
                //타일이 같지 않다면 타일 원상복귀
                _tileManager.ReturnTile(_tileInfos);
                InitValue();
            }
            else
            {
                //이벤트 타일을 모두 뒤집었다면 실행할 함수
                
                //값 초기화
                InitValue();
            }
            */
        }
    }

    bool CompareTile(Tile tile1, Tile tile2)
    {
        TileID id1 = tile1.tileSO.tileID;
        TileID id2 = tile2.tileSO.tileID;
        //두 타일의 아이디 값이 같다면(같은 타일이라면)
        if (id1 == id2)
        {
            if (id1 == TileID.General)
            {
                Debug.Log("일반적인 타일");
                return false;
            }
            if (id1 == TileID.Item)
            {
                ItemType i1 = tile1.GetComponent<ItemTile>().itemSO.itemType;
                ItemType i2 = tile2.GetComponent<ItemTile>().itemSO.itemType;

                if (i1 != i2)
                {
                    return false;
                }
            }
            if (id1 == TileID.Trap)
            {
                TrapID t1 = tile1.GetComponent<TrapTile>().trapSO.trapID;
                TrapID t2 = tile2.GetComponent<TrapTile>().trapSO.trapID;
                
                if (t1 != t2)
                {
                    return false;
                }
            }
            return true;
        }
        //두 타일의 아이디 값이 같지 않다면
        return false;
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
        item.script.Use(this.gameObject);
    }
}
