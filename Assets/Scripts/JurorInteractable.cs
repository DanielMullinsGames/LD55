using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class JurorInteractable : DraggableInteractable
{
    public JurorAnimController Anim => anim;
    [Title("Juror")]
    [SerializeField]
    private JurorAnimController anim = default;
}
