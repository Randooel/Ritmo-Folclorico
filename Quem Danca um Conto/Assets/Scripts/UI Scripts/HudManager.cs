using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    private Conductor conductor;
    private PlayerRhythm playerRhythm;

    [SerializeField] private Image rhythmRing;

    [Header("Color Settings")]
    public Color defaultColor = Color.white;
    public Color correctColor = Color.green;
    public Color perfectColor = Color.yellow;
    public Color wrongColor = Color.red;
    [SerializeField] private bool isHighlighting;

    [Header("Opacity Setting")]
    [SerializeField][Range(0f, 1f)] private float defaultAlpha = 1f;
    [SerializeField] [Range(0f, 1f)] private float targetAlpha = 0.5f;

    void Start()
    {
        conductor = FindObjectOfType<Conductor>();
        playerRhythm = FindObjectOfType<PlayerRhythm>();
    }

    void Update()
    {
        ChangeAlphaOnBeat();
    }

    public void ChangeAlphaOnBeat()
    {
        float songPosition = conductor.songPositionInBeats;
        float firstDecimal = Mathf.Floor((songPosition % 1f) * 10);

        if (firstDecimal >= 9 || firstDecimal < 1f)
        {
            ChangeAlpha(targetAlpha);
        }
        else if (firstDecimal < 0.9f || firstDecimal > 1.1f)
        {
            ChangeAlpha(defaultAlpha);
        }
    }

    public void ChangeAlpha(float alpha)
    {
        Color color = rhythmRing.color;
        color.a = alpha;
        rhythmRing.color = color;
    }

    public void ChangeColorOnHit()
    {
        if(playerRhythm.hitCorrect)
        {
            rhythmRing.color = wrongColor;
        }

        if (playerRhythm.hitPerfect)
        {
            rhythmRing.color = perfectColor;
            isHighlighting = true;
        }
    }
}