using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastForwardButton : ManagedBehaviour
{
    [SerializeField]
    private GameObject hint = default;

    private static bool hintHidden = false;
    float holdTimer = 0f;

    public override void ManagedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            holdTimer += Time.deltaTime;
            Time.timeScale = 2.5f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        if (holdTimer > 2.5f)
        {
            hintHidden = true;
        }

        hint.gameObject.SetActive(!hintHidden);
    }
}
