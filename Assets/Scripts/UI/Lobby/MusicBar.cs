using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class MusicBar : MonoBehaviour
{
    private Slider progressBar; // Slider UI
    public TMP_Text currentTimeText; // 현재 시간 텍스트
    public TMP_Text totalTimeText; // 총 시간 텍스트

    private AudioSource audioSource; // 재생할 AudioSource

    void Start()
    {
        progressBar = GetComponent<Slider>();
        audioSource = GetComponentInParent<AudioSource>();

        if (audioSource.clip != null)
        {
            // 초기화
            totalTimeText.text = FormatTime(audioSource.clip.length);
        }
    }

    void Update()
    {
        if (audioSource.clip != null && audioSource.isPlaying)
        {
            // 슬라이더 업데이트
            progressBar.value = audioSource.time / audioSource.clip.length;

            // 현재 시간 업데이트
            currentTimeText.text = FormatTime(audioSource.time);
        }

        if (!audioSource.isPlaying && audioSource.time >= audioSource.clip.length)
        {
            progressBar.value = 0;
            currentTimeText.text = "00:00";
        }
    }

    // 시간을 MM:SS 형식으로 변환
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

