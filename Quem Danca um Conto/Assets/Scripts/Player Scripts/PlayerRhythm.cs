using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerRhythm : MonoBehaviour
{
    // READ ME: THIS SCRIPT CHECKS HOW WELL PLAYER'S INPUT MATCH THE SONG'S BEAT
    private Conductor conductor;

    public float beatWhenButtonPressed;    

    public delegate void OnHitDelegate(string hitType);
    public event OnHitDelegate OnHit;

    [Header("Player Commands")]
    public List<int> mouseButtonPressed = new List<int>();

    [Header("Timing Interval for Correct Input")]
    [Header("Ex: Defines as *0*.7 / *1*.3")]
    [Range(0,2)] public float minCorrectTime = 0.7f;
    [Range(0,2)] public float maxCorrectTime = 1.3f;
    

    [Header("Timing Interval for Perfect Input")]
    [Header("Ex: Define MIN as *0*.X / MAX: *1*.Y")]
    [Range(0,2)] public float minPerfectTime = 0.9f;
    [Range(0,2)] public float maxPerfectTime = 1.1f;

    [Header("Hit Booleans")]
    public bool hitWrong;
    public bool hitCorrect;
    public bool hitPerfect;
    [Range(0f,1f)] public float boolWaitTime;

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
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if(mouseButtonPressed.Count <= 4)
            {
                CheckMouseButtonPressed();
            }
            if(mouseButtonPressed.Count > 4)
            {
                mouseButtonPressed.Clear();

                CheckMouseButtonPressed();
            }
            

            // Marks when the player pressed the button based on the Conductor's beat counting
            beatWhenButtonPressed = conductor.songPositionInBeats;

            // Gets the songPositionInBeats and turns it into a integer
            int beatTime = Mathf.FloorToInt(conductor.songPositionInBeats);

            if (beatWhenButtonPressed >= beatTime + minCorrectTime && beatWhenButtonPressed <= beatTime + maxCorrectTime)
            {
                if (beatWhenButtonPressed >= beatTime + minPerfectTime && beatWhenButtonPressed < beatTime + maxPerfectTime)
                {
                    hitPerfect = true;
                    OnHit?.Invoke("Perfect");
                    Debug.Log("PERFECT!");
                }
                else
                {
                    hitCorrect = true;
                    OnHit?.Invoke("Correct");
                    Debug.Log("Correct!");
                }
            }
            else
            {
                hitWrong = true;
                OnHit?.Invoke("Wrong");
                Debug.Log("Wrong!");
            }
        }
        StartCoroutine(ResetBools());
    }

    IEnumerator ResetBools()
    {
        yield return new WaitForSeconds(boolWaitTime);
        hitWrong = false;
        hitCorrect = false;
        hitPerfect = false;
    }

    private void CheckMouseButtonPressed()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseButtonPressed.Add(1);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            mouseButtonPressed.Add(2);
        }
    }
}