using UnityEngine;
using UnityEngine.UI;

public class MusicButton : MonoBehaviour
{
    private AudioSource parentAudioSource; // 부모 컴포넌트에 있는 AudioSource
    public Sprite activeSprite; // 재생중
    public Sprite defaultSprite; // 기본
    private Image buttonImage;
    void Start()
    {
        // 부모에서 AudioSource 가져오기
        parentAudioSource = GetComponentInParent<AudioSource>();
        buttonImage = GetComponent<Image>();
        UpdateButtonImage();
    }

    private void UpdateButtonImage()
    {
        // 현재 재생 중인 음악 이름과 부모의 음악 이름 비교
        if (AudioManager.instance.bgmPlayer.clip.name == parentAudioSource.clip.name)
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
        AudioManager.instance.bgmPlayer.clip = parentAudioSource.clip;
        AudioManager.instance.bgmPlayer.Play();
        Debug.Log(parentAudioSource.clip.name);
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
        }
    }

}
