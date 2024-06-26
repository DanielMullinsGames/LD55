using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Pixelplacement;

public class JurorAnimController : ManagedBehaviour
{
    [SerializeField]
    private DraggableInteractable draggable = default;

    [SerializeField]
    private Animator anim = default;

    [SerializeField]
    private AudioSource holdingAudioSource = default;

    public UnityEngine.Rendering.SortingGroup SortingGroup => sortingGroup;
    [SerializeField]
    private UnityEngine.Rendering.SortingGroup sortingGroup;

    [SerializeField]
    private GameObject leftHand = default;

    [SerializeField]
    private GameObject rightHand = default;

    [Title("Disposition Bubble")]
    [SerializeField]
    private Transform topBubble = default;

    [SerializeField]
    private Transform bottomBubble = default;

    [SerializeField]
    private SpriteRenderer topBubbleFill = default;

    [SerializeField]
    private SpriteRenderer bottomBubbleFill = default;

    [SerializeField]
    private SpriteRenderer bubbleLetter = default;

    [SerializeField]
    private Sprite innocentLetterSprite = default;

    [SerializeField]
    private Sprite guiltyLetterSprite = default;

    [SerializeField]
    private Sprite uncertainLetterSprite = default;

    [SerializeField]
    private Color guiltyFillColor = default;

    [SerializeField]
    private Color innocentFillColor = default;

    private bool dispositionShown = true;

    public override void ManagedUpdate()
    {
        anim.SetBool("dragging", draggable.BeingDragged);
        sortingGroup.sortingLayerName = draggable.BeingDragged ? "Cursor" : "Default";
        holdingAudioSource.enabled = draggable.BeingDragged;
    }

    public void SetHandsShown(bool shown)
    {
        leftHand.SetActive(shown);
        rightHand.SetActive(shown);
    }

    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

    public IEnumerator VoteAnimationSequence(Disposition vote, bool deliberate)
    {
        if (deliberate)
        {
            anim.SetTrigger("deliberate");
            yield return new WaitForSeconds(0.5f);
        }
        if (vote == Disposition.Guilty)
        {
            anim.SetTrigger("voteguilty");
        }
        else if (vote == Disposition.Innocent)
        {
            anim.SetTrigger("voteinnocent");
        }
        yield return new WaitForSeconds(0.25f);
    }

    public void SetDispositionShown(bool shown, bool immediate = false)
    {
        Vector2 scale = shown ? Vector2.one : Vector2.zero;

        if (immediate)
        {
            topBubble.transform.localScale = bottomBubble.transform.localScale = scale;
        }
        else if (dispositionShown != shown)
        {
            Tween.LocalScale(topBubble, scale, 0.1f, 0f, shown ? Tween.EaseOut : Tween.EaseIn);
            Tween.LocalScale(bottomBubble, scale, 0.1f, 0f, shown ? Tween.EaseOut : Tween.EaseIn);
        }
        dispositionShown = shown;
    }

    public void ShowDispositionType(Disposition disposition)
    {
        switch (disposition)
        {
            case Disposition.Guilty:
                bubbleLetter.sprite = guiltyLetterSprite;
                bubbleLetter.color = innocentFillColor;
                topBubbleFill.color = bottomBubbleFill.color = guiltyFillColor;
                break;
            case Disposition.Innocent:
                bubbleLetter.sprite = innocentLetterSprite;
                bubbleLetter.color = guiltyFillColor;
                topBubbleFill.color = bottomBubbleFill.color = innocentFillColor;
                break;
            case Disposition.Uncertain:
                bubbleLetter.sprite = uncertainLetterSprite;
                bubbleLetter.color = innocentFillColor;
                topBubbleFill.color = bottomBubbleFill.color = Color.gray;
                break;
        }
    }
}
