using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class WhistlerBrain : MonoBehaviour, IDanceable
{
    private PlayerCommands playerCommands;
    private Conductor conductor;

    public Animator animator;


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

    public GameObject inimigoAfrente;

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
            Debug.Log("Colidiu");
            CurrentCommandSequence = basicCommands.commandSets[1].commandSequence;

            collided = true;

            inimigoAfrente = other.gameObject;

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

    public void ChangeToIdleState()
    {
        _currentState = State.Idle;
        StartCoroutine(SetToWhistle());
    }

    IEnumerator SetToWhistle()
    {
        yield return new WaitForSeconds(0.5f);
        _currentState = State.Whistle;
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
                break;
            case 1:
                Atacar();
                break;
            default:
                break;
        }
        
    }

    public void Atacar()
    {
        Destroy(inimigoAfrente);

        CurrentCommandSequence = basicCommands.commandSets[0].commandSequence;
        ChangeToIdleState();
    }

    void HandleWalk()
    {
        collided = false;
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