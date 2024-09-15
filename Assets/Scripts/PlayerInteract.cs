using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct TileInfo
{
    public char tileID;
    public Vector2Int tilePos;
}
public class PlayerInteract : MonoBehaviour
{
    //최대 타일 선택 횟수
    [SerializeField] private int maxSelectCnt = 2;
    //현재 타일 선택 횟수
    private int _curSelectCnt;
    //각 타일의 정보
    private TileInfo _tile1Info;
    private TileInfo _tile2Info;
    //타일을 뒤집을 수 있는지 여부
    public bool canInteract;
    //타일 검사 flag
    private bool _compareStart;
    //타일 매니저 스크립트
    private TileManager _tileManager;


    void Awake()
    {
        _tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        _curSelectCnt = 0;
        canInteract = true;
        _compareStart = false;
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
            if (!CompareTile(_tile1Info, _tile2Info))
            {
                //타일이 같지 않다면 타일 원상복귀
                _tileManager.ReturnTile(_tile1Info.tilePos,_tile2Info.tilePos);
            }
            else
            {
                //타일이 같으면 아이템 획득 후, 값 초기화(다시 타일 선택할 수 있도록)
                InitValue();
            }
        }
    }

    bool CompareTile(TileInfo info1, TileInfo info2)
    {
        //두 타일의 아이디 값이 같다면(같은 타일이라면)
        if (info1.tileID == info2.tileID)
        {
            if (info1.tileID == '0')
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
    public void SetTile1(char id, Vector2Int pos)
    {
        _tile1Info.tileID = id;
        _tile1Info.tilePos = pos;
        Debug.Log($"First Tile : Tile{id} {pos}");
    }
    public void SetTile2(char id, Vector2Int pos)
    {
        _tile2Info.tileID = id;
        _tile2Info.tilePos = pos;
        Debug.Log($"Second Tile : Tile{id} {pos}");
    }

    public void InitValue()
    {
        //값 초기화
        _curSelectCnt = 0;
        _compareStart = false;
        canInteract = true;
    }
    
}
