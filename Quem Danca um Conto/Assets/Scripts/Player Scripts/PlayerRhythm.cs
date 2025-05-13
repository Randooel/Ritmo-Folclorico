using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRhythm : MonoBehaviour
{
    // READ ME: THIS SCRIPT CHECKS HOW WELL PLAYER'S INPUT MATCH THE SONG'S BEAT
    private Conductor conductor;
    private PlayerCommands playerCommands;
    [SerializeField] Animator animator;

    [Header("Button Set")]
    public float beatWhenButtonPressed;

    [Header("Visual Feedbacks")]
    [SerializeField] Image[] onomatopeia;
    [SerializeField] Image[] commandPrint;
    [SerializeField] Sprite[] spritesPrint;

    public delegate void OnHitDelegate(string hitType);
    public event OnHitDelegate OnHit;

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
        playerCommands = FindObjectOfType<PlayerCommands>();

        animator = GetComponent<Animator>();

        DeactivateCommandPrint();

        onomatopeia[0].gameObject.SetActive(false);
        onomatopeia[1].gameObject.SetActive(false);
    }
    void Update()
    {
        BeatInput();
    }

    void BeatInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if(playerCommands.mouseButtonPressed.Count < 4)
            {
                CheckMouseButtonPressed();
            }
            else if(playerCommands.mouseButtonPressed.Count >= 4)
            {
                playerCommands.mouseButtonPressed.Clear();

                DeactivateCommandPrint();
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

                // Clear the current command list if the player gets the rhythm wrong
                OnWrongTime();
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
            int index = playerCommands.mouseButtonPressed.Count;

            playerCommands.mouseButtonPressed.Add(1);

            if (index < commandPrint.Length)
            {
                commandPrint[index].sprite = spritesPrint[0];
                commandPrint[index].gameObject.SetActive(true);
            }

            onomatopeia[0].gameObject.SetActive(true);
            StartCoroutine(ResetOnomatopeia());
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            int index = playerCommands.mouseButtonPressed.Count;

            playerCommands.mouseButtonPressed.Add(2);

            if (index < commandPrint.Length)
            {
                commandPrint[index].sprite = spritesPrint[1];
                commandPrint[index].gameObject.SetActive(true);
            }

            onomatopeia[1].gameObject.SetActive(true);
            StartCoroutine(ResetOnomatopeia());
            return;
        }
    }

    private void OnWrongTime()
    {
        playerCommands.mouseButtonPressed.Clear();

        DeactivateCommandPrint();
    }

    void DeactivateCommandPrint()
    {
        foreach (var img in commandPrint)
        {
            img.gameObject.SetActive(false);
        }
    }

    IEnumerator ResetOnomatopeia()
    {
        yield return new WaitForSeconds(boolWaitTime);

        onomatopeia[0].gameObject.SetActive(false);
        onomatopeia[1].gameObject.SetActive(false);
    }
}