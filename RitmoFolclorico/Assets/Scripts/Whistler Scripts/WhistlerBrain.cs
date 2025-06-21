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
    private Conductor conductor;

    public Animator animator;

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

    [SerializeField] private Coroutine currentCoroutine;

    [Header("Whistle Settings")]
    [SerializeField] private float whistleWaitTime;
    [SerializeField] float changeStateCooldown;
    bool resetCooldown;

    private Collider2D collider;

    [SerializeField] int currentIndex;

    [SerializeField] private List<int> currentCommandSequence;
    public List<int> CurrentCommandSequence { get => currentCommandSequence; set => currentCommandSequence = value; }

    private int currentCombatIndex;

    [Header("Onomatopeia Objects")]
    [SerializeField] private GameObject[] onomatopeia;

    [Header("Command Lists SO")]
    public CommandCombinations basicCommands;
    public CommandCombinations boitataCommands;

    public GameObject currentEnemy;
    public BossCharacter currentBoss;

    void Start()
    {
        playerCommands = FindObjectOfType<PlayerCommands>();
        playerRhythm = GetComponent<PlayerRhythm>();
        conductor = FindObjectOfType<Conductor>();

        animator = GetComponent<Animator>();
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

    void OnEnable()
    {
        RhythmEvent.onBeat += OnBeat;
        PlayerRhythm.OnActionComplete += HandleAction;
    }

    void OnDisable()
    {
        RhythmEvent.onBeat -= OnBeat;
        PlayerRhythm.OnActionComplete -= HandleAction;
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
            CurrentCommandSequence = basicCommands.commandSets[1 + currentCombatIndex].commandSequence;

            currentEnemy = other.gameObject;

            CurrentState = State.Whistle;
        }

        if(other.TryGetComponent(out BossCharacter boss))
        {
            currentBoss = boss;
            if(currentBoss.bossName == "Boitata")
            {
                CurrentCommandSequence = boitataCommands.commandSets[2 + currentCombatIndex].commandSequence;
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
                if(CurrentState == State.Whistle)
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
    void HandleIdle()
    {
        resetCooldown = true;

        DOVirtual.DelayedCall(conductor.SecPerBeat * 3, () =>
        {
            collider.enabled = true;
        });        
    }

    void HandleWhistle()
    {
        collider.enabled = false;
        // The rest is handled in the OnBeat method
    }

    void HandleWalk()
    {
        collider.enabled = false;

        animator.SetTrigger("isWalking");

        StartCoroutine(SetToIdle());
    }

    public void Recruit()
    {
        CurrentCommandSequence = basicCommands.commandSets[0].commandSequence;
        currentCombatIndex++;

        if (currentCombatIndex >= 2)
        {
            currentCombatIndex = 0;
            OnEnemyRescued?.Invoke(currentEnemy);
            currentEnemy = null;
        }

        ChangeToIdleState();
    }

    public void BossDance()
    {
        CurrentCommandSequence = basicCommands.commandSets[0].commandSequence;
        currentCombatIndex++;

        if (currentCombatIndex >= 3)
        {
            currentCombatIndex = 0;
            OnEnemyRescued?.Invoke(currentBoss.gameObject);
            currentEnemy = null;
        }

        SetToIdle();
    }

    // COROUTINE RELATED
    public void ChangeToIdleState()
    {
        StopCoroutine(OnomatopeiaHandler());

        changeStateCooldown = conductor.SecPerBeat * 3;

        CurrentState = State.Idle;
    }

    public void ChangeToWalkState()
    {
        StopCoroutine(OnomatopeiaHandler());

        changeStateCooldown = conductor.SecPerBeat * 4;

        CurrentState = State.Walk;
    }

    IEnumerator SetToIdle()
    {
        yield return new WaitForSeconds(conductor.SecPerBeat*1);

        CurrentState = State.Idle;
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
                animator.SetTrigger("isWhistlingOno1");
            }
            else if(command == 2)
            {
                onomatopeia2.SetActive(true);
                animator.SetTrigger("isWhistlingOno2");
            }
        }
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