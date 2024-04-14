using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class BuyPhaseSequencer : ManagedBehaviour
{
    private int income = 3;
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

    private List<JurorInteractable> buyableJurors = new();
    private List<JurorCard> buyableJurorCards = new();

    public IEnumerator BuySequence()
    {
        endedPhase = false;

        var remainingPool = new List<GameObject>(jurorPrefabPool);
        for (int i = 0; i < 3; i++)
        {
            var random = remainingPool[Random.Range(0, remainingPool.Count)];
            remainingPool.Remove(random);
            SpawnJuror(random, i);
            yield return new WaitForSeconds(0.25f);
        }

        CashManager.instance.AdjustCash(income);

        yield return new WaitUntil(() => endedPhase);
    }

    private void SpawnJuror(GameObject jurorPrefab, int index)
    {
        var jurorObj = Instantiate(jurorPrefab, transform);
        Vector2 destination = leftAnchor + Vector2.right * jurorSpacing * index;
        jurorObj.transform.position = destination + Vector2.right * 10f;
        Tween.Position(jurorObj.transform, destination, 1f, 0f, Tween.EaseOutStrong);

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
        if (CashManager.instance.Cash >= juror.Data.cost)
        {
            int index = buyableJurors.IndexOf(juror);

            buyableJurors[index].gameObject.SetActive(false);
            buyableJurorCards[index].gameObject.SetActive(false);

            BenchArea.instance.SpawnJuror(juror.Prefab);

            Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.04f, 0.2f, 0f);

            CashManager.instance.AdjustCash(-juror.Data.cost);
        }
        else
        {
            Tween.Shake(cam.transform, new Vector3(0f, 0f, cam.transform.position.z), Vector2.one * 0.04f, 0.2f, 0f);
            CashManager.instance.BlinkCash();
        }
    }
}
