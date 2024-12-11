using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    private AudioSource AudioSource;
    void Start()
    {
        AudioSource = GetComponentInParent<AudioSource>();
    }


    private void Update()
    {
        if (AudioManager.Instance._bgmSource.clip == AudioSource.clip &&
           AudioManager.Instance._bgmSource.isPlaying)
        {
            Transform child = transform.GetChild(0);
            child.gameObject.SetActive(true);
        }
        else
        {
            Transform child = transform.GetChild(0);
            child.gameObject.SetActive(false);
        }
    }
}
