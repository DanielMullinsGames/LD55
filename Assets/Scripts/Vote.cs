using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class Vote : ManagedBehaviour
{
    public Disposition Outcome { get; private set; }

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

    [SerializeField]
    private GameObject crumpledPrefab = default;

    public void Initialize(bool guilty)
    {
        Outcome = guilty ? Disposition.Guilty : Disposition.Innocent;
        letterRenderer.sprite = guilty ? guiltyLetterSprite : innocentLetterSprite;
        letterRenderer.color = guilty ? guiltyLetterColor : innocentLetterColor;
        fillRenderer.color = guilty ? guiltyFillColor : innocentFillColor;
    }

    public void ChangeVote(bool guilty)
    {
        AudioController.Instance.PlaySound2D("paper_short", 1f, pitch: new AudioParams.Pitch(AudioParams.Pitch.Variation.VerySmall), repetition: new AudioParams.Repetition(0.05f));
        Tween.LocalScale(transform, new Vector2(0.25f, 1.25f), 0.1f, 0f, Tween.EaseIn, completeCallback: () =>
        {
            Initialize(guilty);
            Tween.LocalScale(transform, new Vector2(1.25f, 1.25f), 0.1f, 0f, Tween.EaseOut);
        });
        
    }

    public void Cleanup()
    {
        AudioController.Instance.PlaySound2D("vote_crunch", 0.8f,
            pitch: new AudioParams.Pitch(AudioParams.Pitch.Variation.VerySmall), repetition: new AudioParams.Repetition(0.05f));

        var crumpled = Instantiate(crumpledPrefab).transform;
        crumpled.position = transform.position;
        crumpled.eulerAngles = new Vector3(0f, 0f, Random.value * 360f);
        Tween.Shake(crumpled, crumpled.transform.position, Vector2.one * 0.02f, 0.1f + Random.value * 0.05f, 0f, completeCallback: () =>
        {
            Tween.Position(crumpled, crumpled.transform.position + Vector3.down * 10f, 0.5f + Random.value * 0.1f, 0f, Tween.EaseInStrong, completeCallback: () =>
            {
                Destroy(crumpled.gameObject);
            });
        });

        Destroy(gameObject);
    }
}
