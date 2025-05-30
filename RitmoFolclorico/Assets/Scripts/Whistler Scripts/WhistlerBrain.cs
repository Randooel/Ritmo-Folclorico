using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class WhistlerBrain : MonoBehaviour
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
    private Coroutine currentCoroutine;

    [Header("Whistle Settings")]
    [SerializeField] private float whistleWaitTime;
    // [SerializeField] private float comandName;

    private Collider2D collider;

    [SerializeField] private List<int> currentCommandSequence;
    public List<int> CurrentCommandSequence { get => currentCommandSequence; set => currentCommandSequence = value; }

    [Header("Onomatopeia Objects")]
    [SerializeField] private GameObject[] onomatopeia;

    private bool collided;

    [Header("Command Lists SO")]
    public CommandCombinations basicCommands;
    public CommandCombinations boitataCommands;

    

    void Start()
    {
        playerCommands = FindObjectOfType<PlayerCommands>();
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
                //HandleWalk();
                break;
        }
    }

    // OUTPUT
    void HandleIdle()
    {
        collider.enabled = true;

        StartCoroutine(WaitToWalk());
        CurrentCommandSequence = basicCommands.commandSets[0].commandSequence;

        _currentState = State.Whistle;
    }

    void HandleWhistle()
    {
        collider.enabled = false;

        if (currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(ExecuteCurrentSequence());
        }
    }

    IEnumerator ExecuteCurrentSequence()
    {
        for (int i = 0; i < CurrentCommandSequence.Count;  i++)
        {
            // Debug.Log(i);
            if (CurrentCommandSequence[i] == 1)
            {
                onomatopeia[0].gameObject.SetActive(true);
            }
            else if (CurrentCommandSequence[i] == 2)
            {
                onomatopeia[1].gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(0.4f);

            onomatopeia[0].gameObject.SetActive(false);
            onomatopeia[1].gameObject.SetActive(false);

            yield return new WaitForSeconds(0.4f);
        }

        yield return new WaitForSeconds(0.4f);

        currentCoroutine = null;

        _currentState = State.Idle;
    }

    IEnumerator WaitToWalk()
    {
        yield return new WaitForSeconds(2f);
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(whistleWaitTime);
    }
}
