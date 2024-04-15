using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkGameObjectActive : TimedBehaviour
{
    [Header("Blink")]
    [SerializeField]
    private GameObject objectToBlink = default;

    [SerializeField]
    private string blinkSound = default;

    protected override void OnTimerReached()
    {
        objectToBlink.SetActive(!objectToBlink.activeSelf);

        if (objectToBlink.activeSelf && !string.IsNullOrEmpty(blinkSound))
        {
            AudioController.Instance.PlaySound2D(blinkSound, volume: 0.25f);
        }
    }
}
