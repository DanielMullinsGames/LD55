using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class SellArea : ManagedBehaviour
{
    public static SellArea instance;

    public Bounds Bounds => GetComponent<BoxCollider2D>().bounds;

    [SerializeField]
    private Camera cam = default;

    [SerializeField]
    private TMPro.TextMeshPro sellHintText = default;

    private bool hasSalesman;

    protected override void ManagedInitialize()
    {
        instance = this;
    }

    public override void ManagedUpdate()
    {
        hasSalesman = false;
        if (BenchArea.instance.Jurors.Exists(x => x != null && !ReferenceEquals(x, null) && x.Data.special == SpecialTrait.ExtraSell))
        {
            hasSalesman = true;
        }
        sellHintText.text = "sell for $" + (hasSalesman ? 2 : 1).ToString();
    }

    public void SellJuror(JurorInteractable juror)
    {
        Destroy(juror.gameObject);
        CamShake();
        CashManager.instance.AdjustCash(hasSalesman ? 2 : 1);
        AudioController.Instance.PlaySound2D("juror_die", 0.5f);
        AudioController.Instance.PlaySound2D("negate_3");
    }

    private void CamShake()
    {
        Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.04f, 0.2f, 0f);
    }
}
