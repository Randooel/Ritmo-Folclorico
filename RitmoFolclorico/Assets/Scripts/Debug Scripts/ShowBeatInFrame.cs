using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBeatInFrame : MonoBehaviour, IDanceable
{
    [SerializeField] Animator animator;

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
        if(animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void OnBeat()
    {
        DebugFrame();
    }

    public void DebugFrame()
    {
        if (animator == null)
        {
            return;
        }

        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

        if (clipInfo.Length == 0)
        {
            Debug.LogWarning("Nenhum AnimationClip encontrado.");
            return;
        }

        AnimationClip currentClip = clipInfo[0].clip;
        float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        float clipLength = currentClip.length;
        float currentTime = normalizedTime * clipLength;

        float frameRate = currentClip.frameRate;
        int currentFrame = Mathf.FloorToInt(currentTime * frameRate);

        Debug.Log($"[Animation Debug] Clip: {currentClip.name}, Time: {currentTime:F3}s, Frame: {currentFrame}");
    }
}
