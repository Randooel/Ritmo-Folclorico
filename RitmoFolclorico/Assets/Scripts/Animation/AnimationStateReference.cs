using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateReference : MonoBehaviour
{
    Conductor conductor;

    [SerializeField] Animator animator;
    [SerializeField] AnimatorStateInfo stateInfo;
    public string stateName;
    public float normalizedTime;

    [SerializeField] bool isIdle;

    void Start()
    {
        animator = GetComponent<Animator>();

        if(isIdle)
        {
            stateName = "Idle";
        }
        else if(!isIdle)
        {
            stateName = "Walk";
        }
    }

    public void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0); 

        normalizedTime = stateInfo.normalizedTime;
    }

}
