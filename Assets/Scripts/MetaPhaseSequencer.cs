using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class MetaPhaseSequencer : ManagedBehaviour
{
    [SerializeField]
    private Camera cam = default;

    [SerializeField]
    private List<TMPro.TextMeshPro> trialTexts = default;

    [SerializeField]
    private List<TMPro.TextMeshPro> guiltTexts = default;

    [SerializeField]
    private List<GameObject> fills = default;

    [SerializeField]
    private List<BlinkGameObjectActive> blinkers = default;

    [SerializeField]
    private TMPro.TextMeshPro basePaymentText = default;

    [SerializeField]
    private TMPro.TextMeshPro bonusPaymentText = default;

    [SerializeField]
    private TMPro.TextMeshPro defendantPowerText = default;

    [SerializeField]
    private GameObject mickeyHorsePrefab = default;

    [SerializeField]
    private Transform crossHairs = default;

    private void Start()
    {
        transform.position = new Vector2(0f, 10f);
    }

    public IEnumerator MetaPhase(int index, int[] guiltReqs, string[] trialNames, bool won)
    {
        basePaymentText.gameObject.SetActive(false);
        bonusPaymentText.gameObject.SetActive(false);
        for (int i = 0; i < trialTexts.Count; i++)
        {
            trialTexts[i].text = trialNames[i];
            guiltTexts[i].text = guiltReqs[i].ToString();
        }

        for (int i = 0; i < index - 1; i++)
        {
            fills[i].gameObject.SetActive(true);
        }

        transform.position = new Vector2(0f, 10f);
        Tween.Position(transform, new Vector2(0f, 2f), 1f, 0f, Tween.EaseOutStrong);
        AudioController.Instance.PlaySound2D("whoosh", pitch: new AudioParams.Pitch(0.8f));
        yield return new WaitForSeconds(1f);

        if (index > 0)
        {
            yield return new WaitForSeconds(0.5f);
            fills[index - 1].gameObject.SetActive(true);
            CamShake();
        }
        blinkers[index].enabled = true;

        yield return new WaitForSeconds(1f);
        if (index != 0)
        {
            CamShake();
            basePaymentText.gameObject.SetActive(true);
            CashManager.instance.AdjustCash(3);

            if (won)
            {
                yield return new WaitForSeconds(1f);
                CamShake();
                bonusPaymentText.gameObject.SetActive(true);
                CashManager.instance.AdjustCash(1);
            }
        }

        yield return new WaitForSeconds(2f);
        blinkers[index].enabled = false;
        trialTexts[index].gameObject.SetActive(true);
        AudioController.Instance.PlaySound2D("metal_tap", 0.5f);
        Tween.Position(transform, new Vector2(0f, 10f), 0.5f, 0f, Tween.EaseIn);
        yield return new WaitForSeconds(0.5f);
        yield return JurorPowerSequence(index);
    }

    private IEnumerator JurorPowerSequence(int roundIndex)
    {
        // Booth
        if (GameFlowManager.defendantIndex == 1)
        {
            if ((roundIndex == 2 || roundIndex == 4) && BenchArea.instance.Jurors.Count > 0)
            {
                defendantPowerText.gameObject.SetActive(true);
                defendantPowerText.text = "DEFENDANT POWER: ASSASSINATION!";
                AudioController.Instance.PlaySound2D("negate_1");
                yield return new WaitForSeconds(1.5f);

                var target = BenchArea.instance.Jurors[0];
                crossHairs.gameObject.SetActive(true);
                crossHairs.transform.position = new Vector3(10f, 10f);
                Tween.Position(crossHairs, target.transform.position + Vector3.up * 0.5f, 2f, 0f, Tween.EaseOutStrong);
                yield return new WaitForSeconds(3f);
                AudioController.Instance.PlaySound2D("explode");
                AudioController.Instance.PlaySound2D("juror_die", 0.5f);
                Destroy(target.gameObject);
                crossHairs.gameObject.SetActive(false);
                CamShake();
                yield return new WaitForSeconds(1.5f);
                defendantPowerText.gameObject.SetActive(false);
            }
        }
        // Mickey
        if (GameFlowManager.defendantIndex == 2)
        {
            if (roundIndex > 0 && BenchArea.instance.Jurors.Count < 7)
            {
                defendantPowerText.gameObject.SetActive(true);
                defendantPowerText.text = "DEFENDANT POWER: ALL ABOARD!";
                AudioController.Instance.PlaySound2D("negate_1");
                yield return new WaitForSeconds(1.5f);

                AudioController.Instance.PlaySound2D("negate_1");
                CamShake();
                BenchArea.instance.SpawnJuror(mickeyHorsePrefab);
                yield return new WaitForSeconds(1.5f);
                defendantPowerText.gameObject.SetActive(false);
            }
        }
    }

    private void CamShake()
    {
        AudioController.Instance.PlaySound2D("negate_4");
        Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.04f, 0.2f, 0f);
    }
}
