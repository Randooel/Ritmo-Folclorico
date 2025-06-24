using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossCharacter : MonoBehaviour
{
    [SerializeField] public string bossName;
    [SerializeField] PlayerCommands playerCommands;

    protected Animator animator;

    public int currentDance;
    protected float tempoAnim;

    void Start()
    {
        if(playerCommands == null)
        {
            playerCommands = FindObjectOfType<PlayerCommands>();
        }
    }

    void Update()
    {

    }

    public abstract void DecideAction();

    public abstract void Dance1();

    public abstract void Dance2();

    public abstract void Dance3();

    public virtual void DeactivatePlayerCommandable()
    {
        if (playerCommands != null)
        {
            playerCommands.commandable = false;
        }
        else
        {
            Debug.LogError("PlayerCommands was not found");
        }
    }

    public virtual void ActivatePlayerCommandable()
    {
        playerCommands.commandable = true;
    }
}
