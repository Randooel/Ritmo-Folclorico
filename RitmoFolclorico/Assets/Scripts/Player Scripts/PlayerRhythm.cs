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
    private DOAnimations doAnimations;
    private string nextState = "Idle";
    
    //[SerializeField] Animator animator;
    
    public delegate void OnMouseClickDelegate(string hitType);
    public event OnMouseClickDelegate OnMouseClick;

    public delegate void OnActionCompleteDelegate(int hitID);
    public static event OnActionCompleteDelegate OnActionComplete;

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
    public int currentAction;

    [Header("Followers")]
    [SerializeField] private List<GameObject> followers = new List<GameObject>();
    [SerializeField] GameObject[] followerSlot;
    int nextSlot;

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


    private void Awake()
    {
        doAnimations = GetComponentInChildren<DOAnimations>();
        if(doAnimations == null)
        {
            Debug.LogError("doAnimations reference not found");
        }
    }

    private void OnEnable()
    {
        WhistlerBrain.OnEnemyRescued += AddFollower;
        doAnimations.OnAnimStateChanged += ChangeAnimState;
    }

    private void OnDisable()
    {
        WhistlerBrain.OnEnemyRescued -= AddFollower;
        doAnimations.OnAnimStateChanged -= ChangeAnimState;
    }

    void Start()
    {
        conductor = FindObjectOfType<Conductor>();
        playerCommands = FindObjectOfType<PlayerCommands>();
        whistlerBrain = FindObjectOfType<WhistlerBrain>();
        doAnimations = GetComponent<DOAnimations>();

        //animator = GetComponent<Animator>();

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
                        OnMouseClick?.Invoke("Perfect");
                        //Debug.Log("PERFECT!");
                    }
                    else
                    {
                        hitCorrect = true;
                        OnMouseClick?.Invoke("Correct");
                        //Debug.Log("Correct!");
                    }
                }
                else
                {
                    hitWrong = true;
                    OnMouseClick?.Invoke("Wrong");

                    // Clear the current command list if the player gets the rhythm wrong
                    OnWrongTime();
                    //Debug.Log("Wrong!");
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

            nextState = "Ono1";
            ChangeAnimState(DOAnimations.State.Ono1);

            for (int i = 0; i < followers.Count; i++)
            {
                DOAnimations anim = followers[i].GetComponent<DOAnimations>();
                //anim.SetTrigger("isDancingOno1");
                anim.CurrentState = DOAnimations.State.Ono1;
                anim.CheckCurrentState(DOAnimations.State.Ono1);
            }

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

            //animator.SetTrigger("isOno2");
            nextState = "Ono2";
            ChangeAnimState(DOAnimations.State.Ono2);

            for (int i = 0; i < followers.Count; i++)
            {
                DOAnimations anim = followers[i].GetComponent<DOAnimations>();
                //anim.SetTrigger("isDancingOno2");
                anim.CurrentState = DOAnimations.State.Ono2;
            }

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
                //Debug.LogWarning("OnWrongTime");

                hitWrong = true;
                OnMouseClick?.Invoke("Wrong");

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
        yield return new WaitForSeconds(conductor.SecPerBeat);

        playerCommands.commandable = false;

        playerCommands.mouseButtonPressed.Clear();
        DeactivateCommandPrint();

        int actionToCall = -1;

        if(whistlerBrain.CurrentCommandSequence == whistlerBrain.basicCommands.commandSets[0].commandSequence)
        {
            tempoAnim = conductor.SecPerBeat * 2;
            //animator.SetTrigger("isWalking");
            actionToCall = 0;

            for(int i = 0; i < followers.Count; i++)
            {
                Animator anim = followers[i].GetComponent<Animator>();
                anim.SetTrigger("isWalking");
            }
        }

        if (whistlerBrain.CurrentCommandSequence == whistlerBrain.basicCommands.commandSets[1].commandSequence ||
            whistlerBrain.CurrentCommandSequence == whistlerBrain.basicCommands.commandSets[2].commandSequence)
        {
            actionToCall = 1;
        }

        if (whistlerBrain.CurrentCommandSequence == whistlerBrain.boitataCommands.commandSets[2].commandSequence ||
            whistlerBrain.CurrentCommandSequence == whistlerBrain.boitataCommands.commandSets[3].commandSequence ||
            whistlerBrain.CurrentCommandSequence == whistlerBrain.boitataCommands.commandSets[4].commandSequence)
        {
            actionToCall = 2;
        }

        OnActionComplete?.Invoke(actionToCall);
    }

    public void AddFollower(GameObject newFollower)
    {
        newFollower.GetComponent<BoxCollider2D>().enabled = false;
        // newFollower.transform.position += transform.right * 10;

        newFollower.transform.position = followerSlot[nextSlot].transform.position;

        nextSlot++;

        followers.Add(newFollower);
    }

    // Handle
    void HandleWalk()
    {
        if (tempoAnim > 0)
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime;

            for (int i = 0; i < followers.Count; i++)
            {
                followers[i].transform.position += transform.right * moveSpeed * Time.deltaTime;
            }

            tempoAnim -= Time.deltaTime;
        }
        else
        {
            playerCommands.commandable = true;
        }
    }

    // Wrong Time
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
        for (int i = 0; i < followers.Count; i++)
        {
            Animator anim = followers[i].GetComponent<Animator>();
            anim.SetTrigger("isDisapproving");
        }
    }

    void ChangeAnimState(DOAnimations.State state)
    {
        switch (nextState)
        {
            case "Idle":
                doAnimations.CurrentState = DOAnimations.State.Idle;
                break;
            case "Walk":
                doAnimations.CurrentState = DOAnimations.State.Walk;
                break;
            case "Ono1":
                doAnimations.CurrentState = DOAnimations.State.Ono1;
                break;
            case "Ono2":
                doAnimations.CurrentState = DOAnimations.State.Ono2;
                break;
            case "WhistleOno1":
                doAnimations.CurrentState = DOAnimations.State.WhistleOno1;
                break;
            case "WhistleOno2":
                doAnimations.CurrentState = DOAnimations.State.WhistleOno2;
                break;
            case "Disapprove":
                doAnimations.CurrentState = DOAnimations.State.Disapprove;
                break;
            default:
                Debug.Log("On default case");
                break;
        }
    }

    // Visual
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