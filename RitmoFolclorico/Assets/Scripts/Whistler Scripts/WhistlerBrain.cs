using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class WhistlerBrain : MonoBehaviour, IDanceable
{
    private PlayerCommands playerCommands;
    private Conductor conductor;

    public Animator animator;

    public delegate void OnEnemyRescuedDelegate(GameObject enemyRescued);
    public static event OnEnemyRescuedDelegate OnEnemyRescued;

    private enum State
    {
        Idle,
        Whistle,
        Walk,
        Combate
    }

    [Space(20)]
    [SerializeField] private State _currentState;

    // private bool iamPaullo;

    [SerializeField] private Coroutine currentCoroutine;

    [Header("Whistle Settings")]
    [SerializeField] private float whistleWaitTime;
    // [SerializeField] private float comandName;

    private Collider2D collider;

    [SerializeField] private List<int> currentCommandSequence;
    public List<int> CurrentCommandSequence { get => currentCommandSequence; set => currentCommandSequence = value; }

    private int currentIndex;
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
        conductor = FindObjectOfType<Conductor>();

        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();

        _currentState = State.Idle;

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

    // INPUT
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("LostNpc"))
        {
            CurrentCommandSequence = basicCommands.commandSets[1 + currentCombatIndex].commandSequence;

            currentEnemy = other.gameObject;

            _currentState = State.Whistle;
        }

        if(other.TryGetComponent(out BossCharacter boss))
        {
            currentBoss = boss;
            if(currentBoss.bossName == "Boitata")
            {
                CurrentCommandSequence = boitataCommands.commandSets[2 + currentCombatIndex].commandSequence;
                _currentState = State.Whistle;
            }
        }
    }

    // EVALUATION
    void Update()
    {
        switch (_currentState)
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

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            _currentState = State.Idle;
        }
    }

    public void ChangeToIdleState()
    {
        _currentState = State.Idle;
        StartCoroutine(SetToWhistle());
    }

    IEnumerator SetToWhistle()
    {
        yield return new WaitForSeconds(conductor.SecPerBeat*2);
        _currentState = State.Whistle;
    }
    IEnumerator SetToIdle()
    {
        yield return new WaitForSeconds(conductor.SecPerBeat*2);
        ChangeToIdleState();
    }

    // OUTPUT
    void HandleIdle()
    {
        collider.enabled = true;
    }

    void HandleWhistle()
    {
        collider.enabled = false;
        // The rest is handled in the OnBeat method
    }

    void HandleAction(int actionID)
    {
        switch (actionID)
        {
            case -1:
                break;
            case 0:
                _currentState = State.Walk;
                StartCoroutine(SetToIdle());
                break;
            case 1:
                Recruit();
                break;
            case 2:
                BossDance();
                break;
            default:
                break;
        }
        
    }

    public void Recruit()
    {
        CurrentCommandSequence = basicCommands.commandSets[0].commandSequence;
        currentCombatIndex++;

        if(currentCombatIndex >= 2)
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

        ChangeToIdleState();
    }

    void HandleWalk()
    {
        animator.SetTrigger("isWalking");
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
        currentCoroutine = StartCoroutine(OnomatopeiaHandler());
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
            yield break;
        }

        currentIndex++;

        yield return new WaitForSeconds(conductor.SecPerBeat / 2);

        onomatopeia[0].SetActive(false);
        onomatopeia[1].SetActive(false);
    }
}