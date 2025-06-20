using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HudManager : MonoBehaviour //, IDanceable
{
    private Conductor _conductor;
    private PlayerRhythm _playerRhythm;
    private PlayerCommands _playerCommands;

    private Coroutine _resetColorCoroutine;

    [SerializeField] int minDecimal, maxDecimal;

    [SerializeField] private Image _rhythmRing;

    [Header("Color Settings")]
    public Color defaultColor = Color.white;
    public Color correctColor = Color.green;
    public Color perfectColor = Color.yellow;
    public Color wrongColor = Color.red;
    private Color _baseColor;

    [Header("Opacity Setting")]
    [SerializeField][Range(0f, 1f)] private float _defaultAlpha = 0.25f;
    [SerializeField][Range(0f, 1f)] private float _targetAlpha = 0.5f;
    //[SerializeField][Range(0f, 1f)] private float _highlightAlpha = 1f;
    void Start()
    {
        _conductor = FindObjectOfType<Conductor>();
        _playerRhythm = FindObjectOfType<PlayerRhythm>();
        _playerCommands = FindObjectOfType<PlayerCommands>();

        _playerRhythm.OnMouseClick += ChangeColorOnHit;

        _baseColor = defaultColor;
        _rhythmRing.color = _baseColor;
    }

    void Update()
    {
        // ChangeAlphaOnBeat();
    }

    void OnEnable()
    {
        RhythmEvent.onBeat += OnBeat;
    }

    void OnDisable()
    {
        RhythmEvent.onBeat -= OnBeat;
    }

    public void OnBeat()
    {
        //Debug.Log("HUD ON BEAT!!!");

        ChangeAlpha(_targetAlpha);

        DOVirtual.DelayedCall(_conductor.SecPerBeat / 2, () =>
        {
            ChangeAlpha(_defaultAlpha);
        });
    }


    /*
    public void ChangeAlphaOnBeat()
    {
        float beatsPosition = _conductor.songPositionInBeats;
        float firstDecimal = Mathf.Floor((beatsPosition % 1f) * 10);

        
        if (firstDecimal >= minDecimal || firstDecimal <= maxDecimal)
        {
            ChangeAlpha(_targetAlpha);
        }
        else 
        {
            ChangeAlpha(_defaultAlpha);
        }
    }
    */

    public void ChangeAlpha(float alpha)
    {
        Color color = _rhythmRing.color;
        color.a = alpha;
        _rhythmRing.color = color;
    }

    public void ChangeColorOnHit(string HitType)
    {
        switch(HitType)
        {
            case "Wrong":
                _baseColor = wrongColor;
                break;
            case "Correct":
                _baseColor = correctColor;
                break;
            case "Perfect":
                _baseColor = perfectColor;
                break;
        }

        Color newColor = _baseColor;
        newColor.a = _rhythmRing.color.a;
        _rhythmRing.color = newColor;

        if (_resetColorCoroutine != null)
        {
            StopCoroutine(_resetColorCoroutine);
        }
        _resetColorCoroutine = StartCoroutine(ResetColor());
    }

    IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(_playerRhythm.boolWaitTime);
        _baseColor = defaultColor;
        _rhythmRing.color = _baseColor;
    }
}