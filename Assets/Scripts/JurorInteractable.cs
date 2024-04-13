using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class JurorInteractable : DraggableInteractable
{
    public bool Benched { get; set; } = true; //<--

    [Title("Juror")]
    public JurorData Data => data;
    [SerializeField]
    private JurorData data = default;

    public JurorAnimController Anim => anim;
    [SerializeField]
    private JurorAnimController anim = default;

    private Disposition disposition = Disposition.Uncertain;

    protected override void ManagedInitialize()
    {
        disposition = data.disposition;
        anim.SetDispositionShown(false, immediate: true);
    }

    public override void ManagedUpdate()
    {
        base.ManagedUpdate();
        anim.SetDispositionShown(!BeingDragged && Benched);
        anim.ShowDispositionType(disposition);
    }
}
