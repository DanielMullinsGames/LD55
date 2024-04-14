using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : ManagedBehaviour
{
    [SerializeField]
    private TrialSequencer trialSequencer = default;

    [SerializeField]
    private BuyPhaseSequencer buyPhaseSequencer = default;

    private void Start()
    {
        StartCoroutine(GameSequence());
    }

    private IEnumerator GameSequence()
    {
        // foreach defendant
            // TODO: defendant intro sequence
            yield return DefendantSequence();
            // TODO: outro sequence
    }

    private IEnumerator DefendantSequence()
    {
        // foreach trial vs defendant
        yield return buyPhaseSequencer.BuySequence();
        bool trialSucceeded = false;
        yield return trialSequencer.TrialSequence(3, (bool succeeded) => trialSucceeded = succeeded);
    }
}
