using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhistlerBrain : MonoBehaviour
{
    public CommandCombinations basicCommands;
    public CommandCombinations levelCommands;

    void Start()
    {
        if(basicCommands == null)
        {
            Debug.LogError("Basic Command SO wasn't serialized properly.");
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

        }
    }
}
