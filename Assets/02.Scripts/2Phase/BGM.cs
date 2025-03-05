using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    [SerializeField] private AudioSource bgm1;
    [SerializeField] private AudioSource bgm2;
    void Start()
    {
        if (bgm1 != null && bgm2 != null)
        {
            bgm1.Play();
            Invoke("PlayBGM2", bgm1.clip.length);
        }
    }

    void PlayBGM2()
    {
        bgm2.loop = true;
        bgm2.Play();
    }
}
