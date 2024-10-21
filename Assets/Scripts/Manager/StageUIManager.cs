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
    [SerializeField] private Image _sideCutSceneFrameTop;
    [SerializeField] private Image _sideCutSceneFrameBottom;
    [SerializeField] private List<Sprite> _sideCutSceneFrameTopList;
    [SerializeField] private List<Sprite> _sideCutSceneFrameBottomList;
    //0: 함정, 1: 아이템, 2: 마음의 조각, 3: 조커, 4: 그린존, 5: 타이머
    
    //외부 스크립트에서 참조하기 위한 플레이어 변수
    public GameObject player;
    
    void Awake()
    {
        IniteartStonesList();
        player = GameObject.FindWithTag("Player");
    }

    public void UpdateHeartStoneUI()
    {
        stageSO.IncreaseHeartStoneCnt();
        Debug.Log(stageSO.GetHeartStoneCnt());
        _heartStonesList[stageSO.GetHeartStoneCnt()-1].color = Color.white;
    }

    void IniteartStonesList()
    {
        //heart stone ui List에 담기
        _heartStonesList.Clear();
        for (int i = 0; i < stageSO.heartStoneTotalCnt; i++)
        {
            Image heartStone = heartStonesParent.transform.GetChild(i).GetComponent<Image>();
            _heartStonesList.Add(heartStone);
        }
    }

    public void ActiveSideCutSceneUI(TileSO tileSO)
    {
        _sideCutSceneImg.sprite = tileSO.sideCutSceneImg;
        switch (tileSO.tileID)
        {
            case TileID.Item:
                _curImgId = (int)tileSO.itemID;
                _triggerName = "ItemId";
                _sideCutSceneFrameTop.sprite = _sideCutSceneFrameTopList[1];
                _sideCutSceneFrameBottom.sprite = _sideCutSceneFrameBottomList[1];
                break;
            case TileID.Joker:
                break;
            case TileID.Trap:
                _curImgId = (int)tileSO.trapID;
                _triggerName = "TrapId";
                _sideCutSceneFrameTop.sprite = _sideCutSceneFrameTopList[0];
                _sideCutSceneFrameBottom.sprite = _sideCutSceneFrameBottomList[0];
                break;
            case TileID.HeartStone:
                break;
            case TileID.GreenZone:
                _curImgId = 1;
                _triggerName = "GreenZone";
                _sideCutSceneFrameTop.sprite = _sideCutSceneFrameTopList[4];
                _sideCutSceneFrameBottom.sprite = _sideCutSceneFrameBottomList[4];
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
}
