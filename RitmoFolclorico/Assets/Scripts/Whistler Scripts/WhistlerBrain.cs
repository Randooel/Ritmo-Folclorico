using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System;

public class WhistlerBrain : MonoBehaviour, IDanceable
{
    private PlayerCommands playerCommands;
    private PlayerRhythm playerRhythm;
    protected Conductor conductor;
    private DOAnimations doAnimations;
    private string nextState = "Idle";

    //public Animator animator;

    public delegate void OnEnemyRescuedDelegate(GameObject enemyRescued);
    public static event OnEnemyRescuedDelegate OnEnemyRescued;

    //private event Action OnStateChanged;

    private enum State
    {
        Idle,
        Whistle,
        Walk,
        Combate
    }

    [Space(20)]
    [SerializeField] private State _currentState;

    private State CurrentState
    {
        get { return _currentState; }
        set
        {
            _currentState = value;
            SwitchState();
        }
    }

    // private bool iamPaullo;

    [SerializeField] protected Coroutine currentCoroutine;

    [Header("Whistle Settings")]
    [SerializeField] private float whistleWaitTime;
    [SerializeField] float changeStateCooldown;
    bool resetCooldown;

    private Collider2D collider;
    [SerializeField] bool shouldEnableCollider  = true;

    [SerializeField] protected int currentIndex;

    [SerializeField] private List<int> currentCommandSequence;
    public List<int> CurrentCommandSequence { get => currentCommandSequence; set => currentCommandSequence = value; }

    private int currentCombatIndex;

    [Header("Onomatopeia Objects")]
    [SerializeField] private GameObject[] onomatopeia;

    [Header("Command Lists SO")]
    public CommandCombinations basicCommands;
    public CommandCombinations boitataCommands;

    [Header("NPCs & Boss Stuff")]
    public GameObject currentEnemy;
    public GameObject currentBossObject;
    public BossCharacter currentBossScript;

    [SerializeField] bool isFacingBoss =  false;

    private float currentChangeStateCooldown;


    private void Awake()
    {
        doAnimations = GetComponentInChildren<DOAnimations>();
        if (doAnimations == null)
        {
            Debug.LogError("doAnimations reference not found");
        }
    }

    void OnEnable()
    {
        RhythmEvent.onBeat += OnBeat;
        PlayerRhythm.OnActionComplete += HandleAction;
        doAnimations.OnAnimStateChanged += ChangeAnimState;
    }

    void OnDisable()
    {
        RhythmEvent.onBeat -= OnBeat;
        PlayerRhythm.OnActionComplete -= HandleAction;
        doAnimations.OnAnimStateChanged -= ChangeAnimState;
    }

    void Start()
    {
        playerCommands = FindObjectOfType<PlayerCommands>();
        playerRhythm = GetComponent<PlayerRhythm>();
        conductor = FindObjectOfType<Conductor>();

        //animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();

        CurrentState = State.Idle;
        
        CurrentCommandSequence = basicCommands.commandSets[0].commandSequence;

        onomatopeia[0].gameObject.SetActive(false);
        onomatopeia[1].gameObject.SetActive(false);

        currentCombatIndex = 0;

        if (basicCommands == null)
        {
            Debug.LogError("Basic Command SO wasn't serialized properly.");
            return;
        }
    }

    void Update()
    {
        if (CurrentState == State.Whistle || CurrentState == State.Idle)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                ChangeToIdleState();
            }
        }

        if(resetCooldown)
        {
            changeStateCooldown -= Time.deltaTime;

            if (changeStateCooldown < 0)
            {
                resetCooldown = false;
                CurrentState = State.Whistle;
            }
        }
    }

    public void OnBeat()
    {
        if (CurrentState == State.Whistle)
        {
            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }

            currentCoroutine = StartCoroutine(OnomatopeiaHandler());
        }
    }

    // INPUT
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("LostNpc"))
        {
            shouldEnableCollider = false;

            CurrentCommandSequence = basicCommands.commandSets[1 + currentCombatIndex].commandSequence;

            currentEnemy = other.gameObject;

            CurrentState = State.Whistle;
        }

        if (other.TryGetComponent(out BossCharacter boss))
        {
            currentBossScript = boss;
            currentBossObject = other.gameObject;

            if(currentBossScript.bossName == "Boitatá")
            {
                shouldEnableCollider = false;
                isFacingBoss = true;

                currentCombatIndex = 2;
                CurrentCommandSequence = boitataCommands.commandSets[currentCombatIndex].commandSequence;

                CurrentState = State.Whistle;
            }
        }
    }

    // EVALUATION

    void SwitchState()
    {
        switch (CurrentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Whistle:
                HandleWhistle();
                break;
            case State.Walk:
                HandleWalk();
                break;
        }
    }

    void HandleAction(int actionID)
    {
        switch (actionID)
        {
            case -1:
                break;
            case 0:
                ChangeToWalkState();
                break;
            case 1:
                if(CurrentState == State.Whistle && !isFacingBoss)
                {
                    Recruit();
                }
                break;
            case 2:
                BossDance();
                break;
            default:
                break;
        }
    }

    // OUTPUT
    void ChangeAnimState(DOAnimations.State state)
    {
        switch(nextState)
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
            case "DisapproveOno1":
                doAnimations.CurrentState = DOAnimations.State.Disapprove;
                break;
            default:
                Debug.Log("On default case");
                break;
        }
    }

    void HandleIdle()
    {
        changeStateCooldown = currentChangeStateCooldown;
        resetCooldown = true;

        if(shouldEnableCollider)
        {
            DOVirtual.DelayedCall(conductor.SecPerBeat * 3, () =>
            {
                collider.enabled = true;
            });
        }
        else if(!shouldEnableCollider && !isFacingBoss)
        {
            Debug.Log(currentCombatIndex);
            CurrentCommandSequence = basicCommands.commandSets[1 + currentCombatIndex].commandSequence;

            StartCoroutine(SetToWhistle());
        }
    }

    void HandleWhistle()
    {
        collider.enabled = false;
        // The rest is handled in the OnBeat method
    }

    void HandleWalk()
    {
        collider.enabled = false;

        nextState = "Walk";

        StartCoroutine(SetToIdle());
    }

    public void Recruit()
    {
        CurrentCommandSequence = basicCommands.commandSets[0].commandSequence;
        Debug.Log("RECRUIT");
        currentCombatIndex++;

        if(!isFacingBoss)
        {
            if (currentCombatIndex >= 2)
            {
                currentCombatIndex = 0;
                OnEnemyRescued?.Invoke(currentEnemy);
                currentEnemy = null;

                shouldEnableCollider = true;
            }
        }       

        ChangeToIdleState();
    }

    public void BossDance()
    {
        if(currentBossScript.currentDance < 3)
        {
            currentBossScript.DecideAction();
            currentBossScript.currentDance++;
        }
        if (currentCombatIndex == 4)
        {
            currentCombatIndex = 0;
            currentEnemy = null;

            CurrentCommandSequence = basicCommands.commandSets[0].commandSequence;

            DOVirtual.DelayedCall(conductor.SecPerBeat * 4, () =>
            {
                OnEnemyRescued?.Invoke(currentBossScript.gameObject);
            });
        }
        else
        {
            currentCombatIndex++;
            CurrentCommandSequence = boitataCommands.commandSets[currentCombatIndex].commandSequence;
        }

        ChangeToIdleState();
    }

    // COROUTINE RELATED
    public void ChangeToIdleState()
    {
        resetCooldown = false;

        currentIndex = 0;

        StopCoroutine(SetToWhistle());
        StopCoroutine(OnomatopeiaHandler());

        changeStateCooldown = conductor.SecPerBeat * 4;
        currentChangeStateCooldown = changeStateCooldown;

        CurrentState = State.Idle;
    }

    public void ChangeToWalkState()
    {
        StopCoroutine(OnomatopeiaHandler());

        changeStateCooldown = conductor.SecPerBeat * 4;
        currentChangeStateCooldown += changeStateCooldown;

        CurrentState = State.Walk;
    }

    IEnumerator SetToIdle()
    {
        yield return new WaitForSeconds(conductor.SecPerBeat*1);

        ChangeToIdleState();
    }

    IEnumerator SetToWhistle()
    {
        yield return new WaitForSeconds(conductor.SecPerBeat * 4);

        CurrentState = State.Whistle;
    }

    IEnumerator OnomatopeiaHandler()
    {
        GameObject onomatopeia1 = onomatopeia[0].gameObject;
        GameObject onomatopeia2 = onomatopeia[1].gameObject;

        // Emulating a 'for' to maintain the currentIndex's value through different beats
        if (currentIndex < currentCommandSequence.Count)
        {
            var command = currentCommandSequence[currentIndex];
            if (command == 1)
            {
                onomatopeia1.SetActive(true);
                nextState = "WhistleOno1";
            }
            else if(command == 2)
            {
                onomatopeia2.SetActive(true);
                nextState = "WhistleOno2";
            }
        }
        // Ends the 'for'
        else
        {
            currentIndex = 0;
            onomatopeia[0].SetActive(false);
            onomatopeia[1].SetActive(false);

            ChangeToIdleState();

            currentCoroutine = null;

            yield break;
        }

        currentIndex++;

        yield return new WaitForSeconds(conductor.SecPerBeat / 2);

        onomatopeia[0].SetActive(false);
        onomatopeia[1].SetActive(false);
    }
}