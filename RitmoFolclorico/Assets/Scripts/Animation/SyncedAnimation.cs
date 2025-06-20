using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class SyncedAnimation : MonoBehaviour
{
    Conductor conductor;
    [SerializeField] Animator animator;

    [SerializeField] AnimationStateReference animationStateReference, npcRef0, npcRef1;
    [SerializeField] GameObject[] referenceNPC;
    float npcTime0, npcTime1;
    string npcStateName0, npcStateName1;

    AnimatorStateInfo stateInfo;

    void Start()
    {
        conductor = FindObjectOfType<Conductor>();
        animator = GetComponent<Animator>();
        animationStateReference = FindObjectOfType<AnimationStateReference>();

        npcRef0 = referenceNPC[0].GetComponent<AnimationStateReference>();
        npcStateName0 = npcRef0.stateName;
        

        npcRef1 = referenceNPC[1].GetComponent<AnimationStateReference>();
        npcStateName1 = npcRef1.stateName;
        
    }

    public void OnAnimationStarted()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Idle"))
        {
            npcTime0 = npcRef0.normalizedTime;
            

            float animationFrame = npcTime0;
            animator.Play(npcStateName0, 0, animationFrame);
        }
        else if (stateInfo.IsName("Walk"))
        {
            npcTime1 = npcRef1.normalizedTime;

            float animationFrame = npcTime1;
            animator.Play(npcStateName1, 0, animationFrame);
        }
    }
}
