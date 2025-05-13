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
        Whistling,
        Walking
    }

    [SerializeField] private State currentState;
    private Coroutine currentCoroutine;
    [Header("Whistle Settings")]
    [SerializeField] private float whistleWaitTime;
    // [SerializeField] private float comandName;

    [SerializeField] private List<int> currentCommandSequence;
    [SerializeField] private string currentCommandName;

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

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Whistling:
                HandleWhistling();
                break;
            case State.Walking:
                HandleWalk();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(collided)
        {
            return;
        }
        if(other.gameObject.CompareTag("LostNpc"))
        {
            currentCommandName = "SaveNpc1";

            collided = true;

            ReadCommand();
        }
    }

    void ReadCommand()
    {
        switch (currentCommandName)
        {
            case "Walk":
                HandleWalk();
                break;
            case "SaveNpc1":
                HandleSaveNpc();
                break;
            case "PutOutFire1":
                HandlePutOutFire();
                break;
            case "Boitata1":
                HandleBoitataDance();
                break;
        }
    }

    void HandleIdle()
    {
        // animator.SetTrigger("Idle");
    }

    void HandleWhistling()
    {

    }

    void HandleWalk()
    {
        currentState = State.Walking;
    }
    void HandleSaveNpc()
    {
        Debug.Log("SAVE NPC!");

        currentCommandSequence = basicCommands.commandSets[1].commandSequence;

        StartCoroutine(ExecuteCurrentSequence());
    }

    void HandlePutOutFire()
    {

    }

    void HandleBoitataDance()
    {

    }

    IEnumerator ExecuteCurrentSequence()
    {
        for (int i = 0; i < currentCommandSequence.Count;  i++)
        {
            Debug.Log(i);
            if(currentCommandSequence[i] == 1)
            {
                onomatopeia[0].gameObject.SetActive(true);
            }
            else if (currentCommandSequence[i] == 2)
            {
                onomatopeia[1].gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(0.3f);

            onomatopeia[0].gameObject.SetActive(false);
            onomatopeia[1].gameObject.SetActive(false);
        }
    }

    /*
    public void ReadCommandSetByName(string name)
    {
        var selectedSet = basicCommands.commandSets.Find(set => set.commandName == name);
        
        if(selectedSet != null)
        {
            foreach (var command in selectedSet.commandSequence)
            {
                Debug.Log(command);
            }
        }
        else
        {
            Debug.LogError($"Command with name {name} not found!");
        }
    }
    */

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(whistleWaitTime);

    }
}
