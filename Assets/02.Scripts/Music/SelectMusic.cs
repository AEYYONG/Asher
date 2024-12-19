using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectMusic : MonoBehaviour
{
    public List<AudioClip> musicClips; // 음악 리스트
    public GameObject musicBoxPrefab; // 프리팹 (Music Box)
    public Transform musicListParent; // 부모 객체 (음악 리스트가 배치될 Transform)
    public AudioSource audioSource; // 음악을 재생할 AudioSource

    void Start()
    {
        // 음악 리스트 동적 생성
        foreach (var clip in musicClips)
        {
            CreateMusicBox(clip);
        }
    }

    void CreateMusicBox(AudioClip clip)
    {
        // Music Box 생성
        GameObject musicBox = Instantiate(musicBoxPrefab, musicListParent);

        // 텍스트 업데이트
        TMP_Text[] textElements = musicBox.GetComponentsInChildren<TMP_Text>();
        if (textElements.Length > 0)
        {
            textElements[0].text = clip.name; // 첫 번째 Text (음악 이름)
        }

        // 버튼 클릭 이벤트 추가
        Button playButton = musicBox.GetComponentInChildren<Button>();
        playButton.onClick.AddListener(() => PlayMusic(clip));
    }

    public void PlayMusic(AudioClip clip)
    {
        // 선택된 음악 재생
        audioSource.clip = clip;
        audioSource.Play();
        Debug.Log("Playing: " + clip.name);
    }
}
