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
    private MetaPhaseSequencer metaPhaseSequencer = default;

    [SerializeField]
    private Transform benchParent = default;

    [SerializeField]
    private Vector2 buyPhaseBenchPos = default;

    [SerializeField]
    private Vector2 trialPhaseBenchPos = default;

    private readonly int[] PHASE_GUILT_REQUIREMENTS = new int[]
    {
        1,
        2,
        3,
        5,
        7,
    };

    private readonly string[] PHASE_TRIAL_NAMES = new string[]
    {
        "mock trial #1",
        "mock trial #2",
        "pre-trial #1",
        "pre-trial #2",
        "final trial",
    };

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
        bool trialSucceeded = false;
        for (int i = 0; i < numRounds; i++)
        {
            benchParent.transform.position = trialPhaseBenchPos;
            yield return metaPhaseSequencer.MetaPhase(i, PHASE_GUILT_REQUIREMENTS, PHASE_TRIAL_NAMES, trialSucceeded);
            yield return new WaitForSeconds(0.1f);

            Tween.Position(benchParent, buyPhaseBenchPos, 0.25f, 0f, Tween.EaseInOut);
            yield return new WaitForSeconds(0.35f);
            yield return buyPhaseSequencer.BuySequence(PHASE_GUILT_REQUIREMENTS[i], PHASE_TRIAL_NAMES[i]);
            yield return new WaitForSeconds(0.1f);

            
            Tween.Position(benchParent, trialPhaseBenchPos, 0.25f, 0f, Tween.EaseInOut);
            yield return new WaitForSeconds(0.35f);
            yield return trialSequencer.TrialSequence(PHASE_GUILT_REQUIREMENTS[i], (bool succeeded) => trialSucceeded = succeeded);
        }
    }
}
