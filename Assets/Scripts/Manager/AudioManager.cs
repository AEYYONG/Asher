using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("BGM")]
    public AudioClip bgmClip;
    [Range(0f, 1f)] public float bgmVolume = 1f;
    public AudioSource bgmPlayer;

    void Awake()
    {
        instance = this;
        Init();
    }

    void Init()
    {
        // 배경음 초기화
        GameObject bgmObject = new GameObject("BGMObject");
        bgmObject.transform.parent = transform;

        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = true;
        bgmPlayer.loop = true;
        bgmPlayer.clip = bgmClip;

        bgmPlayer.volume = Mathf.Clamp(bgmVolume, 0f, 1f); // 초기 볼륨 동기화
        PlayBgm(bgmClip);

        bgmPlayer.volume = bgmVolume;
        Debug.Log("초기 볼륨(AudioSource),init: " + bgmPlayer.volume);

    }


    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    // AudioSource 볼륨 변경
    public void SetBgmVolume(float volume)
    {
        bgmVolume = Mathf.Clamp(volume, 0f, 1f);
        bgmPlayer.volume = bgmVolume;
        Debug.Log("볼륨 변경: " + bgmVolume);
        Debug.Log("AudioSource 볼륨: " + bgmPlayer.volume);


    }
}
