using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class WhistlerBrain : MonoBehaviour, IDanceable
{
    private PlayerCommands playerCommands;
    private PlayerRhythm playerRhythm;
    private Conductor conductor;

    public Animator animator;

    private bool beatHappened = false;
    private enum State
    {
        Idle,
        Whistle,
        Walk
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

    private string[] commandN;

    [Header("Onomatopeia Objects")]
    [SerializeField] private GameObject[] onomatopeia;

    private bool collided;

    [Header("Command Lists SO")]
    public CommandCombinations basicCommands;
    public CommandCombinations boitataCommands;

    void Start()
    {
        playerCommands = FindObjectOfType<PlayerCommands>();
        playerRhythm = FindObjectOfType<PlayerRhythm>();
        conductor = FindObjectOfType<Conductor>();

        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();

        _currentState = State.Idle;

        onomatopeia[0].gameObject.SetActive(false);
        onomatopeia[1].gameObject.SetActive(false);

        collided = false;

        if (basicCommands == null)
        {
            Debug.LogError("Basic Command SO wasn't serialized properly.");
            return;
        }
    }

    // INPUT
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collided)
        {
            return;
        }

        if (other.gameObject.CompareTag("LostNpc"))
        {
            CurrentCommandSequence = basicCommands.commandSets[1].commandSequence;

            collided = true;

            _currentState = State.Whistle;
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
    }

    // OUTPUT
    void HandleIdle()
    {
        collider.enabled = true;

        CurrentCommandSequence = basicCommands.commandSets[0].commandSequence;

        _currentState = State.Whistle;
    }

    void HandleWhistle()
    {
        collider.enabled = false;

        if(playerRhythm.isOnAction == true)
        {
            HandleAction();
        }

        // The rest is handled in the OnBeat method
    }

    void HandleAction()
    {
        if (playerRhythm.currentAction == 0)
        {
            _currentState = State.Walk;
        }
    }

    void HandleWalk()
    {
        Debug.LogError(_currentState);
        
        StartCoroutine(WaitToIdle());
    }

    IEnumerator WaitToIdle()
    {
        yield return new WaitForSeconds(conductor.SecPerBeat * 2);

        _currentState = State.Idle;
    }

    void OnEnable()
    {
        RhythmEvent.onBeat += OnBeat;
    }

    void OnDisable()
    {
        RhythmEvent.onBeat -= OnBeat;
    }

    public void OnBeat()
    {
        currentCoroutine = StartCoroutine(ExecuteCurrentSequence());
    }

    IEnumerator ExecuteCurrentSequence()
    {
        Debug.Log(currentIndex);
        GameObject onomatopeia1 = onomatopeia[0].gameObject;
        GameObject onomatopeia2 = onomatopeia[1].gameObject;

        // Emulating a 'for' to maintain the currentIndex's value through different beats
        if (currentIndex < currentCommandSequence.Count)
        {
            var command = currentCommandSequence[currentIndex];
            if (command == 1)
            {
                onomatopeia1.SetActive(true);
            }
            else if(command == 2)
            {
                onomatopeia2.gameObject.SetActive(true);
            }
        }
        else
        {
            currentIndex = 0;
            onomatopeia[0].gameObject.SetActive(false);
            onomatopeia[1].gameObject.SetActive(false);
            yield break;
        }

        currentIndex++;

        yield return new WaitForSeconds(0.3f);

        onomatopeia[0].gameObject.SetActive(false);
        onomatopeia[1].gameObject.SetActive(false);
    }
}