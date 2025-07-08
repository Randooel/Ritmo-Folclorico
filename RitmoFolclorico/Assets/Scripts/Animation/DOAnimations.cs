using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Unity.VisualScripting;

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
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Vector3 startPos;

    [SerializeField] float _oneBeat;

    // Idle animation bools
    private bool _isOnBeat;
    private bool _isBeat1 = true;

    

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

        GetOriginalTransforms();

        CurrentState = State.Idle;
        CheckCurrentState(CurrentState);

        if (_visualObject == null)
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
        _isBeat1 = !_isBeat1;

        _isOnBeat = true;
        DOVirtual.DelayedCall(0.1f, () =>
        {
            _isOnBeat = false;
        });

        if (CurrentState == State.Idle)
        {
            DOIdleLoop();
        }

        DOTilt();
    }

    void SetIdle()
    {
        currentState = State.Idle;
        CheckCurrentState(CurrentState);
    }
    void SetWalk()
    {
        currentState = State.Walk;
        CheckCurrentState(CurrentState);
    }
    void SetOno1()
    {
        currentState = State.Ono1;
        CheckCurrentState(CurrentState);
    }
    void SetOno2()
    {
        currentState = State.Ono2;
        CheckCurrentState(CurrentState);
    }
    void SetWhistleOno1()
    {
        currentState = State.WhistleOno1;
        CheckCurrentState(CurrentState);
    }
    void SetWhistleOno2()
    {
        currentState = State.WhistleOno2;
        CheckCurrentState(CurrentState);
    }
    void SetDisapprove()
    {
        currentState = State.Disapprove;
        CheckCurrentState(CurrentState);
    }



    // EVALUATION
    public void CheckCurrentState(State state)
    {
        DOResetTransforms();
        OnAnimationStarted();

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
                Debug.LogWarning("Unknow State: " + CurrentState);
                break;
        }
    }



    // OUTPUT
    void GetOriginalTransforms()
    {
        originalPosition = _visualObject.localPosition;
        originalScale = _visualObject.localScale;
        originalRotation = _visualObject.rotation;

        //Debug.Log("Position: " + originalPosition + "Scale: " + originalScale + "Rotation: " + originalRotation);
    }

    void KillPreviousTilt()
    {
        DOTween.Kill(_bodyObject);
    }

    void KillPreviousTween()
    {
        DOTween.Kill(_visualObject);
    }

    // Animations
    void OnAnimationStarted()
    {
        KillPreviousTilt();
        KillPreviousTween();

        DOResetTransforms();
    }

    void DOResetTransforms()
    {
        _visualObject.DOScale(originalScale, _oneBeat / 2);
        _visualObject.DOLocalMove(originalPosition, _oneBeat / 2);
        _visualObject.DORotate(originalRotation.eulerAngles, _oneBeat / 2);
    }

    void DOTilt()
    {
        KillPreviousTilt();

        DG.Tweening.Sequence seq = DOTween.Sequence();

        seq.Append(_bodyObject.DOScale((new Vector3(1.1f, 1.1f, 1)), _oneBeat / 4).SetEase(Ease.OutSine));
        seq.Append(_bodyObject.DOScale((new Vector3(1f,1f,1f)), _oneBeat / 3).SetEase(Ease.OutSine));
    }
    void DOIdle()
    {
        float startZ = _isBeat1 ? 10f : -10f;
        _visualObject.localRotation = Quaternion.Euler(0, 0, startZ);
    }
    void DOIdleLoop()
    {
        OnAnimationStarted();

        float startZ = _isBeat1 ? 10f : -10f;
        float targetZ = -startZ;

        _visualObject.localRotation = Quaternion.Euler(0, 0, startZ);

        _visualObject.DOLocalRotate(new Vector3(0, 0, targetZ), _oneBeat).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
    void DOWalk()
    {
        KillPreviousTilt();

        _visualObject.DOMoveY(2, _oneBeat /2).SetLoops(3);
    }
    void DOOno1()
    {
        float duration = _oneBeat / 2;

        DG.Tweening.Sequence seq = DOTween.Sequence();

        // Anticipation
        seq.Append(_visualObject.DOLocalMoveY(-0.4f, duration / 4).SetEase(Ease.InOutSine));
        seq.Join(_visualObject.DOScale(new Vector3(1.4f, 0.6f, 0), duration / 4));

        // Action
        seq.Append(_visualObject.DOLocalRotate(new Vector3(0, 0, 30f), duration).SetEase(Ease.OutSine));
        seq.Join(_visualObject.DOScale(new Vector3(0.6f, 1.4f, 0), duration).SetEase(Ease.InOutSine));
        seq.Join(_visualObject.DOLocalMove(new Vector3(-1f, 1.2f, 0), duration).SetEase(Ease.OutSine));

        // Settling
        seq.Append(_visualObject.DOScale(new Vector3(1.4f, 0.6f, 0), duration).SetEase(Ease.InOutSine));
        seq.Join(_visualObject.DORotate(originalRotation.eulerAngles, duration).SetEase(Ease.InOutSine));
        seq.Join(_visualObject.DOLocalMove(new Vector3(0, -0.49f), duration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            SetIdle();
        }));
    }
    void DOOno2()
    {
        float duration = _oneBeat / 2;

        DG.Tweening.Sequence seq = DOTween.Sequence();

        // Anticipation
        seq.Append(_visualObject.DOLocalMoveY(-0.4f, duration / 4).SetEase(Ease.InOutSine));
        seq.Join(_visualObject.DOScale(new Vector3(1.4f, 0.6f, 0), duration / 4));

        // Action
        seq.Append(_visualObject.DOLocalRotate(new Vector3(0, 0, -30f), duration).SetEase(Ease.OutSine));
        seq.Join(_visualObject.DOScale(new Vector3(0.6f, 1.4f, 0), duration).SetEase(Ease.InOutSine));
        seq.Join(_visualObject.DOLocalMove(new Vector3(1f, 1.2f, 0), duration).SetEase(Ease.OutSine));

        // Settling
        seq.Append(_visualObject.DOScale(new Vector3(1.4f, 0.6f, 0), duration).SetEase(Ease.InOutSine));
        seq.Join(_visualObject.DORotate(originalRotation.eulerAngles, duration).SetEase(Ease.InOutSine));
        seq.Join(_visualObject.DOLocalMove(new Vector3(0, -0.49f), duration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            SetIdle();
        }));
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

    // Coroutine
    IEnumerator WaitUntilNextBeat()
    {
        yield return new WaitUntil(() => _isOnBeat);

        DOIdle();
    }
}
