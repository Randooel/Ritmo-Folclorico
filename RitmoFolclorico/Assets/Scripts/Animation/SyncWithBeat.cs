using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncWithBeat : MonoBehaviour
{
    public Animator animator;
    public string stateName = "Idle";

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float normalizedTime = Conductor.instance.loopPositionInAnalog;

        animator.Play(stateName, 0, normalizedTime);
        animator.speed = 0;
    }

    // Functions to be called by Animation Events
    public void IdleState()
    {
        stateName = "Idle";
    }

    public void WalkState()
    {
        stateName = "Walk";
    }

    public void Ono1State()
    {
        stateName = "Ono1";
    }

    public void Ono2State()
    {
        stateName = "Ono2";
    }

    public void DanceOno1State()
    {
        stateName = "DanceOno1";
    }

    public void DanceOno2State()
    {
        stateName = "Ono2";
    }
}