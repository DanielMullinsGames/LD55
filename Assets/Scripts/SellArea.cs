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

    protected override void ManagedInitialize()
    {
        instance = this;
    }

    public void SellJuror(JurorInteractable juror)
    {
        Destroy(juror.gameObject);
        CamShake();
        CashManager.instance.AdjustCash(1);
        AudioController.Instance.PlaySound2D("juror_die", 0.5f);
        AudioController.Instance.PlaySound2D("negate_3");
    }

    private void CamShake()
    {
        Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.04f, 0.2f, 0f);
    }
}
