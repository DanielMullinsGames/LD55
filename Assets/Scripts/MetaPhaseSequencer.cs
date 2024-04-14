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

    private void Start()
    {
        transform.position = new Vector2(0f, 10f);
    }

    public IEnumerator MetaPhase(int index, int[] guiltReqs, string[] trialNames)
    {
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
        yield return new WaitForSeconds(1f);

        if (index > 0)
        {
            yield return new WaitForSeconds(0.5f);
            fills[index - 1].gameObject.SetActive(true);
            Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.04f, 0.2f, 0f);
        }
        blinkers[index].enabled = true;

        yield return new WaitForSeconds(3f);
        blinkers[index].enabled = false;
        trialTexts[index].gameObject.SetActive(true);
        Tween.Position(transform, new Vector2(0f, 10f), 0.5f, 0f, Tween.EaseIn);
        yield return new WaitForSeconds(0.5f);
    }
}
