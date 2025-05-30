using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    // READ ME: This script is used to store all songs and determine which one should be played

    [Header("Songs Info")]
    // This value will determine which song will be played
    [SerializeField] private int _currentSongIndex;
    [SerializeField] private AudioClip[] music;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;
    

    void Start()
    {
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        ChangeCurrentMusic();
    }

    void ChangeCurrentMusic()
    {
        if(audioSource.clip != music[_currentSongIndex])
        {
            audioSource.clip = music[_currentSongIndex];
            audioSource.Play();
        }


        // DELETE AFTER TESTS:
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _currentSongIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _currentSongIndex = 1;
        }
    }
}