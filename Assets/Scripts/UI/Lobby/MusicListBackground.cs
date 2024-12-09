using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MusicListBackground : MonoBehaviour
{
    private AudioSource AudioSource;
    public Sprite activeSprite; // 재생중
    public Sprite defaultSprite; // 기본
    private Image backgroundImage;

    void Start()
    {

        AudioSource = GetComponent<AudioSource>();
        backgroundImage = GetComponent<Image>();

    }


    private void Update()
    {
        if (AudioManager.instance.bgmPlayer.clip == AudioSource.clip &&
           AudioManager.instance.bgmPlayer.isPlaying)
        {
            backgroundImage.sprite = activeSprite;
           
        }
        else
        {
            backgroundImage.sprite = defaultSprite;
            
        }
    }

}
