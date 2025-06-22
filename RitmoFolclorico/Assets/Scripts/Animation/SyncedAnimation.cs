using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class SyncedAnimation : MonoBehaviour, IDanceable
{
    Conductor conductor;
    RhythmManager rhythmManager;

    [SerializeField] Animator animator;
    [SerializeField] bool isOnBeat;

    [SerializeField] AnimationStateReference animationStateReference, npcRef0, npcRef1;
    [SerializeField] GameObject[] referenceNPC;

    float npcTime0, npcTime1;
    string npcStateName0, npcStateName1;

    AnimatorStateInfo stateInfo;

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
        rhythmManager = FindObjectOfType<RhythmManager>();

        animator = GetComponent<Animator>();
        animationStateReference = FindObjectOfType<AnimationStateReference>();

        npcRef0 = referenceNPC[0].GetComponent<AnimationStateReference>();
        npcStateName0 = npcRef0.stateName;
        

        npcRef1 = referenceNPC[1].GetComponent<AnimationStateReference>();
        npcStateName1 = npcRef1.stateName;
        
    }

    public void OnBeat()
    {
        if(stateInfo.IsName("Walk"))
        {
            if(!isOnBeat)
            {
                isOnBeat = true;

                DOVirtual.DelayedCall(conductor.SecPerBeat / 2, () =>
                {
                    isOnBeat = false;
                });
            }
        }
    }

    public void OnAnimationStarted()
    {
        CheckIfOnBeat();

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Idle"))
        {
            PlayIdle();
        }
        else if (stateInfo.IsName("Walk"))
        {
            if (!isOnBeat)
            {
                StartCoroutine(WaitUntilNextBeat());
            }
        }
    }

    void PlayIdle()
    {
        npcTime0 = npcRef0.normalizedTime;

        float animationFrame = npcTime0;
        animator.Play(npcStateName0, 0, animationFrame);
    }

    void PlayWalk()
    {
        npcTime1 = npcRef1.normalizedTime;

        float animationFrame = npcTime1;
        animator.Play(npcStateName1, 0, animationFrame);
    }

    IEnumerator WaitUntilNextBeat()
    {
        PauseAnimation();

        yield return new WaitUntil(() => isOnBeat);

        UnpauseAnimation();
    }

    void PauseAnimation()
    {
        animator.speed = 0f;
    }

    void UnpauseAnimation()
    {
        animator.speed = 1f;
    }

    void CheckIfOnBeat()
    {
        float beatsPosition = conductor.songPositionInBeats;
        float firstDecimal = Mathf.Floor((beatsPosition % 1f) * 10);


        if (firstDecimal >= rhythmManager.MinDecimal || firstDecimal <= rhythmManager.MaxDecimal)
        {
            isOnBeat = true;
        }
        else
        {
            isOnBeat = false;
        }
    }
}
