using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private State currentState;
    private Coroutine currentCoroutine;
    [Header("Whistle Settings")]
    [SerializeField] private float whistleWaitTime;
    // [SerializeField] private float comandName;

    private Collider2D collider;

    [SerializeField] private List<int> currentCommandSequence;

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

        currentState = State.Idle;

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
        else if (other.gameObject.CompareTag("LostNpc"))
        {
            currentCommandSequence = basicCommands.commandSets[1].commandSequence;

            collided = true;

            currentState = State.Whistle;
        }
    }

    // EVALUATION
    void Update()
    {
        switch (currentState)
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
    }

    void HandleWhistle()
    {
        collider.enabled = false;

        if (currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(ExecuteCurrentSequence());
        }
    }

    void HandleWalk()
    {
        
    }

    void HandlePutOutFire()
    {

    }

    void HandleBoitataDance()
    {

    }

    IEnumerator ExecuteCurrentSequence()
    {
        Debug.Log("Bombardira Coroutina");
        for (int i = 0; i < currentCommandSequence.Count;  i++)
        {
            Debug.Log(i);
            if (currentCommandSequence[i] == 1)
            {
                onomatopeia[0].gameObject.SetActive(true);
            }
            else if (currentCommandSequence[i] == 2)
            {
                onomatopeia[1].gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(0.4f);

            onomatopeia[0].gameObject.SetActive(false);
            onomatopeia[1].gameObject.SetActive(false);

            yield return new WaitForSeconds(0.4f);
        }

        currentCoroutine = null;

        currentState = State.Idle;
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(whistleWaitTime);
    }
}
