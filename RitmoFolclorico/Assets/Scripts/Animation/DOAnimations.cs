using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class DOAnimations : MonoBehaviour, IDanceable
{
    // SETUP
    // Another scripts reference
    [SerializeField] Conductor _conductor;

    [Header("State Parameters")]

    [SerializeField] State currentState;
    public State CurrentState
    {
        get => currentState;
        set
        {
            if(currentState != value)
            {
                currentState = value;
                OnAnimStateChanged?.Invoke(currentState);
            }
        }
    }

    public event Action<State> OnAnimStateChanged;

    public enum State
    {
        Idle,
        Walk,
        Ono1,
        Ono2,
        WhistleOno1,
        WhistleOno2,
        Disapprove
    }

    [Header("Visual Parameters")]

    [SerializeField] Transform _visualObject;
    [SerializeField] Transform _bodyObject;

    private Vector3 originalScale;

    private Vector3 startPos;

    [SerializeField] float _oneBeat;

    

    void OnEnable()
    {
        OnAnimStateChanged += CheckCurrentState;
        RhythmEvent.onBeat += OnBeat;
    }
    void OnDisable()
    {
        OnAnimStateChanged -= CheckCurrentState;
        RhythmEvent.onBeat -= OnBeat;
    }

    void Start()
    {
        _conductor = FindObjectOfType<Conductor>();

        if (_conductor != null )
        {
            _oneBeat = 0.6f;
        }
        else
        {
            Debug.LogWarning("Conductor script not found.");
        }

        CurrentState = State.Idle;      

        if(_visualObject == null)
        {
            _visualObject = transform.GetChild(0).transform;

            if (_visualObject == null)
            {
                Debug.LogError("Please assign a visualObject");
            }
        }
        else
        {
            startPos = _visualObject.localPosition;
        }

        if(_bodyObject == null)
        {
            Debug.LogError("Please assign a bodyObject");
        }
    }



    // INPUT
    public void OnBeat()
    {
        DOTilt();
    }

    void SetIdle()
    {
        CurrentState = State.Idle;
        //CheckCurrentState(State.Idle);
    }
    void SetWalk()
    {
        CurrentState = State.Walk;
        CheckCurrentState(State.Walk);
    }
    void SetOno1()
    {
        CurrentState = State.Ono1;
        CheckCurrentState(State.Ono1);
    }
    void SetOno2()
    {
        CurrentState = State.Ono2;
        CheckCurrentState(State.Ono2);
    }
    void SetWhistleOno1()
    {
        CurrentState = State.WhistleOno1;
        CheckCurrentState(State.WhistleOno1);
    }
    void SetWhistleOno2()
    {
        CurrentState = State.WhistleOno2;
        CheckCurrentState(State.WhistleOno2);
    }
    void SetDisapprove()
    {
        CurrentState = State.Disapprove;
        CheckCurrentState(State.Disapprove);
    }



    // EVALUATION
    void CheckCurrentState(State state)
    {
        switch (CurrentState)
        {
            case State.Idle:
                DOIdle();
                break;
            case State.Walk:
                DOWalk();
                break;
            case State.Ono1:
                DOOno1();
                break;
            case State.Ono2:
                DOOno2();
                break;
            case State.WhistleOno1:
                DOWhistleOno1();
                break;
            case State.WhistleOno2:
                DOWhistleOno2();
                break;
            case State.Disapprove:
                DODisapprove();
                break;
            default:
                Debug.LogWarning("Unknow State: "+ CurrentState);
                break;
        }
    }



    // OUTPUT
    void KillPreviousTilt()
    {
        DOTween.Kill(_visualObject);
        DOTween.Kill(_bodyObject);
    }

    // Animations
    void DOTilt()
    {
        KillPreviousTilt();

        Sequence seq = DOTween.Sequence();

        seq.Append(_bodyObject.DOScale((new Vector3(1.2f, 1.2f, 1)), _oneBeat / 4).SetEase(Ease.OutQuad));
        seq.Append(_bodyObject.DOScale((new Vector3(1f,1f,1f)), _oneBeat / 3).SetEase(Ease.OutQuad));
    }
    void DOIdle()
    {
        KillPreviousTilt();
    }
    void DOWalk()
    {
        KillPreviousTilt();

        _visualObject.DOMoveY(2, _oneBeat /2).SetLoops(3);
    }
    void DOOno1()
    {
        KillPreviousTilt();
        DOTween.Kill(_visualObject);

        float duration = _oneBeat / 2;

        Debug.Log(2);

        _visualObject.DOLocalMoveY(1.2f, duration).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        {
            SetIdle();
        });
    }
    void DOOno2()
    {
        KillPreviousTilt();

        _visualObject.DOMoveY(-0.5f, _oneBeat / 2).OnComplete(() =>
        {
            _visualObject.DOMoveY(0, _oneBeat / 2);
        });
        _visualObject.DOMoveX(0.3f, _oneBeat / 2).OnComplete(() =>
        {
            _visualObject.DOMoveX(0, _oneBeat / 2);
        });
    }

    // Whistler/NPC Exclusive Animations
    void DOWhistleOno1()
    {
        KillPreviousTilt();

        // Animation logic goes here
    }
    void DOWhistleOno2()
    {
        KillPreviousTilt();

        // Animation logic goes here
    }
    void DODisapprove()
    {
        KillPreviousTilt();

        // Animation logic goes here
    }
}
