using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class GameFlowManager : ManagedBehaviour
{
    [SerializeField]
    private TrialSequencer trialSequencer = default;

    [SerializeField]
    private BuyPhaseSequencer buyPhaseSequencer = default;

    [SerializeField]
    private Transform benchParent = default;

    [SerializeField]
    private BenchArea benchArea = default;

    [SerializeField]
    private Vector2 buyPhaseBenchPos = default;

    [SerializeField]
    private Vector2 trialPhaseBenchPos = default;

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
        int numRounds = 5;
        for (int i = 0; i < numRounds; i++)
        {
            Tween.Position(benchParent, buyPhaseBenchPos, 0.25f, 0f, Tween.EaseInOut);
            yield return new WaitForSeconds(0.35f);
            yield return buyPhaseSequencer.BuySequence();
            yield return new WaitForSeconds(0.1f);

            bool trialSucceeded = false;
            Tween.Position(benchParent, trialPhaseBenchPos, 0.25f, 0f, Tween.EaseInOut);
            yield return new WaitForSeconds(0.35f);
            yield return trialSequencer.TrialSequence(3, (bool succeeded) => trialSucceeded = succeeded);
        }
    }
}
