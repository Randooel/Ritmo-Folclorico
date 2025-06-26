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

    [SerializeField] GameObject _visualObject;

    private Vector2 originalScale;

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
        if(_conductor != null )
        {
            _oneBeat = 0.6f;
            Debug.Log(_oneBeat);
        }
        else
        {
            Debug.LogWarning("Conductor script not found.");
        }

        CurrentState = State.Idle;

        _visualObject = transform.GetChild(0).gameObject ;

        if(_visualObject == null)
        {
            Debug.LogError("Please assign a visual Object");
        }
        else
        {
            originalScale = _visualObject.transform.localScale;
        }
    }



    // INPUT
    public void OnBeat()
    {
        CheckCurrentState(currentState);
    }

    void SetIdleState()
    {
        CurrentState = State.Idle;
    }
    void SetWalkState()
    {
        CurrentState = State.Walk;
    }
    void SetOno1State()
    {
        CurrentState = State.Ono1;
    }
    void SetOno2State()
    {
        CurrentState = State.Ono2;
    }
    void SetWhistleOno1State()
    {
        CurrentState = State.WhistleOno1;
    }
    void SetWhistleOno2State()
    {
        CurrentState = State.WhistleOno2;
    }
    void SetDisapproveState()
    {
        CurrentState = State.Disapprove;
    }



    // EVALUATION
    void CheckCurrentState(State state)
    {
        switch (CurrentState)
        {
            case State.Idle:
                HandleIdleState();
                break;
            case State.Walk:
                HandleWalkState();
                break;
            case State.Ono1:
                HandleOno1State();
                break;
            case State.Ono2:
                HandleOno2State();
                break;
            case State.WhistleOno1:
                HandleWhistleOno1State();
                break;
            case State.WhistleOno2:
                HandleWhistleOno2State();
                break;
            case State.Disapprove:
                HandleDisapproveState();
                break;
            default:
                Debug.LogWarning("Unknow State: "+ CurrentState);
                break;
        }
    }



    // OUTPUT
    void HandleIdleState()
    {
        DOIdle();
    }
    void HandleWalkState()
    {
        DOWalk();
    }
    void HandleOno1State()
    {
        DOOno1();
    }
    void HandleOno2State()
    {
        DOOno2();
    }
    void HandleWhistleOno1State()
    {
        DOWhistleOno1();
    }
    void HandleWhistleOno2State()
    {
        DOWhistleOno2();
    }
    void HandleDisapproveState()
    {
        DODisapprove();
    }

    // Animations
    void DOIdle()
    {
        _visualObject.transform.DOScale(originalScale * new Vector2(0.8f, 1.2f), _oneBeat / 2).OnComplete(() =>
        {
            _visualObject.transform.DOScale(originalScale, _oneBeat / 2);
        });
    }
    void DOWalk()
    {

    }
    void DOOno1()
    {

    }
    void DOOno2()
    {

    }
    // Whistler/NPC Exclusive Animations
    void DOWhistleOno1()
    {

    }
    void DOWhistleOno2()
    {

    }
    void DODisapprove()
    {

    }
}
