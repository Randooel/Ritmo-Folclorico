using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    Conductor _conductor;

    [SerializeField][Range(0, 9)] int minDecimal, maxDecimal;

    [SerializeField] bool _isBeatActivated;

    private int _lastDecimal = -1;


    void Start()
    {
        _conductor = GetComponent<Conductor>();
    }

    void Update()
    {
        if (!_isBeatActivated)
        {
            ActivateBeatEvent();
        }
    }

    public void ActivateBeatEvent()
    {
        float beatPosition = _conductor.songPositionInBeats;
        int firstDecimal = Mathf.FloorToInt((beatPosition % 1) * 10);

        if (firstDecimal <= minDecimal || firstDecimal >= maxDecimal)
        {
            Debug.LogWarning("Valid beat");
            _isBeatActivated = true;

            RhythmEvent.onBeat?.Invoke();
            DOVirtual.DelayedCall(_conductor.SecPerBeat, () =>
            {
                _isBeatActivated = false;
            });
        }
    }
    IEnumerator WaitUntilNextBeat()
    {
        yield return new WaitForSeconds(_conductor.SecPerBeat);

        _isBeatActivated = false;
    }
}