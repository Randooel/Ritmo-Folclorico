using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnBeat : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;

    void OnEnable()
    {
        RhythmEvent.onBeat += OnBeat;
    }
    private void OnDisable()
    {
        RhythmEvent.onBeat -= OnBeat;
    }

    void Start()
    {
        audioSource.clip = audioClip;
    }

    void OnBeat()
    {
        audioSource.Play();
    }
}
