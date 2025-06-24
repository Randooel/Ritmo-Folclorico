using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class Conductor : MonoBehaviour
{
    // READ ME: This script is responsible for keeping track the music's rythm and
    // current position (in seconds)
    [Header("ATENTION: ONLY THE '_songBpm' FIELD MUST BE EDITED.")]
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

    //the number of beats in each loop
    public float beatsPerLoop;

    //the total number of loops completed since the looping clip first started
    public int completedLoops = 0;

    //The current position of the song within the loop in beats.
    public float loopPositionInBeats;

    //The current relative position of the song within the loop measured between 0 and 1.
    public float loopPositionInAnalog;

    //Conductor instance
    public static Conductor instance;

void Awake()
    {
        instance = this;
    }

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

        //calculate the loop position
        if (songPositionInBeats >= (completedLoops + 1) * beatsPerLoop)
        {
            completedLoops++;
        }

        loopPositionInBeats = songPositionInBeats - completedLoops * beatsPerLoop;

        loopPositionInAnalog = loopPositionInBeats / beatsPerLoop;
    }
}
