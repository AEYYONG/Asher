using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MusicButton : MonoBehaviour
{
    private AudioSource parentAudioSource; // 부모 컴포넌트에 있는 AudioSource
    public Sprite activeSprite; // 재생중
    public Sprite defaultSprite; // 기본
    private Image buttonImage;

    [Header("UI Elements")]
    public Slider progressBar; // 음악 재생 상태를 나타내는 슬라이더
    public TMP_Text currentTimeText; // 현재 재생 시간
    public TMP_Text totalTimeText; // 총 재생 시간
    void Start()
    {
        // 부모에서 AudioSource 가져오기
        parentAudioSource = GetComponentInParent<AudioSource>();
        buttonImage = GetComponent<Image>();

        totalTimeText.text = FormatTime(parentAudioSource.clip.length);

        UpdateButtonImage();
    }
    private void Update()
    {
        if (AudioManager.Instance._bgmSource.clip == parentAudioSource.clip &&
           AudioManager.Instance._bgmSource.isPlaying)
        {
            buttonImage.sprite = activeSprite;
            progressBar.value = AudioManager.Instance._bgmSource.time / AudioManager.Instance._bgmSource.clip.length;

            // 현재 시간 업데이트
            currentTimeText.text = FormatTime(AudioManager.Instance._bgmSource.time);
        }
        else
        {
            buttonImage.sprite = defaultSprite;
        }
    }
    
    private void UpdateButtonImage()
    {
        // 현재 재생 중인 음악 이름과 부모의 음악 이름 비교
        if (AudioManager.Instance._bgmSource.clip.name == parentAudioSource.clip.name)
        {
            buttonImage.sprite = activeSprite;
        }
        else
        {
            buttonImage.sprite = defaultSprite;
        }

    }

    public void OnMusicButtonClick()
    {
        ResetAllButtons();

        // AudioManager를 통해 음악 변경
        AudioManager.Instance.PlayBGM(AudioManager.Instance.bgmDictionary[parentAudioSource.clip.name]);
        Debug.Log(parentAudioSource.clip.name);
        GameManager.Instance.lobbyBgmName = parentAudioSource.clip.name;
        
        
        SetToActive();
    }


    // 모든 버튼 상태 초기화
    private void ResetAllButtons()
    {
        MusicButton[] allButtons = FindObjectsOfType<MusicButton>();
        foreach (MusicButton button in allButtons)
        {
            button.SetToDefault();
        }
    }

    // 활성화 상태로 변경
    public void SetToActive()
    {
        buttonImage.sprite = activeSprite;
    }

    // 기본 상태로 변경
    public void SetToDefault()
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = defaultSprite;
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
