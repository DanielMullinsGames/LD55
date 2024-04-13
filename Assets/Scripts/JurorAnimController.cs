using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JurorAnimController : ManagedBehaviour
{
    [SerializeField]
    private DraggableInteractable draggable = default;

    [SerializeField]
    private Animator anim = default;

    public override void ManagedUpdate()
    {
        anim.SetBool("dragging", draggable.BeingDragged);
    }
}
