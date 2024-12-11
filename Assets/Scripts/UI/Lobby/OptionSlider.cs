using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionSlider : MonoBehaviour
{
    private Slider volumeSlider; // 슬라이더 UI

    void Start()
    {
        volumeSlider = GetComponent<Slider>();

        // 슬라이더 값 변경 이벤트를 임시로 해제
        volumeSlider.onValueChanged.RemoveAllListeners(); // 이벤트 제거
        volumeSlider.value = AudioManager.Instance.bgmVolume;      // 초기값 설정
        Debug.Log("초기 슬라이더 값: " + volumeSlider.value);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged); // 이벤트 재등록
    }

    public void OnVolumeChanged(float value)
    {
        Debug.Log("슬라이더 값 변경: " + value);
        AudioManager.Instance.SetBgmVolume(value);
        Debug.Log("AudioManager 볼륨: " + AudioManager.Instance.bgmVolume);
    }
}