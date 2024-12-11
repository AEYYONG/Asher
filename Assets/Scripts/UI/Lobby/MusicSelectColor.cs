using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MusicSelectColor : MonoBehaviour
{
    private TMP_Text text;
    private AudioSource AudioSource;
    void Start()
    {
        text = GetComponent<TMP_Text>();
        AudioSource = GetComponentInParent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (AudioManager.Instance._bgmSource.clip == AudioSource.clip &&
           AudioManager.Instance._bgmSource.isPlaying)
        {
            text.color = new Color32(50, 253, 0, 255);
        }
        else
        {
            text.color = new Color32(255, 255, 255, 255);
        }

    }
}
