using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    //타이머 값
    [SerializeField] private float timer;
    
    private int _min; //분
    private int _sec; //초
    
    //UI 연결
    [SerializeField] private TextMeshProUGUI timeText;

    void Start()
    {
        StartCoroutine(TimerStart(timer));
    }

    IEnumerator TimerStart(float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            //분:초 형식으로 나타내기
            if (time / 60 != 0)
            {
                _min = (int)(time / 60);
                _sec = (int)(time % 60);
            }
            else
            {
                _min = 0;
                _sec = (int)(time % 60);
            }
            
            //time.Tostring("D2")는 두자리 정수
            timeText.text = "Time : " + _min.ToString("D2") + ":" + _sec.ToString("D2");
            yield return null;
        }
        time = 0;
        timeText.text = "Time : 00:00";
        Debug.Log("Stage Over");
    }
}
