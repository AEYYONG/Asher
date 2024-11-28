using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour
{
    public StageInfoSO stageSO;
    private int _curImgId = 0;
    private string _triggerName = "";
    [SerializeField] private GameObject heartStonesParent;
    [SerializeField] private List<Image> _heartStonesList = new List<Image>();
    [SerializeField] private GameObject _sideCutSceneUI;
    [SerializeField] private Image _sideCutSceneImg;
    [SerializeField] private Animator _sideCutSceneImgAnimator;
    //0: 함정, 1: 아이템, 2: 마음의 조각, 3: 조커, 4: 그린존, 5: 타이머
    
    //외부 스크립트에서 참조하기 위한 플레이어 변수
    public GameObject player;
    public GameObject npc;
    
    //피버타임 시, 아이템 획득 관련 UI
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _itemSelectionPrefab;
    [SerializeField] private List<ItemSelectUI> _itemSelectionUIList = new List<ItemSelectUI>();
    public float itemSelectionDuration;
    [SerializeField] private bool _itemSelectStart = false;
    [SerializeField] private Tile _selectItem;
    private bool _isSelect = false;
    private float _timer = 0f;
    [SerializeField] private InventoryManager _inventoryManager;
    public TileManager tileManager;
    class ItemSelectUI
    {
        public GameObject selectionUI;
        public Tile script;

        public ItemSelectUI(GameObject ui, Tile tile)
        {
            selectionUI = ui;
            script = tile;
        }
    }
    void Awake()
    {
        InitHeartStonesList();
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        npc = GameObject.FindWithTag("NPC");
        tileManager = FindObjectOfType<TileManager>();
    }
    
    void Update()
    {
        if (_itemSelectStart && !_isSelect)
        {
            if (Input.GetButtonUp("SelectLeftItem"))
            {
                _selectItem = _itemSelectionUIList[0].script;
                _isSelect = true;
            }
            else if (Input.GetButtonUp("SelectRightItem"))
            {
                _selectItem = _itemSelectionUIList[1].script;
                _isSelect = true;
            }
            else if (_itemSelectionUIList.Count >= 3 && Input.GetButtonUp("SelectDownItem"))
            {
                _selectItem = _itemSelectionUIList[2].script;
                _isSelect = true;
            } 
            else if (_itemSelectionUIList.Count == 4 && Input.GetButtonUp("SelectUpItem"))
            {
                _selectItem = _itemSelectionUIList[3].script;
                _isSelect = true;
            }

            if (_selectItem != null)
            {
                _inventoryManager.SelectFeverTimeItem(_selectItem);
                Debug.Log(_selectItem.name + "아이템 선택");
            }
        }
    }

    void FixedUpdate()
    {
        if (_itemSelectStart)
        {
            _timer += Time.fixedDeltaTime;
            foreach (var ui in _itemSelectionUIList)
            {
                ui.selectionUI.transform.GetChild(0).GetComponent<Image>().fillAmount = _timer / itemSelectionDuration;
            }

            if (_isSelect)
            {
                Debug.Log("시간 내에 아이템 선택 완료");
                //선택한 아이템 획득
                //아이템 선택 UI 파괴
                DestroyItemSelectionUI();
            }

            if (_timer >= itemSelectionDuration)
            {
                Debug.Log("시간 내에 아이템 선택하지 못함");
                _itemSelectStart = false;
                DestroyItemSelectionUI();
            }
        }
    }

    public void UpdateHeartStoneUI()
    {
        stageSO.IncreaseHeartStoneCnt();
        Debug.Log(stageSO.GetHeartStoneCnt());
        _heartStonesList[stageSO.GetHeartStoneCnt()-1].color = Color.white;
    }

    void InitHeartStonesList()
    {
        //heart stone ui List에 담기
        _heartStonesList.Clear();
        for (int i = 0; i < stageSO.heartStoneTotalCnt; i++)
        {
            Image heartStone = heartStonesParent.transform.GetChild(i).GetComponent<Image>();
            heartStone.color = Color.black;
            _heartStonesList.Add(heartStone);
        }
    }

    public void LoseAllHeartStones()
    {
        //마음의 조각 다 잃는 애니메이션
        //마음의 조각 리스트 초기화
        _heartStonesList.Clear();
    }

    public void ActiveSideCutSceneUI(TileSO tileSO)
    {
        _sideCutSceneImg.sprite = tileSO.sideCutSceneImg;
        switch (tileSO.tileID)
        {
            case TileID.Item:
                _curImgId = (int)tileSO.itemID;
                _triggerName = "ItemId";
                break;
            case TileID.Joker:
                _curImgId = 1;
                _triggerName = "Joker";
                break;
            case TileID.Trap:
                _curImgId = (int)tileSO.trapID;
                _triggerName = "TrapId";
                break;
            case TileID.HeartStone:
                _curImgId = 1;
                _triggerName = "HeartStone";
                break;
            case TileID.GreenZone:
                _curImgId = 1;
                _triggerName = "GreenZone";
                break;
                
        }
        _sideCutSceneUI.SetActive(true);
        StartCoroutine(SetSideCutSceneUIImg(_triggerName, _curImgId));
    }

    IEnumerator SetSideCutSceneUIImg(string name, int id)
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("애니메이션 트리거 변수 셋");
        _sideCutSceneImgAnimator.SetInteger(name,id);
    }
    
    //아이템 획득 UI 그리기
    public void DrawItemSelectionUI(List<Tile> items)
    {
        _itemSelectionUIList.Clear();

        foreach (var item in items)
        {
            GameObject instance = Instantiate(_itemSelectionPrefab, this.gameObject.transform, true);
            instance.transform.GetChild(2).GetComponent<Image>().sprite = item.tileSO.itemImg;
            instance.transform.localScale *= Screen.width / 1920f;
            ItemSelectUI ui = new ItemSelectUI(instance,item);
            _itemSelectionUIList.Add(ui);
        }

        Vector3 playerPos = player.transform.position;
        Vector3 playerScreenPos = _camera.WorldToScreenPoint(playerPos);
        // 해상도에 따른 비율 조정
        _itemSelectionUIList[0].selectionUI.transform.position = playerScreenPos + new Vector3(-130 * Screen.width / 1920f, -30 * Screen.height / 1080f, 0);
        _itemSelectionUIList[1].selectionUI.transform.position = playerScreenPos + new Vector3(130 * Screen.width / 1920f, -30 * Screen.height / 1080f, 0);

        if (items.Count == 3)
        {
            _itemSelectionUIList[2].selectionUI.transform.position = playerScreenPos + new Vector3(0, -150 *  Screen.height / 1080f, 0);   
        }
        else if (items.Count == 4)
        {
            _itemSelectionUIList[2].selectionUI.transform.position = playerScreenPos + new Vector3(0, -150 *  Screen.height / 1080f, 0);
            _itemSelectionUIList[3].selectionUI.transform.position = playerScreenPos + new Vector3(0, 90 * Screen.height / 1080f, 0);   
        }
        
        //아이템 선택 시작
        _itemSelectStart = true;
    }
    
    public void DestroyItemSelectionUI()
    {
        foreach (var ui in _itemSelectionUIList)
        {
            Destroy(ui.selectionUI);
        }
        _itemSelectionUIList.Clear();
    }
}
