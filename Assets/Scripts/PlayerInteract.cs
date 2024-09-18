using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct TileInfo
{
    public TileID tileID;
    public Vector2Int tilePos;
}
public class PlayerInteract : MonoBehaviour
{
    //최대 타일 선택 횟수
    [SerializeField] private int maxSelectCnt = 2;
    //현재 타일 선택 횟수
    private int _curSelectCnt;
    //각 타일의 정보
    private List<TileInfo> _tileInfos = new List<TileInfo>();
    //타일을 뒤집을 수 있는지 여부
    public bool canInteract;
    //타일 검사 flag
    private bool _compareStart;
    //타일 매니저 스크립트
    private TileManager _tileManager;
    //인벤토리 오브젝트
    private Inventory _inventory;
    


    void Awake()
    {
        _tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        _curSelectCnt = 0;
        canInteract = true;
        _compareStart = false;
        _inventory = FindObjectOfType<Inventory>();
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
            if (!CompareTile(_tileInfos[0], _tileInfos[1]))
            {
                //타일이 같지 않다면 타일 원상복귀
                _tileManager.ReturnTile(_tileInfos);
            }
            else
            {
                //현재까지 선택된 타일이 모두 이벤트 타일들이 아니라면
                if (_tileInfos[0].tileID != TileID.Event)
                {
                    //습득 아이템인 경우
                    //인벤토리의 slot1이 비어있다면
                    if (!_inventory.isFirstSlotFull)
                    {
                        //slot1에 해당 아이템 추가
                        _inventory.AddItem(1,_tileInfos[0].tileID);
                    }
                    else
                    {
                        _inventory.AddItem(2,_tileInfos[0].tileID);
                    }
                    
                    //이벤트 타일이 아니면서 타일이 서로 같으면 아이템 획득 후, 값 초기화(다시 타일 선택할 수 있도록)
                    InitValue();
                }
                else
                {
                    canInteract = true;
                }
            }
        }
        
        //이벤트 타일에 대해 n-1번째 타일을 선택하였을 때(세번째 타일까지 왔다면, 첫번째 두번째 타일은 모두 이벤트 타일인 것.
        if (_curSelectCnt < _tileManager.eventTileCnt && _curSelectCnt > maxSelectCnt)
        {
            if (_tileInfos[_curSelectCnt-1].tileID != TileID.Event)
            {
                //타일이 같지 않다면 타일 원상복귀
                Debug.Log("이벤트 타일 debug");
                _tileManager.ReturnTile(_tileInfos);
                InitValue();
            }
        }
        else if (_curSelectCnt == _tileManager.eventTileCnt)
        {
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
        }
    }

    bool CompareTile(TileInfo info1, TileInfo info2)
    {
        //두 타일의 아이디 값이 같다면(같은 타일이라면)
        if (info1.tileID == info2.tileID)
        {
            if (info1.tileID == TileID.General)
            {
                Debug.Log("일반적인 타일");
                return false;
            }
            Debug.Log($"Tile{info1.tileID} == Tile{info2.tileID}");
            return true;
        }
        //두 타일의 아이디 값이 같지 않다면
        Debug.Log($"Tile{info1.tileID} != Tile{info2.tileID}");
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
    public void AddTile(TileID id, Vector2Int pos)
    {
        TileInfo info = new TileInfo();
        info.tileID = id;
        info.tilePos = pos;
        Debug.Log("타일 info에 추가");
        _tileInfos.Add(info);
    }

    public void InitValue()
    {
        //값 초기화
        _curSelectCnt = 0;
        _compareStart = false;
        canInteract = true;
        //선택된 타일 정보가 담긴 리스트 초기화
        _tileInfos.Clear();
    }
    
}
