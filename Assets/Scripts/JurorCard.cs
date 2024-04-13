using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JurorCard : ManagedBehaviour
{
    public JurorInteractable juror = default;

    [SerializeField]
    private float xExtent = default;

    [SerializeField]
    private UnityEngine.Rendering.SortingGroup sortingGroup;

    [SerializeField]
    private TMPro.TextMeshPro titleText = default;

    [SerializeField]
    private TMPro.TextMeshPro detailText = default;

    public override void ManagedUpdate()
    {
        transform.position = new Vector2(juror.transform.position.x, transform.position.y);
        transform.localPosition = new Vector2(Mathf.Clamp(transform.localPosition.x, -xExtent, xExtent), transform.localPosition.y);

        int sortOrderAdjust = juror.BeingDragged ? 10 : 0;
        sortingGroup.sortingOrder = 2 + sortOrderAdjust;
        titleText.sortingOrder = 5 + sortOrderAdjust;
        detailText.sortingOrder = 3 + sortOrderAdjust;
    }
}
