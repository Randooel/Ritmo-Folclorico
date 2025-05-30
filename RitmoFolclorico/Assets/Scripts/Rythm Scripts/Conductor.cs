using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    // READ ME: This script is responsible for keeping track the music's rythm and
    // current position (in seconds)
    [Header("ATENTION: ONLY THE 'songPosition' FIELD MUST BE EDITED.")]
    [Header("All other fields are filled automatically")]

    [Header("Song Info")]
    [SerializeField] private float _songBpm;
    [SerializeField] private float _secPerBeat;

    [Header("Song Position")]
    // Current song position in seconds
    public float songPosition;
    // Current song position in beats
    public float songPositionInBeats;
    // How many seconds have passed since song started
    public float dspSongTime;

    [Header("Audio Source Reference")]
    public AudioSource musicSource;

    public float SecPerBeat 
    {
        get { return _secPerBeat; }
        private set { _secPerBeat = value;  }  
    }

    void Start()
    {
        // If musicSource wasn't serialized
        if(musicSource == null)
        {
            // Finds the game object "Song Source" that have it
            musicSource = GameObject.Find("Song Manager").GetComponent<AudioSource>();
        }

        // Calculate the number of seconds in each beat
        SecPerBeat = 60f / _songBpm;

        // Record the time when music started
        dspSongTime = (float)AudioSettings.dspTime;

        // Plays music
        musicSource.Play();
    }

    
    void Update()
    {
        // Determine how many seconds passed since song started playing
        songPosition = (float)(AudioSettings.dspTime - dspSongTime);

        // Determine how many beats passed since song started playing
        songPositionInBeats = songPosition / SecPerBeat;
    }
}
