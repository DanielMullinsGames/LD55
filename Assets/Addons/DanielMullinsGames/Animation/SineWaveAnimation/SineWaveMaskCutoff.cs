using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineWaveMaskCutoff : ManagedBehaviour
{
    [SerializeField]
    private SpriteMask mask = default;

    [SerializeField]
    private float speed = default;

    [SerializeField]
    private float minValue = default;

    [SerializeField]
    private float maxValue = default;

    public override void ManagedUpdate()
    {
        float sine = (Mathf.Sin(Time.time * speed) * 0.5f) + 0.5f;
        mask.alphaCutoff = Mathf.Lerp(minValue, maxValue, sine);
    }
}
