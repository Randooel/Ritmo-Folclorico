using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedAnimation : MonoBehaviour
{
    public Animator animator;

    public AnimatorStateInfo animatorStateInfo;

    public int currentState;

    void Start()
    {
        animator = GetComponent<Animator>();

        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        currentState = animatorStateInfo.fullPathHash;
    }

    void Update()
    {
        animator.Play(currentState, -1, (Conductor.instance.loopPositionInAnalog));

        animator.speed = 0;

    }
}
