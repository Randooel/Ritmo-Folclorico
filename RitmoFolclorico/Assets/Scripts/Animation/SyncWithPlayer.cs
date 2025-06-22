using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SyncWithPlayer : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;
    [SerializeField] AnimatorStateInfo stateInfo;
    [SerializeField] Animator[] followerAnimator;

    public float normalizedTime;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    
    void Update()
    {
        stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

        normalizedTime = stateInfo.normalizedTime;
    }

    public void OnOno1()
    {

    }

    public void OnOno2()
    {
        
    }
}
