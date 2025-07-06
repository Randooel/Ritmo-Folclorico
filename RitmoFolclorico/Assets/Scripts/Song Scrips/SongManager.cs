using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    Conductor conductor;

    [Header("Songs Info")]
    [SerializeField] private int _currentSongIndex;
    [SerializeField] private AudioClip[] audioClip;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] double goalTime;
    [SerializeField] double musicDuration;

    void Start()
    {
        if (conductor == null)
        {
            conductor = FindObjectOfType<Conductor>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        PlayCurrentMusic();
    }

    void Update()
    {
        // Reinicia a música assim que o tempo programado chega
        if (AudioSettings.dspTime >= goalTime)
        {
            ScheduleNextLoop();
        }

        // Troca de música (apenas para testes)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _currentSongIndex = 0;
            PlayCurrentMusic();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _currentSongIndex = 1;
            PlayCurrentMusic();
        }
    }

    void PlayCurrentMusic()
    {
        audioSource.clip = audioClip[_currentSongIndex];

        // Calcula a duração da música em segundos
        musicDuration = (double)audioSource.clip.samples / audioSource.clip.frequency;

        // Agenda o início imediato (ou com offset se quiser)
        goalTime = AudioSettings.dspTime;
        audioSource.PlayScheduled(goalTime);

        // Define quando a próxima repetição será agendada
        goalTime += musicDuration;
    }

    void ScheduleNextLoop()
    {
        // Agenda a próxima execução
        audioSource.PlayScheduled(goalTime);
        goalTime += musicDuration;
    }
}
