using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boitata : BossCharacter
{
    WhistlerBrain _whistlerBrain;
    Conductor _conductor;

    void Start()
    {
        _whistlerBrain = FindObjectOfType<WhistlerBrain>();
        _conductor = FindObjectOfType<Conductor>();

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    public override void DecideAction()
    {
        if(currentDance == 0)
        {
            Dance1();
        }

        if(currentDance == 1)
        {
            Dance2();
        }

        if(currentDance == 2)
        {
            Dance3();
        }
    }

    public override void Dance1()
    {
        animator.SetTrigger("isDancing1");
    }

    public override void Dance2()
    {
        animator.SetTrigger("isDancing2");
    }

    public override void Dance3()
    {
        animator.SetTrigger("isDancing3");
    }

    IEnumerator Dance()
    {
        yield return new WaitForSeconds(_conductor.SecPerBeat * 2);
    }
}
