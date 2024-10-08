using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StageInfoSO",fileName = "StageInfoSO")]
public class StageInfoSO : ScriptableObject
{
    public int heartStoneTotalCnt;
    public float limitedTime;
    [SerializeField] private int heartStoneCurCnt;
    
    public int GetHeartStoneCnt() { return heartStoneCurCnt; }
    public void IncreaseHeartStoneCnt() { heartStoneCurCnt++; }
    

    private void OnEnable()
    {
        heartStoneCurCnt = 0;
    }
}
