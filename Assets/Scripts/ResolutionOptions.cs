using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionOptions : ManagedBehaviour
{
    private static bool pressedF;

    public GameObject hint = default;

    public override void ManagedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            pressedF = true;
            Screen.fullScreen = !Screen.fullScreen;
        }
        hint.gameObject.SetActive(!pressedF);
    }
}
