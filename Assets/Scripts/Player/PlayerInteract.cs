using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

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
    private RaycastHit _hit;
    private Vector3 _rayPos;
    
    //그린존 사용여부 flag 변수
    //한번 사용하면 더는 추격 상황에서 그린존이 활성화되지 않음.
    public bool useGreenZone = false;
    
    //최근 타일 리스트
    [SerializeField] private int _maxRecentTile;
    public LinkedList<Tile> _recentTiles = new LinkedList<Tile>();
    
    //피버 타임인지
    private InventoryManager _inventoryManager;
    public bool isFever = false;
    
    //애셔 초상화
    [SerializeField] private PortraitTest _asherPortrait;
    
    //sfx 재생
    public AudioSource audioSource;

    public class FeverTile
    {
        public Tile tile;
        public bool isComplete; //짝을 맞추었는지
    }

    void Awake()
    {
        _tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        _curSelectCnt = 0;
        canInteract = true;
        _compareStart = false;
    }

    void Start()
    {
        _inventoryManager = FindObjectOfType<InventoryManager>();
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
        _rayPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
        Debug.DrawRay(_rayPos,Vector3.down * 1f, Color.red);
        //플레이어가 타일 뒤집기(space)를 클릭한다면
        if (Input.GetButtonUp("FlipTile"))
        {
            if (Physics.Raycast(_rayPos, Vector3.down, out _hit, 1f))
            {
                Tile curTile = _hit.collider.GetComponent<Tile>();
                if (isFever)
                {
                    //피버타일 이펙트 실행
                    Vector3 vfxPos = transform.position + new Vector3(0, 0, -0.5f);
                    VFXManager.Instance.PlayVFX("FeverTimeTileEffect", vfxPos);
                    List<Tile> nearTiles = curTile.GetNearTiles();
                    List<FeverTile> compareTiles = new List<FeverTile>();
                    List<Tile> returnTiles = new List<Tile>();
                    foreach (var tile in nearTiles)
                    {
                        if (!tile.isSelected && canInteract && tile.tileType != TileType.RandomNotAvail)
                        {
                            //선택 여부 true로 변경
                            tile.isSelected = true;
                            //뒤집기 애니메이션 시작
                            tile._animator.SetTrigger("Select");
                            //뒤집힌 피버 타일들 비교를 위해 비교 타일에 집어넣기
                            FeverTile feverTile = new FeverTile();
                            feverTile.tile = tile;
                            feverTile.isComplete = false;
                            compareTiles.Add(feverTile);
                        }
                    }
                    //compareTiles에 있는 타일들을 비교
                    CompareNearTiles(compareTiles,returnTiles);
                }
                else if (!curTile.isSelected && canInteract && curTile.tileType!=TileType.RandomNotAvail)
                {   //선택되지 않은 타일이라면 && 상호작용 가능하다면
                    //타일 선택 횟수 하나 증가
                    _curSelectCnt++;
                    curTile.tileSO.selectNum = _curSelectCnt;
                    //선택 여부 true로 변경
                    curTile.isSelected = true;
                    //뒤집기 애니메이션 시작
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxDictionary["SFX_TileFlip"]);
                    curTile._animator.SetTrigger("Select");
            
                    //타일 아이디 값 저장
                    _tiles.Add(curTile);
                }
            }
        }
        
        //선택한 첫번째 카드가 함정 카드라면
        if (_tiles.Count > 0 && _tiles[0].tileSO.tileID == TileID.Trap && canInteract)
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
        //이 부분이 NPC 정면 확인 코드로 이동되면서 그린존 타일이 앞에 감지되면 멈추고 애니메이션 defend 실행되도록 코드 수정 필요
        /*
        if (_tileManager.isGreenZoneActive && !useGreenZone)
        {
            if (Physics.Raycast(_rayPos, Vector3.down, out _hit, 1f))
            {
                Tile curTile = _hit.collider.GetComponent<Tile>();
                //그린존 타일로 들어왔다면
                if (curTile.tileSO.tileID == TileID.GreenZone)
                {
                    Debug.Log("그린존 입성");
                    useGreenZone = true;
                    curTile.Use(_stageUIManager);
                }
            }
        }*/
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
                    //최근 선택 타일 스택에 저장
                    AddRecentTileList(tile1);
                    AddRecentTileList(tile2);
                    break;
                case TileID.HeartStone:
                    Debug.Log("Heart Piece Tile");
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxDictionary["SFX_Matching_HeartGem_Success"]);
                    StartTileMatchEffect(tile1, tile2);
                    tile1.Use(_stageUIManager);
                    _asherPortrait.SetGood();
                    StageManager.Instance.UpdateHeartStoneScore();
                    StartCoroutine(InvokeInitValue());
                    break;
                case TileID.Item:
                    if (tile1.tileSO.itemID == tile2.tileSO.itemID)
                    {
                        Debug.Log("Same Item Tile");
                        StartTileMatchEffect(tile1, tile2);
                        inventory.AddItemEvent(tile1);
                        _asherPortrait.SetGood();
                        Debug.Log("픽업 vfx");
                        StageManager.Instance.UpdateItemScore();
                        VFXManager.Instance.PlayVFX("GetItem",_stageUIManager.player.transform);
                        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxDictionary["SFX_Matching_Item_Success"]);
                        StartCoroutine(InvokeInitValue());
                        break;
                    }
                    Debug.Log("Not Same Item Tile"); 
                    _tileManager.ReturnTile(_tiles);
                    AddRecentTileList(tile1);
                    AddRecentTileList(tile2);
                    break;
                case TileID.Joker:
                    Debug.Log("Red and Black Joker");
                    _tileManager.ReturnTile(_tiles);
                    AddRecentTileList(tile1);
                    AddRecentTileList(tile2);
                    break;
                default:
                    _tileManager.ReturnTile(_tiles);
                    AddRecentTileList(tile1);
                    AddRecentTileList(tile2);
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
                StartTileMatchEffect(tile1, tile2);
                inventory.AddItemEvent(tile2);
                _asherPortrait.SetGood();
                StageManager.Instance.UpdateItemScore();
                VFXManager.Instance.PlayVFX("GetItem",_stageUIManager.player.transform);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxDictionary["SFX_Matching_Item_Success"]);
                StartCoroutine(InvokeInitValue());
            }
            else if (id1 == TileID.Item && id2 == TileID.Joker)
            {
                Debug.Log("Joker and Item");
                StartTileMatchEffect(tile1, tile2);
                inventory.AddItemEvent(tile1);
                _asherPortrait.SetGood();
                StageManager.Instance.UpdateItemScore();
                VFXManager.Instance.PlayVFX("GetItem",_stageUIManager.player.transform);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxDictionary["SFX_Matching_Item_Success"]);
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
                AddRecentTileList(tile1);
                AddRecentTileList(tile2);
            }
            //InitValue();
        }
    }

    //피버타일로 인한 인접 타일들 비교 함수
    public void CompareNearTiles(List<FeverTile> tiles, List<Tile> returnTiles)
    {
        List<Tile> itemList = new List<Tile>();
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].isComplete || tiles[i].tile.tileSO.tileID != TileID.Item)
            {
                continue;
            }
            for (int j = i+1; j < tiles.Count; j++)
            {
                if (tiles[j].isComplete || tiles[j].tile.tileSO.tileID != TileID.Item)
                {
                    continue;
                }

                if (tiles[i].tile.tileSO.itemID == tiles[j].tile.tileSO.itemID)
                {
                    tiles[i].isComplete = true;
                    tiles[j].isComplete = true;
                    //획득 아이템 리스트에 추가
                    itemList.Add(tiles[i].tile);
                    break;
                }
            }
        }

        for (int i = 0; i < tiles.Count(); i++)
        {
            if (!tiles[i].isComplete)
            {
                returnTiles.Add(tiles[i].tile);
            }
        }
        SelectFeverItems(itemList, returnTiles);
    }
    
    //피버 타일로 획득한 아이템들 중 획득할 아이템을 선택하는 함수
    public void SelectFeverItems(List<Tile> items, List<Tile> returnTiles)
    {
        //인벤토리가 비어있는 개수만큼 아이템이 존재하면 인벤토리에 넣기
        int availCnt = 2 - _inventoryManager.GetItemCnt();
        
        //획득한 아이템 수가, 획득 가능한 아이템 수보다 같거나 작으면 -> 획득한 아이템 그대로 인벤토리에 넣으면 됨.
        if (availCnt >= items.Count)
        {
            foreach (var item in items)
            {
                StageManager.Instance.UpdateItemScore();
                inventory.AddItemEvent(item);
            }
        }
        else
        {
            //획득한 아이템 수가, 획득 가능한 아이템 수보다 많으면
            //아이템 선택 시스템 발동
            Debug.Log("선택 시스템 발동");
            foreach (var item in items)
            {
                Debug.Log(item.name + "\n");
            }
            Debug.Log("중에 선택 가능");
            _stageUIManager.DrawItemSelectionUI(items);
        }
        //다 선택하면 이제 뒤집어져야 하는 타일들 뒤집기
        _tileManager.ReturnTile(returnTiles);
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
        item.script.ItemUse(_stageUIManager);
    }

    public IEnumerator InvokeInitValue()
    {
        yield return new WaitForSeconds(_tileManager.tileReturnTime);
        InitValue();
    }

    public void AddRecentTileList(Tile tile)
    {
        if (!_recentTiles.Contains(tile))
        {
            if (_recentTiles.Count == _maxRecentTile)
            {
                Debug.Log("5개 꽉차서, 가장 처음에 선택한 " + _recentTiles.First().name + " 제거");
                _recentTiles.RemoveFirst();
            }
            Debug.Log(tile.name+"을 최근 리스트에 추가");
            _recentTiles.AddLast(tile);
        }
    }

    public void StartTileMatchEffect(Tile tile1, Tile tile2)
    {
        StartCoroutine(tile1.StartTileMatchEffect());
        StartCoroutine(tile2.StartTileMatchEffect());
    }
}
