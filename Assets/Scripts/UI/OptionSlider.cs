using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionSlider : MonoBehaviour
{
    private Slider volumeSlider; // 슬라이더 UI
    private AudioManager audioManager;

    void Start()
    {
        volumeSlider = GetComponent<Slider>();
        // AudioManager 가져오기
        audioManager = AudioManager.instance;

        // 슬라이더 값 변경 이벤트를 임시로 해제
        volumeSlider.onValueChanged.RemoveAllListeners(); // 이벤트 제거
        volumeSlider.value = audioManager.bgmVolume;      // 초기값 설정
        Debug.Log("초기 슬라이더 값: " + volumeSlider.value);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged); // 이벤트 재등록
    }

    public void OnVolumeChanged(float value)
    {
        Debug.Log("슬라이더 값 변경: " + value);
        if (audioManager != null)
        {
            audioManager.SetBgmVolume(value);
            Debug.Log("AudioManager 볼륨: " + audioManager.bgmVolume);
        }
    }
}