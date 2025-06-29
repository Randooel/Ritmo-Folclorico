using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HudManager : MonoBehaviour //, IDanceable
{
    [SerializeField] bool isRhythmRing;

    private Conductor _conductor;
    private PlayerRhythm _playerRhythm;
    private PlayerCommands _playerCommands;

    private Coroutine _resetColorCoroutine;

    [SerializeField] int minDecimal, maxDecimal;

    [SerializeField] private Image _uiImage, _listImage;
    [SerializeField] private Transform _uiTransform, _listTransform;

    const string doWrongID = "wrong";
    const string doCorrectID = "correct";
    const string doPerfectID = "perfect";

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
        _uiImage.color = _baseColor;
        _listImage.color = _baseColor;


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

        DOTilt();

        DOVirtual.DelayedCall(_conductor.SecPerBeat / 2, () =>
        {
            ChangeAlpha(_defaultAlpha);
        });
    }

    public void ChangeAlpha(float alpha)
    {
        Color color = _uiImage.color;
        Color color2 = _listImage.color;

        color.a = alpha;
        color2.a = alpha;

        _uiImage.color = color;
        _listImage.color = color2;
    }

    public void ChangeColorOnHit(string HitType)
    {
        switch(HitType)
        {
            case "Wrong":
                _baseColor = wrongColor;
                DOWrong();
                break;
            case "Correct":
                _baseColor = correctColor;
                DOCorrect();
                break;
            case "Perfect":
                _baseColor = perfectColor;
                DOPerfect();
                break;
        }

        Color newUIColor = _baseColor;
        Color newListColor = _baseColor;
        newUIColor.a = _uiImage.color.a;
        newListColor.a = _listImage.color.a;
        _uiImage.color = newUIColor;
        _listImage.color = newListColor;


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
        _uiImage.color = _baseColor;
        _listImage.color = _baseColor;
    }

    void DOTilt()
    {
        _listTransform.transform.DOScale(new Vector3(1.05f, 1.05f, 0),
            _conductor.SecPerBeat / 3).SetLoops(2, LoopType.Yoyo);
    }

    void DOWrong()
    {
        DOKillHitAnim();

        float duration = _conductor.SecPerBeat / 3;
        float strength = 0.08f;
        int vibrato = 10;
        float randomness = 1;
        bool fadeOut = false;

        _listTransform.transform.DOShakeScale(duration, strength, vibrato, randomness,
            fadeOut);
        _listTransform.transform.DOShakePosition(duration, strength, vibrato, randomness,
            fadeOut);
        _listTransform.transform.DOShakeRotation(duration, strength, vibrato, randomness,
            fadeOut);

        _uiTransform.transform.DOShakeScale(duration, strength, vibrato, randomness,
            fadeOut);
        _uiTransform.transform.DOShakePosition(duration, strength, vibrato, randomness,
            fadeOut);
        _uiTransform.transform.DOShakeRotation(duration, strength, vibrato, randomness,
            fadeOut);
    }

    void DOCorrect()
    {
        DOKillHitAnim();

        _uiTransform.DOScale(new Vector3(1.01f, 1.01f, 0),
            _conductor.SecPerBeat / 3).SetLoops(2, LoopType.Yoyo);
        _listTransform.DOScale(new Vector3(1.02f, 1.02f, 0),
            _conductor.SecPerBeat / 3).SetLoops(2, LoopType.Yoyo);
    }

    void DOPerfect()
    {
        DOKillHitAnim();

        _uiTransform.transform.DOScale(new Vector3(1.02f, 1.02f, 0),
             _conductor.SecPerBeat / 3).SetLoops(2, LoopType.Yoyo);
        _listTransform.DOScale(new Vector3(1.02f, 1.02f, 0),
            _conductor.SecPerBeat / 3).SetLoops(2, LoopType.Yoyo);
    }

    void DOKillHitAnim()
    {
        DOTween.Kill(transform);
    }
}