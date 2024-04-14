using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private List<JurorInteractable> buyableJurors = new();
    private List<JurorCard> buyableJurorCards = new();

    public IEnumerator BuySequence()
    {
        endedPhase = false;
        for (int i = 0; i < 3; i++)
        {
            SpawnJuror(jurorPrefabPool[0], i);//<--
        }

        yield return new WaitUntil(() => endedPhase);
    }

    private void SpawnJuror(GameObject jurorPrefab, int index)
    {
        var jurorObj = Instantiate(jurorPrefab, transform);
        jurorObj.transform.position = leftAnchor + Vector2.right * jurorSpacing * index;
        var juror = jurorObj.GetComponent<JurorInteractable>();
        juror.ConfigureForBuyPhase(OnJurorBuyButtonPressed);

        var cardObj = Instantiate(jurorCardPrefab, transform);
        cardObj.transform.position = new Vector2(jurorObj.transform.position.x, cardsY);
        var card = cardObj.GetComponent<JurorCard>();
        card.juror = juror;

        buyableJurors.Add(juror);
        buyableJurorCards.Add(card);
    }

    private void OnJurorBuyButtonPressed(JurorInteractable juror)
    {
        int index = buyableJurors.IndexOf(juror);

        buyableJurors[index].gameObject.SetActive(false);
        buyableJurorCards[index].gameObject.SetActive(false);
    }
}
