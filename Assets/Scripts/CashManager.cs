using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class CashManager : ManagedBehaviour
{
    public static CashManager instance;
    public int Cash { get; private set; }

    [SerializeField]
    private TMPro.TextMeshPro cashText = default;

    [SerializeField]
    private Transform animParent = default;

    protected override void ManagedInitialize()
    {
        instance = this;
    }

    public void AdjustCash(int amount)
    {
        Cash = Mathf.Max(0, Cash + amount);

        Tween.LocalScale(animParent, Vector2.one * 0.75f, 0.1f, 0f, Tween.EaseInOut, completeCallback: () =>
        {
            cashText.text = "x " + amount.ToString();
            Tween.LocalScale(animParent, Vector2.one, 0.1f, 0f, Tween.EaseInOut);
        });
    }
}
