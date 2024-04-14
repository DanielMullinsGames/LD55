using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

[System.Serializable]
public class Defendant
{
    public string name;
    public Animator anim;
    public int[] guiltPhases;
}

public class GameFlowManager : ManagedBehaviour
{
    public static GameFlowManager instance;

    [SerializeField]
    private TrialSequencer trialSequencer = default;

    [SerializeField]
    private BuyPhaseSequencer buyPhaseSequencer = default;

    [SerializeField]
    private MetaPhaseSequencer metaPhaseSequencer = default;

    [SerializeField]
    private Transform benchParent = default;

    [SerializeField]
    private Transform defendantStand = default;

    [SerializeField]
    private Vector2 buyPhaseBenchPos = default;

    [SerializeField]
    private Vector2 trialPhaseBenchPos = default;

    [SerializeField]
    private Vector2 trialDefendantPos = default;

    [SerializeField]
    private TMPro.TextMeshPro defendantText = default;

    [SerializeField]
    private TMPro.TextMeshPro finalVerdictText = default;

    [Header("DEFENDANTS")]
    [SerializeField]
    private List<Defendant> defendants = new();
    public static int defendantIndex = 0;

    private readonly string[] PHASE_TRIAL_NAMES = new string[]
    {
        "mock trial #1",
        "mock trial #2",
        "pre-trial #1",
        "pre-trial #2",
        "final trial",
    };

    protected override void ManagedInitialize()
    {
        instance = this;
    }

    private void Start()
    {
        defendants.ForEach(x => x.anim.gameObject.SetActive(false));
        StartCoroutine(GameSequence());
    }

    public void ShowDefendantFear(bool fear)
    {
        defendants[defendantIndex].anim.SetBool("fear", fear);
    }

    private IEnumerator GameSequence()
    {
        yield return DefendantIntroSequence();
        yield return DefendantSequence();
    }

    private IEnumerator DefendantIntroSequence()
    {
        benchParent.transform.position = new Vector2(buyPhaseBenchPos.x, -14f);
        defendantStand.transform.position = new Vector2(0f, -14f);
        defendants[defendantIndex].anim.gameObject.SetActive(true);
        ShowDefendantFear(true);

        yield return new WaitForSeconds(1.5f);
        defendantText.gameObject.SetActive(true);
        defendantText.text = "Defendant: " + defendants[defendantIndex].name;
        AudioController.Instance.PlaySound2D("horn_1");

        yield return new WaitForSeconds(0.5f);
        AudioController.Instance.PlaySound2D("whoosh");
        Tween.Position(defendantStand, new Vector2(0f, trialDefendantPos.y), 1f, 0f, Tween.EaseOutStrong);

        yield return new WaitForSeconds(2f);
        AudioController.Instance.PlaySound2D("whoosh");
        Tween.Position(defendantStand, trialDefendantPos, 1f, 0f, Tween.EaseOutStrong);

        yield return new WaitForSeconds(0.5f);
        ShowDefendantFear(false);
        defendantText.gameObject.SetActive(false);
        AudioController.Instance.PlaySound2D("horn_1");
    }

    private IEnumerator DefendantSequence()
    {
        var guiltReqs = defendants[defendantIndex].guiltPhases;

        int numRounds = 5;
        bool trialSucceeded = false;
        for (int i = 0; i < numRounds; i++)
        {
            yield return metaPhaseSequencer.MetaPhase(i, guiltReqs, PHASE_TRIAL_NAMES, trialSucceeded);
            yield return new WaitForSeconds(0.1f);

            Tween.Position(benchParent, buyPhaseBenchPos, 0.25f, 0f, Tween.EaseInOut);
            AudioController.Instance.PlaySound2D("whoosh", pitch: new AudioParams.Pitch(0.5f));
            yield return new WaitForSeconds(0.35f);
            yield return buyPhaseSequencer.BuySequence(guiltReqs[i], PHASE_TRIAL_NAMES[i]);
            yield return new WaitForSeconds(0.1f);

            
            Tween.Position(benchParent, trialPhaseBenchPos, 0.25f, 0f, Tween.EaseInOut);
            AudioController.Instance.PlaySound2D("whoosh", pitch: new AudioParams.Pitch(0.5f));
            yield return new WaitForSeconds(0.35f);
            yield return trialSequencer.TrialSequence(guiltReqs[i], (bool succeeded) => trialSucceeded = succeeded);
        }
    }
}
