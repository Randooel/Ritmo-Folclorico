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
    [SerializeField] GameObject _bodyObject;

    private Vector2 originalScale;

    [SerializeField] float _oneBeat;

    // DOTween IDs
    const string TiltTweenID = "Tilt";
    const string IdleTweenID = "Idle;";
    const string WalkTweenID = "Walk";
    const string Ono1TweenID = "Ono1";
    const string Ono2TweenID = "Ono2";
    const string WhistleOno1TweenID = "WhislteOno1";
    const string WhistleOno2TweenID = "WhistleOno2";
    const string DisapproveOno2TweenID = "Disapprove";

    

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
        }
        else
        {
            Debug.LogWarning("Conductor script not found.");
        }

        CurrentState = State.Idle;      

        if(_visualObject == null)
        {
            _visualObject = transform.GetChild(0).gameObject;

            if (_visualObject == null)
            {
                Debug.LogError("Please assign a visualObject");
            }
        }
        else
        {
            originalScale = _visualObject.transform.localScale;
        }

        if(_bodyObject == null)
        {
            Debug.LogError("Please assign a bodyObject");
        }
    }

    void Update()
    {
        if(currentState == State.Ono1)
        {
            DOOno1();
        }
        else if(currentState == State.Ono2)
        {
            DOOno2();
        }
    }



    // INPUT
    public void OnBeat()
    {
        DOTilt();
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
    void KillPreviousTilt()
    {
        DOTween.Kill(TiltTweenID, true);
    }
    void DOTilt()
    {
        KillPreviousTilt();

        _visualObject.transform.DOScale((new Vector3(1.2f, 1.2f, 0)), _oneBeat / 4)
            .SetId("tilt").SetEase(Ease.OutQuad).OnComplete(() =>
            {
                _visualObject.transform.DOScale((originalScale), _oneBeat / 3)
                .SetId("tilt").SetEase(Ease.OutQuad);
            });
    }
    void DOIdle()
    {
        KillPreviousTilt();

        _bodyObject.transform.DOScale(originalScale * new Vector2(0.8f, 1.2f), _oneBeat / 2)
            .SetId("Idle").OnComplete(() =>
            {
                _bodyObject.transform.DOScale(originalScale, _oneBeat / 2).SetId("Idle");
            });
        /*
        _bodyObject.transform.DOMoveY(0.004f, _oneBeat / 2).OnComplete(() =>
        {
            _bodyObject.transform.DOMoveY(-0.004f, _oneBeat / 2);
        });
        */
    }
    void DOWalk()
    {
        KillPreviousTilt();

        _bodyObject.transform.DOMoveY(2, _oneBeat /2).SetLoops(3);
    }
    void DOOno1()
    {
        KillPreviousTilt();

        _bodyObject.transform.DOMoveY(0.5f, _oneBeat / 2).OnComplete(() =>
        {
            _bodyObject.transform.DOMoveY(0, _oneBeat / 2);
        });
        _bodyObject.transform.DOMoveX(0.3f, _oneBeat / 2).OnComplete(() =>
        {
            _bodyObject.transform.DOMoveX(0, _oneBeat / 2);
        });
    }
    void DOOno2()
    {
        KillPreviousTilt();

        // Animation logic goes here
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
