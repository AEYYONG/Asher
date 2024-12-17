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
    public float _time;
    
    //UI 연결
    [SerializeField] private Image timeBar;
    [SerializeField] private Image timeBarBorder;

    void Start()
    {
        _time = timer;
    }

    public IEnumerator TimerStart(float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            timeBar.fillAmount = time / timer;
            timeBarBorder.fillAmount = timeBar.fillAmount;

            if (time < 11)
            {
                timeBar.color = Color.red;
            }
            yield return null;
        }
        time = 0;
        Debug.Log("Time Over");
        StartCoroutine(StageManager.Instance.GameOver());
    }
}
