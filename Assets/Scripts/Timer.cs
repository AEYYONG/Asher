using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    //UI 연결

    public IEnumerator TimerStart(float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            //time.Tostring("F1")는 소숫점 첫째자리까지만 표기
            //timeText.text = "Time : " + time.ToString("F1");
            yield return null;
        }
        time = 0;
        //timeText.text = "Time : " + time.ToString("F1");
    }
}
