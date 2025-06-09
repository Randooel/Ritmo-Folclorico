using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerRhythm : MonoBehaviour
{
    // READ ME: THIS SCRIPT CHECKS HOW WELL PLAYER'S INPUT MATCH THE SONG'S BEAT
    private Conductor conductor;
    private PlayerCommands playerCommands;
    private WhistlerBrain whistlerBrain;
    [SerializeField] Animator animator;

    public delegate void OnHitDelegate(string hitType);
    public event OnHitDelegate OnHit;

    public bool onDebug;

    [Header("Timing Interval for Correct Input")]
    [Header("Ex: Defines as *0*.7 / *1*.3")]
    [Range(0, 2)] public float minCorrectTime = 0.7f;
    [Range(0, 2)] public float maxCorrectTime = 1.3f;


    [Header("Timing Interval for Perfect Input")]
    [Header("Ex: Define MIN as *0*.X / MAX: *1*.Y")]
    [Range(0, 2)] public float minPerfectTime = 0.9f;
    [Range(0, 2)] public float maxPerfectTime = 1.1f;

    [Header("Button Set")]
    public float beatWhenButtonPressed;
    [SerializeField] private bool _wasWrongTime = false;

    [Header("Parameters")]
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float tempoAnim;

    [Header("Followers")]
    [SerializeField] private List<GameObject> followers = new List<GameObject>(); 

    [Header("Hit Booleans")]
    public bool hitWrong;
    public bool hitCorrect;
    public bool hitPerfect;
    [Range(0f,1f)] public float boolWaitTime;

    [Header("Visual Feedbacks")]
    [SerializeField] Image[] onomatopeia;
    [SerializeField] Image[] commandPrint;
    [SerializeField] Sprite[] spritesPrint;
    [SerializeField] bool _isCommandPrintCleared;

    void Start()
    {
        conductor = FindObjectOfType<Conductor>();
        playerCommands = FindObjectOfType<PlayerCommands>();
        whistlerBrain = FindObjectOfType<WhistlerBrain>();

        animator = GetComponent<Animator>();

        DeactivateCommandPrint();

        _isCommandPrintCleared = true;

        onomatopeia[0].gameObject.SetActive(false);
        onomatopeia[1].gameObject.SetActive(false);
    }

    void Update()
    {
        BeatInput();
        
        // PARAMETER ADJUSTS DURING DEBUG
        if(onDebug)
        {
            minCorrectTime = 0;
            maxCorrectTime = 2;
        }
    }

    private void FixedUpdate()
    {
        HandleWalk();
    }

    void BeatInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if(playerCommands.commandable)
            {
                if(_wasWrongTime)
                {
                    _wasWrongTime = false;
                    return;
                }
                if(playerCommands.mouseButtonPressed.Count < 4)
                {
                    CheckMouseButtonPressed();
                    _isCommandPrintCleared = false;
                }
                
                else
                {
                    // OnCompletedComand();

                    if (!_isCommandPrintCleared)
                    {
                        playerCommands.mouseButtonPressed.Clear();
                        _isCommandPrintCleared = true;
                    }

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

            if (playerCommands.mouseButtonPressed.Count == 4)
            {
                OnCompletedCommand();
            }
        }
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

    private void OnCompletedCommand()
    {
        // Checks if the inserted command sequence matches the whistler's command sequence
        for (int i = 0; i < 4; i++)
        {
            if (playerCommands.mouseButtonPressed[i] != whistlerBrain.CurrentCommandSequence[i])
            {
                Debug.LogWarning("OnWrongTime");

                hitWrong = true;
                OnHit?.Invoke("Wrong");

                StartCoroutine(OnWaitToWrong());
                return;
            }
            if(i == 3)
            {
                StartCoroutine(OnExecuteAction());
            }
        }
    }

    IEnumerator OnExecuteAction()
    {
        Debug.Log("OnExecuteAction");

        playerCommands.commandable = false;
        Debug.LogWarning("OnExecuteAction");

        yield return new WaitForSeconds(conductor.SecPerBeat);

        playerCommands.mouseButtonPressed.Clear();
        DeactivateCommandPrint();

        if(whistlerBrain.CurrentCommandSequence == whistlerBrain.basicCommands.commandSets[0].commandSequence)
        {
            tempoAnim = conductor.SecPerBeat * 2;
        }

        playerCommands.commandable = true;
    }

    IEnumerator OnWaitToWrong()
    {
        yield return new WaitForSeconds(conductor.SecPerBeat);
        OnWrongTime();
    }

    private void OnWrongTime()
    {
        if(!_isCommandPrintCleared)
        {
            playerCommands.mouseButtonPressed.Clear();
            DeactivateCommandPrint();

            _isCommandPrintCleared = true;
            //_wasWrongTime = true;
        }
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

    // Handle
    void HandleWalk()
    {
        if(tempoAnim > 0)
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime;
            for(int i = 0; i < followers.Count; i++)
            {
                followers[i].transform.position += transform.right * moveSpeed * Time.deltaTime;
            }
            tempoAnim -= Time.deltaTime;
        }
    }
}