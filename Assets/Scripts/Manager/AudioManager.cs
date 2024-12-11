using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    // 딕셔너리로 오디오 관리
    public Dictionary<string, AudioData> bgmDictionary = new Dictionary<string, AudioData>();
    public Dictionary<string, AudioData> sfxDictionary = new Dictionary<string, AudioData>();

    [SerializeField]
    private List<AudioData> bgmDataList; //bgm 데이터 리스트
    [SerializeField]
    private List<AudioData> sfxDataList; //sfx 데이터 리스트
    
    public AudioSource _bgmSource; // BGM을 재생하는 AudioSource
    private List<AudioSource> _sfxSources = new List<AudioSource>(); // SFX를 재생하는 AudioSource 리스트
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [SerializeField] private int _maxSFXPoolSize = 10;

    void Start()
    {
        // bgm 데이터 초기화
        foreach (var bgmData in bgmDataList)
        {
            if (bgmData != null && bgmData.audioClip != null)
            {
                bgmDictionary[bgmData.audioName] = bgmData;
            }
        }
        
        // sfx 데이터 초기화
        foreach (var sfxData in sfxDataList)
        {
            if (sfxData != null && sfxData.audioClip != null)
            {
                sfxDictionary[sfxData.audioName] = sfxData;
            }
        }
        
        // BGM 소스 생성
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.loop = true;
        PlayBGM(bgmDictionary["Asher Main Theme"]);
    }
    public void PlayBGM(AudioData audioData)
    {
        if (audioData == null || audioData.audioClip == null)
        {
            Debug.LogWarning("AudioData가 유효하지 않습니다.");
            return;
        }

        // 현재 재생 중인 BGM과 같다면 다시 재생할 필요 없음
        if (_bgmSource.clip == audioData.audioClip && _bgmSource.isPlaying)
        {
            Debug.Log("현재 재생 중인 BGM과 동일합니다.");
            return;
        }

        // 기존 BGM 중지 후 새 BGM 설정
        _bgmSource.Stop();
        _bgmSource.clip = audioData.audioClip;
        _bgmSource.volume = Mathf.Clamp(bgmVolume, 0f, 1f); //option에서 초기 볼륨 동기화
        _bgmSource.loop = audioData.loop;
        _bgmSource.playOnAwake = audioData.playOnAwake;

        // 자동 재생
        if (audioData.playOnAwake)
        {
            _bgmSource.Play();
        }
    }

    public IEnumerator PlaySequentialBGM(AudioData audio1, AudioData audio2)
    {
        Debug.Log("audio1 시작");
        PlayBGM(audio1);
        yield return new WaitForSeconds(audio1.audioClip.length);
        Debug.Log("audio2 시작");
        PlayBGM(audio2);
    }

    public void PlaySFX(AudioData audioData)
    {
        if (audioData == null || audioData.audioClip == null)
        {
            Debug.LogWarning("AudioData가 유효하지 않습니다.");
            return;
        }

        // 재사용 가능한 AudioSource 가져오기
        AudioSource source = GetAvailableSFXSource();

        source.clip = audioData.audioClip;
        source.volume = audioData.volume;
        source.loop = audioData.loop;
        source.Play();

        // **AudioClip의 길이만큼 대기 후 오디오 소스 중지 및 반환**
        float clipLength = audioData.audioClip.length; // 클립의 길이 가져오기
        StartCoroutine(StopAndReleaseSourceAfterDelay(source, clipLength));
    }
    
    private AudioSource GetAvailableSFXSource()
    {
        // 사용 가능한 오디오 소스 찾기
        foreach (AudioSource source in _sfxSources)
        {
            if (!source.isPlaying)
            {
                return source; // 재사용 가능한 소스를 반환
            }
        }

        // 새 오디오 소스를 생성 (최대 개수 제한)
        if (_sfxSources.Count < _maxSFXPoolSize)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            _sfxSources.Add(newSource);
            return newSource;
        }

        // 풀이 꽉 찼을 때 가장 오래된 소스를 재사용
        return _sfxSources[0];
    }
    
    private System.Collections.IEnumerator StopAndReleaseSourceAfterDelay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (source != null && source.isPlaying)
        {
            source.Stop();
        }
    }
    
    public void StopAllSFX()
    {
        foreach (AudioSource source in _sfxSources)
        {
            source.Stop();
        }
    }
    public void StopBGM()
    {
        if (_bgmSource.isPlaying)
        {
            _bgmSource.Stop();
        }
    }
    
    // AudioSource 볼륨 변경
    public void SetBgmVolume(float volume)
    {
        bgmVolume = Mathf.Clamp(volume, 0f, 1f);
        _bgmSource.volume = bgmVolume;
        Debug.Log("볼륨 변경: " + bgmVolume);
        Debug.Log("AudioSource 볼륨: " + _bgmSource.volume);
    }
}
