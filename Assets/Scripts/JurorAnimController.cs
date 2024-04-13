using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JurorAnimController : ManagedBehaviour
{
    [SerializeField]
    private DraggableInteractable draggable = default;

    [SerializeField]
    private Animator anim = default;

    public UnityEngine.Rendering.SortingGroup SortingGroup => sortingGroup;
    [SerializeField]
    private UnityEngine.Rendering.SortingGroup sortingGroup;

    public override void ManagedUpdate()
    {
        anim.SetBool("dragging", draggable.BeingDragged);
        sortingGroup.sortingLayerName = draggable.BeingDragged ? "Cursor" : "Default";
    }
}
