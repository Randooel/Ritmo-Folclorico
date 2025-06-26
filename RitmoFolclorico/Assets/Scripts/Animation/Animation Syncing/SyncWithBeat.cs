using UnityEngine;

public class StaticSyncedAnimation : MonoBehaviour
{
    public Animator animator;
    public string stateName = "Idle";

    private int stateHash;
    private bool isPlaying = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        stateHash = Animator.StringToHash(stateName);
        PlaySyncedAnimation();
    }

    void Update()
    {
        if (!isPlaying) return;

        // Atualiza normalized time baseado no loop da música
        float normalizedTime = Conductor.instance.loopPositionInAnalog;

        // Usa CrossFade para evitar reset abrupto e atualiza normalizedTime manualmente
        animator.Play(stateHash, 0, normalizedTime);
    }

    public void PlaySyncedAnimation()
    {
        isPlaying = true;
        animator.speed = 1f;
        animator.Play(stateHash, 0, Conductor.instance.loopPositionInAnalog);
    }

    public void StopAnimation()
    {
        isPlaying = false;
        animator.speed = 0f;
    }

    public void ChangeState(string newState)
    {
        stateName = newState;
        stateHash = Animator.StringToHash(stateName);
        PlaySyncedAnimation();
    }
}
