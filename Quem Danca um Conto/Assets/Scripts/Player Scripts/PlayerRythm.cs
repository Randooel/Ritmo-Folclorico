using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerRythm : MonoBehaviour
{
    // READ ME: THIS SCRIPT CHECKS HOW WELL PLAYER'S INPUT MATCH THE SONG'S BEAT
    private Conductor conductor;

    public float beatWhenButtonPressed;

    [Header("Timing Interval for Correct Input")]
    [Header("Ex: Defines as *0*.7 / *1*.3")]
    [Range(0,2)] public float minCorrectTime = 0.7f;
    [Range(0,2)] public float maxCorrectTime = 1.3f;

    [Header("Timing Interval for Perfect Input")]
    [Header("Ex: Define as *0*.9 / *1*.1")]
    [Range(0,2)] public float minPerfectTime = 0.9f;
    [Range(0,2)] public float maxPerfectTime = 1.1f;

    void Start()
    {
        conductor = FindObjectOfType<Conductor>();
    }
    void Update()
    {
        BeatInput();
    }

    void BeatInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Marks when the player pressed the button based on the Conductor's beat counting
            beatWhenButtonPressed = conductor.songPositionInBeats;

            // Gets the songPositionInBeats and turns it into a integer
            int beatTime = Mathf.FloorToInt(conductor.songPositionInBeats);

            if (beatWhenButtonPressed >= beatTime + minCorrectTime && beatWhenButtonPressed <= beatTime + maxCorrectTime)
            {
                if (beatWhenButtonPressed >= beatTime + minPerfectTime && beatWhenButtonPressed < beatTime + maxPerfectTime)
                {
                    Debug.Log("PRESSED AT THE PERFECT TIME!!!!");
                }
                else
                {
                    Debug.Log("Pressed at the CORRECT Time!");
                }
            }
            else
            {
                Debug.Log("Missed!");
            }
        }
    }
}
