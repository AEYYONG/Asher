using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Audio/Create Audio Data")]
public class AudioData : ScriptableObject
{
    public string audioName; //audio 이름
    public AudioClip audioClip;  //audio clip
    public bool loop;        //루프 여부
    public bool playOnAwake;
    public float volume; //볼륨
}
