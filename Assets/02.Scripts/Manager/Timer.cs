using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    // 타이머 값
    [SerializeField] private float timer;
    public float _time;
    
    // UI 연결
    [SerializeField] private Image timeBar;
    [SerializeField] private Image timeBarBorder;

    private Coroutine timerCoroutine; // 타이머 Coroutine 참조

    void Start()
    {
        _time = timer;
    }

    // 타이머 시작
    public IEnumerator TimerStart(float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            _time = time;
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

    // 타이머 재개 함수
    public void RestartTimer()
    {
        // 이미 실행 중인 타이머가 있다면 중지
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(TimerStart(_time));
    }

    // 타이머 일시 중지
    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine); // 실행 중인 Coroutine 중지
            timerCoroutine = null; // 참조 해제
        }
    }
}
