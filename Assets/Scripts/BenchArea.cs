using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenchArea : ManagedBehaviour
{
    public static BenchArea instance;
    public List<JurorInteractable> Jurors => jurorsOnBench;

    [SerializeField]
    private float maxWidth = default;

    [SerializeField]
    private float maxSpacing = default;

    [SerializeField]
    private List<JurorInteractable> jurorsOnBench = default;

    protected override void ManagedInitialize()
    {
        instance = this;
        UpdateJurorPositions(null, immediate: true);
    }

    public override void ManagedUpdate()
    {
        if (DraggableInteractable.CurrentDraggable is JurorInteractable draggingJuror)
        {
            UpdateJurorOrder(draggingJuror);
            UpdateJurorPositions(draggingJuror);
        }
        else
        {
            UpdateJurorPositions(null);
        }
    }

    private void UpdateJurorPositions(JurorInteractable currentDraggingJuror, bool immediate = false)
    {
        float spacing = maxWidth / jurorsOnBench.Count;
        spacing = Mathf.Min(spacing, maxSpacing);

        float leftAnchor = (jurorsOnBench.Count * -spacing * 0.5f) + (spacing * 0.5f);
        foreach (var juror in jurorsOnBench)
        {
            if (juror != currentDraggingJuror)
            {
                int index = jurorsOnBench.IndexOf(juror);
                float xPos = leftAnchor + (spacing * index);
                juror.transform.localPosition = Vector2.Lerp(juror.transform.localPosition, new Vector2(xPos, 0f),
                    immediate ? 1f : Time.deltaTime * 10f);

                int sortingOrder = jurorsOnBench.Count - index;
                juror.Anim.SortingGroup.sortingOrder = sortingOrder;

                juror.SortOrderAdjustment = sortingOrder;
            }
        }
    }

    private void UpdateJurorOrder(JurorInteractable currentDraggingJuror)
    {
        if (currentDraggingJuror != null)
        {
            jurorsOnBench.Remove(currentDraggingJuror);

            int newIndex = 0;
            for (int i = 0; i < jurorsOnBench.Count; i++)
            {
                if (currentDraggingJuror.transform.position.x > jurorsOnBench[i].transform.position.x &&
                    (i + 1 >= jurorsOnBench.Count || jurorsOnBench[i + 1].transform.position.x > currentDraggingJuror.transform.position.x))
                {
                    newIndex = i + 1;
                }
            }

            jurorsOnBench.Insert(newIndex, currentDraggingJuror);
        }
    }
}
