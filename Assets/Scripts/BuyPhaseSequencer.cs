using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class BuyPhaseSequencer : ManagedBehaviour
{
    private bool endedPhase = false;

    [SerializeField]
    private List<GameObject> jurorPrefabPool = default;

    [SerializeField]
    private GameObject jurorCardPrefab = default;

    [SerializeField]
    private Vector2 leftAnchor = default;

    [SerializeField]
    private float jurorSpacing = default;

    [SerializeField]
    private float cardsY = default;

    [SerializeField]
    private Camera cam = default;

    [SerializeField]
    private Transform uiParent = default;

    [SerializeField]
    private Interactable rerollButton = default;

    [SerializeField]
    private Interactable endPhaseButton = default;

    [SerializeField]
    private TMPro.TextMeshPro trialDetailText = default;

    [SerializeField]
    private GameObject fullBenchText = default;

    private List<JurorInteractable> buyableJurors = new();
    private List<JurorCard> buyableJurorCards = new();

    private readonly Vector2 OFFSCREEN_UI_POS = new Vector2(0f, -10f);

    private void Start()
    {
        endPhaseButton.CursorSelectStarted += (Interactable i) => endedPhase = true;
        rerollButton.CursorSelectStarted += (Interactable i) => OnRerollPressed();
        uiParent.transform.localPosition = OFFSCREEN_UI_POS;
        endPhaseButton.gameObject.SetActive(false);
    }

    public IEnumerator BuySequence(int guiltNeeded, string trialName)
    {
        trialDetailText.text = trialName + "\n" + guiltNeeded + " guilty votes needed";

        endedPhase = false;
        Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.04f, 1.5f, 0f);
        Tween.LocalPosition(uiParent, Vector2.zero, 1.5f, 0f, Tween.EaseOutStrong);
        AudioController.Instance.PlaySound2D("weird_power");
        yield return new WaitForSeconds(1.5f);
        endPhaseButton.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);

        yield return SpawnJurors();

        yield return new WaitUntil(() => endedPhase);
        CamShake();
        AudioController.Instance.PlaySound2D("horn_1");
        yield return new WaitForSeconds(0.1f);
        endPhaseButton.gameObject.SetActive(false);
        yield return ClearJurors();
        yield return new WaitForSeconds(0.2f);
        Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.04f, 1.5f, 0f);
        Tween.LocalPosition(uiParent, OFFSCREEN_UI_POS, 1.5f, 0f, Tween.EaseIn);
        AudioController.Instance.PlaySound2D("weird_power", pitch: new AudioParams.Pitch(0.9f));
        yield return new WaitForSeconds(1.35f);
    }

    private IEnumerator SpawnJurors()
    {
        var remainingPool = new List<GameObject>(jurorPrefabPool);
        for (int i = 0; i < 3; i++)
        {
            var random = remainingPool[Random.Range(0, remainingPool.Count)];
            remainingPool.Remove(random);
            SpawnJuror(random, i);
            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator ClearJurors()
    {
        foreach (var juror in buyableJurors)
        {
            if (juror != null)
            {
                Tween.Position(juror.transform, juror.transform.position + Vector3.left * 10f, 0.5f, 0f, Tween.EaseIn);
                AudioController.Instance.PlaySound2D("whoosh", 
                    pitch: new AudioParams.Pitch(AudioParams.Pitch.Variation.Small), repetition: new AudioParams.Repetition(0.05f));
                Destroy(juror.gameObject, 0.5f);
                yield return new WaitForSeconds(0.05f);
            }
        }
        buyableJurors.Clear();
        buyableJurorCards.Clear();
    }

    private void OnRerollPressed()
    {
        AudioController.Instance.PlaySound2D("negate_4");
        if (CashManager.instance.Cash >= 1)
        {
            StartCoroutine(RerollSequence());
            CamShake();
            CashManager.instance.AdjustCash(-1);
        }
        else
        {
            CamShake();
            CashManager.instance.BlinkCash();
        }
    }
    
    private IEnumerator RerollSequence()
    {
        rerollButton.SetCollisionEnabled(false);
        endPhaseButton.SetCollisionEnabled(false);
        yield return ClearJurors();
        yield return new WaitForSeconds(0.2f);
        yield return SpawnJurors();
        rerollButton.SetCollisionEnabled(true);
        endPhaseButton.SetCollisionEnabled(true);
    }

    private void SpawnJuror(GameObject jurorPrefab, int index)
    {
        var jurorObj = Instantiate(jurorPrefab, transform);
        Vector2 destination = leftAnchor + Vector2.right * jurorSpacing * index;
        jurorObj.transform.position = destination + Vector2.right * 10f;
        Tween.Position(jurorObj.transform, destination, 1f, 0f, Tween.EaseOutStrong);
        AudioController.Instance.PlaySound2D("whoosh", pitch: new AudioParams.Pitch(AudioParams.Pitch.Variation.Small));

        var juror = jurorObj.GetComponent<JurorInteractable>();
        juror.ConfigureForBuyPhase(OnJurorBuyButtonPressed);
        juror.Prefab = jurorPrefab;

        var cardObj = Instantiate(jurorCardPrefab, transform);
        cardObj.transform.position = new Vector2(100f, cardsY);
        var card = cardObj.GetComponent<JurorCard>();
        card.juror = juror;

        buyableJurors.Add(juror);
        buyableJurorCards.Add(card);
    }

    private void OnJurorBuyButtonPressed(JurorInteractable juror)
    {
        if (BenchArea.instance.Jurors.Count >= 7)
        {
            AudioController.Instance.PlaySound2D("negate_1");
            CamShake();
            CustomCoroutine.FlickerSequence(() => fullBenchText.SetActive(true), () => fullBenchText.SetActive(false), true, false, 0.1f, 3);
        }
        else if (CashManager.instance.Cash >= juror.Data.cost)
        {
            AudioController.Instance.PlaySound2D("negate_4");
            int index = buyableJurors.IndexOf(juror);

            buyableJurors[index].gameObject.SetActive(false);
            buyableJurorCards[index].gameObject.SetActive(false);

            BenchArea.instance.SpawnJuror(juror.Prefab);

            CamShake();
            CashManager.instance.AdjustCash(-juror.Data.cost);
        }
        else
        {
            AudioController.Instance.PlaySound2D("negate_1");
            CamShake();
            CashManager.instance.BlinkCash();
        }
    }

    private void CamShake()
    {
        Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.04f, 0.2f, 0f);
    }
}
