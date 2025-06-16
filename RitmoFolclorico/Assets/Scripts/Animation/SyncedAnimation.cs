using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class SyncedAnimation : MonoBehaviour, IDanceable
{
    Conductor conductor;
    Animator animator;
    [SerializeField] bool isOnBeat;
    [SerializeField] float defaultAnimatorSpeed;

    void OnEnable()
    {
        RhythmEvent.onBeat += OnBeat;
    }

    void OnDisable()
    {
        RhythmEvent.onBeat -= OnBeat;
    }

    void Start()
    {
        conductor = FindObjectOfType<Conductor>(); 
        animator = GetComponent<Animator>();

        defaultAnimatorSpeed = animator.speed;
    }

    public void OnBeat()
    {
        // Fullfil the WaitUntilNextBeat's requisites to unpause the animation
        isOnBeat = true;

        UnpauseAnimation();

        // Resets the bool so the script will be ready to handle the next animation transition
        DOVirtual.DelayedCall(conductor.SecPerBeat / 2, () =>
        {
            isOnBeat = false;
        });
    }

    public void OnAnimationStarted()
    {
        if (!isOnBeat)
        {
            Debug.LogError("IS OFF BEAT");
            // Updates the animator speed every time a new animation starts off beat
            defaultAnimatorSpeed = animator.speed;

            // Pauses animation
            PauseAnimation();

            StartCoroutine(WaitUntilNextBeat());
        }
    }

    void UnpauseAnimation()
    {
        animator.speed = defaultAnimatorSpeed;
    }

    void PauseAnimation()
    {
        animator.speed = 0;
    }

    IEnumerator WaitUntilNextBeat()
    {
        yield return new WaitUntil(() => isOnBeat);
    }
}
