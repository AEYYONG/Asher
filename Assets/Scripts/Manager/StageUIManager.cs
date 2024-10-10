using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour
{
    public StageInfoSO stageSO;
    [SerializeField] private GameObject heartStonesParent;
    [SerializeField] private List<Image> _heartStonesList = new List<Image>();
    [SerializeField] private GameObject _sideCutSceneUI;
    [SerializeField] private Animator _sideCutSceneImgAnimator;
    
    void Awake()
    {
        IniteartStonesList();
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

    public void ActiveSideCutSceneUI()
    {
        _sideCutSceneUI.SetActive(true);
    }

    public void SetSideCutSceneUIImg(TrapID id)
    {
        int _id = (int)id;
        Debug.Log("trap id : "+ _id);
        _sideCutSceneImgAnimator.SetInteger("TrapId",_id);
    }
}
