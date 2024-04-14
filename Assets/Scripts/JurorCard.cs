using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class JurorCard : ManagedBehaviour
{
    public JurorInteractable juror = default;

    [SerializeField]
    private bool isBuyPhase = default;

    [SerializeField]
    private float xExtent = default;

    [SerializeField]
    private UnityEngine.Rendering.SortingGroup sortingGroup;

    [SerializeField]
    private TMPro.TextMeshPro titleText = default;

    [SerializeField]
    private TMPro.TextMeshPro detailText = default;

    [SerializeField]
    private Transform actionBadge = default;

    private void Start()
    {
        DisplayData(juror.Data);
        juror.Card = this;
    }

    public void DisplayData(JurorData data)
    {
        titleText.text = data.nameText;
        string fullDetails = "";
        if (isBuyPhase)
        {
            if (data.disposition == Disposition.Guilty || data.disposition == Disposition.Innocent)
            {
                fullDetails += "Default: " + data.disposition.ToString() + "\n\n";
            }
        }
        fullDetails += data.detailText;
        detailText.text = fullDetails;
        actionBadge.gameObject.SetActive(data.afterVoteAction != AfterVoteAction.None && !isBuyPhase);
    }

    public void SetActionBadgePulsing(bool pulsing)
    {
        if (pulsing)
        {
            Tween.LocalScale(actionBadge, Vector2.one * 1.5f, 0.1f, 0f, Tween.EaseInOut, Tween.LoopType.PingPong);
        }
        else
        {
            Tween.Cancel(actionBadge.GetInstanceID());
            Tween.LocalScale(actionBadge, Vector2.one, 0.1f, 0, Tween.EaseInOut);
        }
    }

    public override void ManagedUpdate()
    {
        if (juror == null || ReferenceEquals(juror, null))
        {
            Destroy(gameObject);
            return;
        }

        transform.position = new Vector2(juror.transform.position.x, transform.position.y);
        transform.localPosition = new Vector2(Mathf.Clamp(transform.localPosition.x, -xExtent, xExtent), transform.localPosition.y);

        int sortOrderAdjust = juror.BeingDragged ? 10 : 0;
        sortingGroup.sortingOrder = 2 + sortOrderAdjust;
        titleText.sortingOrder = 5 + sortOrderAdjust;
        detailText.sortingOrder = 3 + sortOrderAdjust;
    }
}
