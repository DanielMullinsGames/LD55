using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vote : ManagedBehaviour
{
    [SerializeField]
    private SpriteRenderer fillRenderer = default;

    [SerializeField]
    private SpriteRenderer letterRenderer = default;

    [SerializeField]
    private Sprite guiltyLetterSprite = default;

    [SerializeField]
    private Sprite innocentLetterSprite = default;

    [SerializeField]
    private Color guiltyFillColor = default;

    [SerializeField]
    private Color innocentFillColor = default;

    [SerializeField]
    private Color guiltyLetterColor = default;

    [SerializeField]
    private Color innocentLetterColor = default;

    public void Initialize(bool guilty)
    {
        letterRenderer.sprite = guilty ? guiltyLetterSprite : innocentLetterSprite;
        letterRenderer.color = guilty ? guiltyLetterColor : innocentLetterColor;
        fillRenderer.color = guilty ? guiltyFillColor : innocentFillColor;
    }
}
