using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhistlerBrain : MonoBehaviour
{
    public CommandCombinations basicCommands;
    public CommandCombinations levelCommands;

    [SerializeField] float whistleWaitTime;

    [SerializeField] private List<int> currentCommandSequence;
    [SerializeField] private int currentCommandIndex = 0;

    void Start()
    {
        if(basicCommands == null)
        {
            Debug.LogError("Basic Command SO wasn't serialized properly.");
            return;
        }
        else
        {
            //List<>
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("LostNpc"))
        {
            StartCoroutine(PlayCommandSequence());
        }
    }

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

    IEnumerator PlayCommandSequence()
    {
        yield return new WaitForSeconds(whistleWaitTime);

    }
}
